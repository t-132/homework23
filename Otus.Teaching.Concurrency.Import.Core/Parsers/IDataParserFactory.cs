namespace Otus.Teaching.Concurrency.Import.Core.Parsers
{
    public interface IDataParserFactory<T>
    {
        IDataParser<T> GetParser(string type, string fileName);
    }
}