using System;

namespace Otus.Teaching.Concurrency.Import.Generator
{
    class Program
    {        
        private static string _dataFileType; 
        private static int _dataCount = 100; 
        
        static void Main(string[] args)
        {
            try
            {
                if (!TryValidateAndParseArgs(args))
                    return;

                GeneratorFactory.GetGenerator(Console.OpenStandardOutput(), _dataCount, _dataFileType).Generate();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
        }

        private static bool TryValidateAndParseArgs(string[] args)
        {
            int i = 0;
            if (args != null && args.Length > 0)
            {
                int dataCount;
                if (int.TryParse(args[i], out dataCount))
                {
                    i++;
                    _dataCount = dataCount;
                }
            }

            if (args.Length > i)
            {
                _dataFileType = args[i];
            }            
                        
            return true;
        }
    }
}