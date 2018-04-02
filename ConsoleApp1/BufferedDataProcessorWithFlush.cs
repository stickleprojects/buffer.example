using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace ConsoleApp1
{
    public class BufferedDataProcessorWithFlush<TRECORD> : BufferedDataProcessor<TRECORD>
    {
        private Subject<TRECORD> _flush;

        public BufferedDataProcessorWithFlush(TimeSpan maxWaitTime, int maxRecords) : base(maxWaitTime, maxRecords)
        {
        }

        protected override void initObservable(TimeSpan maxWaitTime, int maxRecords)
        {
            _flush = new Subject<TRECORD>();
            _subject = new Subject<TRECORD>();
            _subject
                .Merge(_flush)
                .Buffer(maxWaitTime, maxRecords)

                // ignore null records
                .Select(y => y.Where(z => z != null).ToList())
                .Subscribe(onProcessRecords, onError, onComplete);
        }

        public override void Dispose()
        {
            _flush?.OnNext(default(TRECORD));
        }
    }
}