using System;

namespace Otus.Teaching.Concurrency.Import.Handler.Data
{
    public interface IDataGenerator : IDisposable
    {        
        void Generate();
    }
}