using System.Threading;
using System.Threading.Tasks;

namespace RobotsTxt;

public interface IRobotsTxtProvider {
    Task<RobotsTxtResult> GetResultAsync(CancellationToken cancellationToken);
}
