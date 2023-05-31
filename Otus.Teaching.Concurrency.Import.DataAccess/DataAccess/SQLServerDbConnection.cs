using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;
using Otus.Teaching.Concurrency.Import.Core.DataAccess;

namespace Otus.Teaching.Concurrency.Import.DataAccess.DataAccess
{
    class SQLServerDbConnection : IOtusDbConnection
    {
        private readonly ILogger _log;
        private readonly IConfiguration _config;
        private readonly SqlConnectionStringBuilder _connectionString;

        public SQLServerDbConnection(ILogger log, IConfiguration config)
        {
            _log = log;
            _config = config;
            _connectionString = new SqlConnectionStringBuilder(_config.GetValue<string>("ConnectionString", "Server=(local);Database=test;Trusted_Connection=yes;"))
            {
                LoadBalanceTimeout = 5 * 60,
                ConnectTimeout = 60,
                Pooling = true,
                MaxPoolSize = _config.GetValue<int>("MaxPoolSize", 10),
                MinPoolSize = _config.GetValue<int>("MinPoolSize", 0)
            };            
        }

        public object GetConnection()
        {
            return (object)new SqlConnection(_connectionString.ConnectionString);
        }
        
    }
}
