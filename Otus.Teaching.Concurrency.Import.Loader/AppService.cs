using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Otus.Teaching.Concurrency.Import.Core.Loaders;
using Otus.Teaching.Concurrency.Import.Handler.Data;

namespace Otus.Teaching.Concurrency.Import.Loader
{
    class AppService : IAppService
    {
        private readonly ILogger<AppService> _log;
        private readonly IConfiguration _config;
        private readonly IDataLoader _loader;
        private readonly IGeneratorFactory _generator;

        public AppService(ILogger<AppService> log, IConfiguration config, IDataLoader loader, IGeneratorFactory generator)
        {
            _log = log;
            _config = config;
            _loader = loader;
            _generator = generator;
        }

        public void Run()
        {
            Generate();
            Load();
        }
        void Generate()        
        {
            using (var x = _generator.GetGenerator())
            {
                x.Generate();
            };
        }
        void Load() 
        {
            _loader.LoadData();
        }

    }
}
