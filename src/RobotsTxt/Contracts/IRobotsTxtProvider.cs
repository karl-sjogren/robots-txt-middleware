using System;
using System.Threading;
using System.Threading.Tasks;

namespace RobotsTxt.Contracts {
    public interface IRobotsTxtProvider {
        ValueTask<Memory<byte>> GetRobotsTxtAsync(CancellationToken cancellationToken);
        ValueTask<TimeSpan> GetMaxAgeAsync(CancellationToken cancellationToken);
    }
}
