using System;
using System.Threading;
using System.Threading.Tasks;

namespace Configuration
{
    public interface IFix
    {
        Task Pre(IProgress<FixProgress> progress = null, CancellationToken cancelToken = default);

        Task Post(IProgress<FixProgress> progress = null, CancellationToken cancelToken = default);

        void ExecuteApplication(string executable, string[] args);
    }
}
