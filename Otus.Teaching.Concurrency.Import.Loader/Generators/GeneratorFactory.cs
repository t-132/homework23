using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Otus.Teaching.Concurrency.Import.Core.Loaders;
using Otus.Teaching.Concurrency.Import.Handler.Data;
using XmlDataGenerator = Otus.Teaching.Concurrency.Import.DataGenerator.Generators.XmlStreamGenerator;
using CsvDataGenerator = Otus.Teaching.Concurrency.Import.DataGenerator.Generators.CsvStreamGenerator;

namespace Otus.Teaching.Concurrency.Import.Loader.Generators
{
    class GeneratorFactory : IGeneratorFactory
    {
        private readonly ILogger _log;
        private readonly IConfiguration _config;        

        public GeneratorFactory(ILogger<GeneratorFactory> log, IConfiguration config)
        {
            _log = log;
            _config = config;         
        }

        public IDataGenerator GetGenerator()
        {
            if (_config.GetValue<string>("Generator", "inner") == "outer")
                return new ProcessGenerator(_log,_config);

            var count = _config.GetValue<int>("Count", 0);
            var fileType = _config.GetValue<string>("DataFileType", "");
            var file = new FileStream(_config.GetValue<string>("DataFile", "out"), FileMode.Create);

            if (fileType == "csv")
                return new CsvDataGenerator(file, count);
            if (fileType == "xml")
                return new XmlDataGenerator(count, file);
            
            return new XmlDataGenerator(count, file);
        }


    }
}
