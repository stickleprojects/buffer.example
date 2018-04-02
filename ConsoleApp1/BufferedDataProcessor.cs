using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class BufferedDataProcessor<TRECORD> : IDataProcessor<TRECORD>, IDisposable
    {
        protected Subject<TRECORD> _subject;

        public BufferedDataProcessor(TimeSpan maxWaitTime, int maxRecords)
        {
            initObservable(maxWaitTime, maxRecords);
        }

        protected virtual void initObservable(TimeSpan maxWaitTime, int maxRecords)
        {
            _subject = new Subject<TRECORD>();
            _subject
                .Buffer(maxWaitTime, maxRecords)
                .Where(x => x.Any())
                .Subscribe(onProcessRecords, onError, onComplete);
        }

        protected virtual void onError(Exception exception)
        {
            Debug.WriteLine("Exception!!!!" + exception.Message);
        }

        protected virtual void onComplete()
        {
            Debug.WriteLine("Completed"); //throw new NotImplementedException();
            ;
        }

        protected virtual void onProcessRecords(IList<TRECORD> records)
        {
            Debug.WriteLine($"Processing {records.Count} records");
            foreach (var d in records)
            {
                Debug.WriteLine($"\t{d.ToString()}");
            }
        }

        public Task<bool> Process(TRECORD record)
        {
            _subject.OnNext(record);
            return Task.FromResult(true);
        }

        ~BufferedDataProcessor()
        {
            Dispose();
        }

        public virtual void Dispose()
        {
        }
    }
}