using System.IO;
using System.Collections.Generic;
using System.Xml.Serialization;
using Otus.Teaching.Concurrency.Import.Core.Parsers;
using Otus.Teaching.Concurrency.Import.Handler.Entities;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Linq;

namespace Otus.Teaching.Concurrency.Import.DataAccess.Parsers
{
    public class CsvParser : IDataParser<List<Customer>>
    {
        private readonly Stream _stream;

        public CsvParser(Stream stream)
        {
            _stream = stream;
        }

        public List<Customer> Parse()
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
            };

            using var csv = new CsvReader(new StreamReader(_stream), config);
            return csv.GetRecords<Customer>().ToList();
        }
        
    }
}