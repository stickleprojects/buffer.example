namespace ConsoleApp1
{
    public class TaskEngineOptions<TRECORD>
    {
        public int MaxParallelThreads { get; set; }
        public IDataProvider<TRECORD> DataProvider { get; set; }
        public IDataProcessor<TRECORD> DataProcessor { get; set; }
    }
}