using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp1
{
    public class SampleDataProvider<TRECORD> : IDataProvider<TRECORD>
    {
        private IEnumerable<TRECORD> _data;
        public int Maxvalues { get; }

        public SampleDataProvider(int maxvalues, Func<int, TRECORD> factory)
        {
            Maxvalues = maxvalues;
            _data = Enumerable.Range(0, Maxvalues).Select(i => factory(i));
        }

        public IEnumerable<TRECORD> GetData()
        {
            return _data;
        }
    }
}