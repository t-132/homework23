using System;
using System.Collections.Generic;
using System.Text;

namespace Otus.Teaching.Concurrency.Import.Core.DataAccess
{
    public interface IDataWriterFactory
    {
        object GetWriter();
    }
}
