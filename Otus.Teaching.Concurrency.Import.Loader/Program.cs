using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Otus.Teaching.Concurrency.Import.Core.Loaders;
using Serilog;
using System;
using System.IO;
using CustomerRepository = Otus.Teaching.Concurrency.Import.DataAccess.Repositories.CustomerRepository;
using CustomerWriterFactory = Otus.Teaching.Concurrency.Import.DataAccess.DataAccess.CustomerWriterFactory;
using GeneratorFactory = Otus.Teaching.Concurrency.Import.Loader.Generators.GeneratorFactory;
using ICustomerRepository = Otus.Teaching.Concurrency.Import.Handler.Repositories.ICustomerRepository;
using IDataWriterFactory = Otus.Teaching.Concurrency.Import.Core.DataAccess.IDataWriterFactory;
using IGeneratorFactory = Otus.Teaching.Concurrency.Import.Handler.Data.IGeneratorFactory;


namespace Otus.Teaching.Concurrency.Import.Loader
{
    class Program
    {
        private static string _dataFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "customers.xml");
        
        static void Main(string[] args)
        {
            try
            {
                var builder = new ConfigurationBuilder();
                BuildConfig(builder);
                var config = builder.Build();

                Log.Logger = new LoggerConfiguration()
                   .ReadFrom.Configuration(config)
                   .Enrich.FromLogContext()
                   .WriteTo.Console()
                   .CreateLogger();

                var v = Host.CreateDefaultBuilder()
               .ConfigureServices((ctx, srv) =>
               {
                   srv.AddTransient<IAppService, AppService>();
                   srv.AddSingleton<IGeneratorFactory, GeneratorFactory>();
                   srv.AddTransient<IDataLoader, DataLoader>();
                   srv.AddSingleton<IDataWriterFactory, CustomerWriterFactory>();
                   srv.AddTransient<ICustomerRepository, CustomerRepository>();
                   srv.AddTransient<IDataWriterFactory, CustomerWriterFactory>();
               })
               .UseSerilog();
               
                var host = v.Build();
                IServiceScope serviceScope = host.Services.CreateScope();
                IServiceProvider provider = serviceScope.ServiceProvider;
                ActivatorUtilities.CreateInstance<AppService>(host.Services).Run();
                    

//              IServiceScope serviceScope = host.Services.CreateScope();
 //             IServiceProvider provider = serviceScope.ServiceProvider;
   
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        /*
        static void GenerateCustomersDataFile()
        {
            var xmlGenerator = new XmlStreamGenerator();
            xmlGenerator.Generate(10);
        }*/
        public static void BuildConfig(IConfigurationBuilder builder)
        {
            builder.SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
              .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ENVIRONMENT") ?? "Production"}.json", optional: true)
              .AddEnvironmentVariables();
        }
    }
}