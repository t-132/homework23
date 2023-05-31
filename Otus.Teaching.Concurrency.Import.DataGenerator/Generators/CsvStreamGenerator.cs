
using System;
using System.IO;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Otus.Teaching.Concurrency.Import.DataGenerator.Dto;
using Otus.Teaching.Concurrency.Import.Handler.Data;
using Otus.Teaching.Concurrency.Import.Handler.Entities;

namespace Otus.Teaching.Concurrency.Import.DataGenerator.Generators
{
    public class CsvStreamGenerator : IDataGenerator, IDisposable
    {
        private readonly Stream _stream;
        private readonly int _count;

        public CsvStreamGenerator(int Count)
        {
            _stream = Console.OpenStandardOutput();
            _count = Count;
        }

        public CsvStreamGenerator(Stream Stream, int Count)
        {
            _stream = Stream;
            _count = Count;
        }

        public void Dispose()
        {
            _stream.Dispose();
        }

        public void Generate()
        {
            var customers = RandomCustomerGenerator.Generate(_count);

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {                
                HasHeaderRecord = false,
            };
            
            using (var writer = new StreamWriter(_stream))
            using (var csv = new CsvWriter(writer, config))
            {
                csv.WriteRecords(customers);
                writer.Flush();
            }
        }
    }    
}
