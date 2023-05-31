using System.IO;
using System.Collections.Generic;
using System.Xml.Serialization;
using Otus.Teaching.Concurrency.Import.Core.Parsers;
using Otus.Teaching.Concurrency.Import.Handler.Entities;

namespace Otus.Teaching.Concurrency.Import.DataAccess.Parsers
{
    public class XmlParser: IDataParser<List<Customer>>
    {
        private readonly Stream _stream;

        public XmlParser(Stream stream)
        {
            _stream = stream;
        }
        public List<Customer> Parse()
        {
            return (List<Customer>)  new XmlSerializer(typeof(List<Customer>)).Deserialize(_stream);            
        }
    }
}