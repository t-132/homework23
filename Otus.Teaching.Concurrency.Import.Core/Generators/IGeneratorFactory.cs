using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Otus.Teaching.Concurrency.Import.Handler.Data
{
    public interface IGeneratorFactory
    {
        IDataGenerator GetGenerator();
    }
}
