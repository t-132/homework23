using System;
using System.Collections.Generic;
using System.Text;

namespace Otus.Teaching.Concurrency.Import.Core.DataAccess
{
    public interface IDataWriter<T> where T : new()
    {
        bool Write(T data);
        bool Write(IEnumerable<T> data);
        bool Write(IEnumerator<T> data);
    }
}
