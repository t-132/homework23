using System;
using System.IO;
using System.Collections.Generic;
using Otus.Teaching.Concurrency.Import.Core.Parsers;
using Otus.Teaching.Concurrency.Import.Handler.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Otus.Teaching.Concurrency.Import.Core.Loaders;
using Otus.Teaching.Concurrency.Import.Handler.Data;

namespace Otus.Teaching.Concurrency.Import.DataAccess.Parsers
{
    public class PaserFactory : IDataParserFactory<List<Customer>>
    {

        private readonly ILogger _log;
        private readonly IConfiguration _config;

        public PaserFactory(ILogger log, IConfiguration config)
        {
            _log = log;
            _config = config;
        }

        public IDataParser<List<Customer>> GetDefaultParser()
        {
            return new XmlParser(Console.OpenStandardInput());
        }

        public IDataParser<List<Customer>> GetParser(string parserFormat, string fileName)
        {            
            var file = new FileStream(fileName, FileMode.OpenOrCreate);

            if (parserFormat == "csv") return new CsvParser(file);
            if (parserFormat == "xml") return new XmlParser(file);
            return GetDefaultParser();
        }
        
    }
}