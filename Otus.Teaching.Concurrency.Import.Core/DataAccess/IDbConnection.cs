using System;
using System.Collections.Generic;
using System.Text;

namespace Otus.Teaching.Concurrency.Import.Core.DataAccess
{
    public interface IOtusDbConnection
    {
        object GetConnection();
    }
}
