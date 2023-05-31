using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Otus.Teaching.Concurrency.Import.Core.DataAccess;
using Otus.Teaching.Concurrency.Import.Handler.Entities;

namespace Otus.Teaching.Concurrency.Import.DataAccess.DataAccess
{
    public class CustomerWriterFactory : IDataWriterFactory
    {
        private readonly ILogger _log;
        private readonly IConfiguration _config;
        private readonly IOtusDbConnection _connection;

        public CustomerWriterFactory(ILogger<CustomerWriterFactory> log, IConfiguration config)
        {
            _log = log;
            _config = config;
            _connection = new SQLServerDbConnection(log, config);
        }
        public object GetWriter()
        {
            return new SQLServerDbWriter<Customer>(_connection);
        }
    }
}
