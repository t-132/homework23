using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Otus.Teaching.Concurrency.Import.DataAccess.Parsers;
using Otus.Teaching.Concurrency.Import.Handler.Entities;
using Otus.Teaching.Concurrency.Import.Handler.Repositories;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Otus.Teaching.Concurrency.Import.Core.Loaders
{
    public class DataLoader
        : IDataLoader
    {
        private readonly ILogger<DataLoader> _log;
        private readonly IConfiguration _config;        
        IServiceProvider _provider;

        public DataLoader(ILogger<DataLoader> log, IConfiguration config, IServiceProvider provider)
        {
            _log = log;
            _config = config;            
            _provider = provider;          
        }

        public void LoadData()
        {
            _log.LogInformation("Loading ...");

            var result = new PaserFactory(_log, _config)
                .GetParser(_config.GetValue<string>("DataFileType", ""), _config.GetValue<string>("DataFile", "out"))
                .Parse();
                        
            var threadCount = _config.GetValue<int>("ThreadCount", 1);
            var chunck = result.Count / threadCount;
            var remainder = result.Count % threadCount;
            var additive = 0;            

            if (chunck == 0)
            {
                threadCount = remainder;
                remainder = 0;
                chunck = 1;
            }

            var repairQueue = new ConcurrentQueue<Customer>();
            var errorQueue = new ConcurrentQueue<Customer>();
            var attempts = _config.GetValue<int>("RepairAttempts", 1);
            var useThreadPool = _config.GetValue<bool>("UseThreadPool", false);

            using (var evt = new CountdownEvent(threadCount))
            using (var evt2 = new CountdownEvent(threadCount))
            {

                var stopwatch = new Stopwatch();
                stopwatch.Start();
                for (int i = 0; i < threadCount;)
                {
                    var repo = _provider.GetService<ICustomerRepository>();
                    var beginChunck = i++ * chunck + additive;
                    if (remainder > 0)
                    {
                        remainder--;
                        additive++;
                    }
                    var endChunck = i * chunck + additive;

                    if (useThreadPool)
                    {
                        ThreadPool.QueueUserWorkItem(new WaitCallback((state) =>
                        {
                            LoadWorker(result, beginChunck, endChunck, repairQueue, evt, repo);
                            evt.Wait();
                            RepairWorker(repairQueue, errorQueue, evt2, attempts, repo);
                        }));
                    }
                    else
                    {
                        new Thread(() =>
                        {
                            LoadWorker(result, beginChunck, endChunck, repairQueue, evt, repo);
                            evt.Wait();
                            RepairWorker(repairQueue, errorQueue, evt2, attempts, repo);
                        }).Start();
                    }                                        
                }

                evt2.Wait();

                stopwatch.Stop();
                _log.LogInformation($"Elapsed: {stopwatch.ElapsedMilliseconds}");

                while (errorQueue.TryDequeue(out Customer errCustomer))
                {
                    _log.LogInformation($"Error load {errCustomer.Id} ");
                }
            }

            _log.LogInformation("Loaded");        
        }

        void LoadWorker(List<Customer> buffer, int start, int end, ConcurrentQueue<Customer> repairQueue, CountdownEvent loadEvent, ICustomerRepository repo)
        {
            for (int j = start; j < end; j++)
                if (!repo.AddCustomer(buffer[j]))
                    repairQueue.Enqueue(buffer[j]);

            loadEvent.Signal();
        }

        void RepairWorker(ConcurrentQueue<Customer> repairQueue, ConcurrentQueue<Customer> errorQueue, CountdownEvent repairEvent, int attempts, ICustomerRepository repo)
        {
            while (repairQueue.TryDequeue(out Customer repair))
            {
                for (int tryCount = 0; !repo.AddCustomer(repair); tryCount++)
                {
                    if (tryCount >= attempts)
                    {
                        errorQueue.Enqueue(repair);
                        break;
                    }
                }             
            }

            repairEvent.Signal();
        }

    }
    
}