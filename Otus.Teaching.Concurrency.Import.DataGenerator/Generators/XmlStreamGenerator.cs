using System.Collections.Generic;
using System.IO;
using System;
using System.Xml;
using System.Xml.Serialization;
using System.Linq;
using Otus.Teaching.Concurrency.Import.DataGenerator.Dto;
using Otus.Teaching.Concurrency.Import.Handler.Data;
using Otus.Teaching.Concurrency.Import.Handler.Entities;

namespace Otus.Teaching.Concurrency.Import.DataGenerator.Generators
{
    public class XmlStreamGenerator : IDataGenerator, IDisposable
    {        
        private readonly Stream _stream;
        private readonly int _count;

        public XmlStreamGenerator(int Count) 
        {           
            _stream = Console.OpenStandardOutput();
            _count = Count;
        }

        public XmlStreamGenerator(int Count, Stream Stream)
        {
            _count = Count;
            _stream = Stream;
        }

        public void Dispose()
        {
            _stream.Dispose();
        }

        public void Generate()
        {
            var customers = RandomCustomerGenerator.Generate(_count);                        
            new XmlSerializer(typeof(List<Customer>)).Serialize(_stream, customers.ToList());
            _stream.Flush();            
        }
    }
}