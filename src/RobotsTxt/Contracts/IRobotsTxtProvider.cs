using System;
using System.Threading;
using System.Threading.Tasks;

namespace RobotsTxt.Contracts {
    public interface IRobotsTxtProvider {
        Task<Memory<byte>> GetRobotsTxtAsync(CancellationToken cancellationToken);
        Task<TimeSpan> GetMaxAgeAsync(CancellationToken cancellationToken);
    }
}
