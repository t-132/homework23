using Otus.Teaching.Concurrency.Import.Handler.Data;
using XmlDataGenerator = Otus.Teaching.Concurrency.Import.DataGenerator.Generators.XmlStreamGenerator;
using CsvDataGenerator = Otus.Teaching.Concurrency.Import.DataGenerator.Generators.CsvStreamGenerator;
using System.IO;

namespace Otus.Teaching.Concurrency.Import.Generator
{
    public static class GeneratorFactory
    {
        public static IDataGenerator GetDefaultGenerator(Stream stream, int dataCount)
        {         
            return new XmlDataGenerator(dataCount, stream);
        }

        public static IDataGenerator GetGenerator(Stream stream, int dataCount, string generatorFormat = default)
        {
            if (generatorFormat == "csv") return new CsvDataGenerator(stream, dataCount);
            if (generatorFormat == "xml") return new XmlDataGenerator(dataCount, stream);
            return GetDefaultGenerator(stream, dataCount);
        }
    }
}