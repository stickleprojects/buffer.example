using System.Collections.Generic;

namespace ConsoleApp1
{
    public interface IDataProvider<TRECORD>
    {
        IEnumerable<TRECORD> GetData();
    }
}