using System;
using Otus.Teaching.Concurrency.Import.Handler.Entities;
using Otus.Teaching.Concurrency.Import.Handler.Repositories;
using Otus.Teaching.Concurrency.Import.Core.DataAccess;

namespace Otus.Teaching.Concurrency.Import.DataAccess.Repositories
{
    public class CustomerRepository
        : ICustomerRepository
    {
        private readonly IDataWriter<Customer> _writer;

        public CustomerRepository(IDataWriterFactory writer)
        {
            _writer = (IDataWriter<Customer>)writer.GetWriter();
        }
        public bool AddCustomer(Customer customer)
        {
            return _writer.Write(customer);   
        }
    }
   
}