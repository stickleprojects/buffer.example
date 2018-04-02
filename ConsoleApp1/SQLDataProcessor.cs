using System.Diagnostics;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class SQLDataProcessor<TRECORD> : IDataProcessor<TRECORD>
    {
        private readonly string _connectionString;

        public SQLDataProcessor(string connectionString)
        {
            _connectionString = connectionString;
        }

        Task<bool> IDataProcessor<TRECORD>.Process(TRECORD record)
        {
            // all good here
            Debug.WriteLine("Processing record" + record.ToString());
            return Task.FromResult(true);
        }
    }
}