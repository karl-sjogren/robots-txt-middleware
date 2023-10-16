namespace RobotsTxt;

public interface IRobotsTxtProvider {
    Task<RobotsTxtResult> GetResultAsync(CancellationToken cancellationToken);
}
