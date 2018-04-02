using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class TaskEngine<TRECORD>
    {
        private TaskEngineOptions<TRECORD> options;

        public TaskEngine(TaskEngineOptions<TRECORD> options)
        {
            this.options = options;
        }

        public void Run()
        {
            var data = options.DataProvider.GetData();

            var opts = new ParallelOptions()
            {
                MaxDegreeOfParallelism = options.MaxParallelThreads
            };
            Parallel.ForEach(data, opts, (record, state) =>
            {
                //   Debug.WriteLine("Parallel pushing " + record.ToString());
                // do some processing (ostensibly we'd update the sql record here)
                options.DataProcessor.Process(record).Wait();
            });
        }
    }
}