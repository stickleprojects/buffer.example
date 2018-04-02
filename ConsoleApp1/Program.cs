using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using YetAnotherConsoleTables;

namespace ConsoleApp1
{
    internal class Program
    {
        public class Result
        {
            public string Name { get; set; }
            public int RecordCount { get; set; }
            public long Milliseconds { get; set; }

            public override string ToString()
            {
                return $"{nameof(Name)}: {Name}, {nameof(RecordCount)}: {RecordCount}, {nameof(Milliseconds)}: {Milliseconds}";
            }

            public Result(string name, int recordCount, long milliseconds)
            {
                Name = name;
                RecordCount = recordCount;
                Milliseconds = milliseconds;
            }
        }

        private static void Main(string[] args)
        {
            Console.WriteLine("\n\nHello World!   " + DateTime.Now.ToString());

            // i want a bunch of tasks, writing to sql
            // prove i an put a buffer and increase throughput
            var connectionString = "server=tcp:192.168.99.100;database=testdb;user id=test;password=password";
            //IDataProcessor<ARecord> dataProcessor = new SQLDataProcessor<ARecord>(connectionString);

            TimeSpan maxWaitTime = TimeSpan.FromSeconds(3);
            int bufferSize = 30;
            int multiplier = 5;
            var loopsOfTest = 2;
            //var dataProcessor = new BufferedDataProcessorWithFlush<ARecord>(maxWaitTime, bufferSize);
            var maxParallelThreads = 5;
            //var dataSize = 30;

            var processors = new List<SQLBufferedDataProcessor<ARecord>>
            {
                new SQLBufferedDataProcessor<ARecord>.WithConnectionPerRecord(connectionString, maxWaitTime,
                    bufferSize),
                new SQLBufferedDataProcessor<ARecord>.WithConnectionPerListOfRecords(connectionString, maxWaitTime,
                    bufferSize),
            };

            var results = new List<Result>();
            foreach (var d in Enumerable.Range(30, loopsOfTest))
            {
                var dataSize = d * multiplier;
                multiplier *= multiplier;
                Console.WriteLine($"Running {dataSize}...");
                foreach (var dataProcessor in processors)
                {
                    var options = new TaskEngineOptions<ARecord>()
                    {
                        MaxParallelThreads = maxParallelThreads,
                        DataProcessor = dataProcessor,
                        DataProvider = new SampleDataProvider<ARecord>(dataSize, i => new ARecord(i))
                    };
                    var engine = new TaskEngine<ARecord>(options);
                    var t = Stopwatch.StartNew();

                    engine.Run();
                    t.Stop();
                    dataProcessor.Dispose();
                    Console.WriteLine($"{dataSize} took {t.ElapsedMilliseconds} ms");

                    results.Add(
                        new Result(dataProcessor.GetType().Name, dataSize, t.ElapsedMilliseconds));
                }

                //Thread.Sleep(TimeSpan.FromSeconds(4));
            }

            results.ForEach(r => { Debug.WriteLine(r); });
            ConsoleTable.From(results)

                .Write(ConsoleTableFormat.Plus);
        }
    }
}