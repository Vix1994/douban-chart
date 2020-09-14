using System.Threading.Tasks;

namespace DataCatch.Douban.Base
{
    public interface IProcess
    {
        Task Start();
        static bool IsRunning { get; }
    }
}
