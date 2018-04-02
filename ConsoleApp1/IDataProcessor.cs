using System.Threading.Tasks;

namespace ConsoleApp1
{
    public interface IDataProcessor<TRECORD>
    {
        Task<bool> Process(TRECORD record);
    }
}