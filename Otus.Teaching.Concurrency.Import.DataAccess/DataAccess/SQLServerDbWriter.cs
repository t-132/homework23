using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;
using Otus.Teaching.Concurrency.Import.Core.DataAccess;


namespace Otus.Teaching.Concurrency.Import.DataAccess.DataAccess
{
    public class SQLServerDbWriter<T> :IDisposable, IDataWriter<T> where T : new()
    {
        private readonly IOtusDbConnection _connection;
        private  SqlConnection _cnn = null;
        private SqlCommand _cmd;
        private SqlParameter[] _param;
        private readonly Type _tp;
        

        public SQLServerDbWriter(IOtusDbConnection connection) 
        {
            _connection = connection;
            
            _tp = typeof(T);
            _param = new SqlParameter[_tp.GetProperties().Length];
            var table = _tp.Name;
            
            string sql = "";
            string values = "";
            bool comma = false;

            int i = 0;
            foreach (var field in _tp.GetProperties())
            {                              
                if (comma)
                {
                    sql += "," + field.Name;
                    values += ",@" + field.Name; ;
                }
                else
                {
                    sql += field.Name; ;
                    values += "@" + field.Name; ;
                }                
                comma = true;
                _param[i++] = new SqlParameter(field.Name, field);
            }
            sql = $"insert into {table} ({sql}) values({values})";
            _cnn = (SqlConnection)_connection.GetConnection();
            _cmd = new SqlCommand(sql, _cnn);            
        }

        ~SQLServerDbWriter() 
        {
            if (!(_cnn is null))
                _cnn.Close();
            _cnn = null;
        }

        public void Dispose() 
        {
            if (!(_cnn is null))
                _cnn.Dispose();
            _cnn = null;
        }

        public bool Write(T data) 
        {
            int i = 0;
            foreach (var field in _tp.GetProperties())
            {
                _param[i++].Value = field.GetValue(data);
            }
            _cmd.Parameters.Clear();
            _cmd.Parameters.AddRange(_param);

            if (_cnn.State <= ConnectionState.Closed)
                _cnn.Open();

            if (_cmd.ExecuteNonQuery() > 0) return true;
            
            return false; 
        }

        public bool Write(IEnumerable<T> data)
        {
            throw new NotImplementedException();
        }

        public bool Write(IEnumerator<T> data)
        {
            throw new NotImplementedException();
        }
    }
}
