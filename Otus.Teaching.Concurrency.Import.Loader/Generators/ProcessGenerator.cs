using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Otus.Teaching.Concurrency.Import.Core.Loaders;
using Otus.Teaching.Concurrency.Import.Handler.Data;

namespace Otus.Teaching.Concurrency.Import.Loader.Generators
{
    class ProcessGenerator: IDataGenerator
    {
        private readonly ILogger _log;
        private readonly IConfiguration _config;         

        public ProcessGenerator(ILogger log, IConfiguration config)
        {
            _log = log;
            _config = config;        
        }

        public void Dispose()
        {            
        }

        public void Generate()
        {
            var count = _config.GetValue<int>("Count", 0);
            var fileType = _config.GetValue<string>("DataFileType", "");
            string args ="";
            if (count != 0 && fileType != "")
                args += $"{count} {fileType}";
            else
            {
                if (count != 0)
                    args += $"{count}";
                if (fileType != "")
                    args += fileType;
            }

            
            var startInfo = new ProcessStartInfo();
            startInfo.FileName = _config.GetValue<string>("OuterProcess", "Otus.Teaching.Concurrency.Import.DataGenerator.App.exe");
            startInfo.Arguments = args;
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;

            var process = new Process();
            process.StartInfo = startInfo;
            process.Exited += (sender, e) => _log.LogInformation($"Outer process exit: {process.ExitCode}");
            process.EnableRaisingEvents = true;

            using (var file = new StreamWriter(_config.GetValue<string>("DataFile", "out")))
            {
                process.OutputDataReceived += (s, e) => file.WriteLine(e.Data);
                process.ErrorDataReceived += (s, e) => { if (!(e.Data is null)) _log.LogError(e.Data); };
            
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                _log.LogInformation($"Outer process strted: {process.Id}");
                _log.LogInformation($"Waiting...");
                process.WaitForExit();
                file.Flush();
            }

            if (process.ExitCode != 0)
            {
                _log.LogCritical("Child process fail...");
                throw new Exception("Child process fail");
            }
        }
    }
}
