using System.Text;
using Microsoft.Extensions.Hosting;

namespace RobotsTxt;

public class StaticRobotsTxtProvider : IRobotsTxtProvider {
    private readonly Memory<byte> _content;
    private readonly Int32 _maxAge;

    public StaticRobotsTxtProvider(IEnumerable<RobotsTxtOptions> optionsEnumerable, IHostEnvironment hostingEnvironment) {
        var options = optionsEnumerable.ToArray();
        var robotsOptions = options.FirstOrDefault(o => hostingEnvironment.IsEnvironment(o.Environment));

        if(robotsOptions is null) {
            robotsOptions = options.FirstOrDefault(o => string.IsNullOrWhiteSpace(o.Environment));

            if(robotsOptions == null)
                throw new InvalidOperationException($"No RobotsTxtOptions matching environment \"{hostingEnvironment.EnvironmentName}\" or any environment found.");
        }

        var content = robotsOptions.ToString();

        if(string.IsNullOrWhiteSpace(content))
            content = "# This file didn't get any instructions so everyone is allowed";

        _content = Encoding.UTF8.GetBytes(content).AsMemory();

        _maxAge = Convert.ToInt32(robotsOptions.MaxAge.TotalSeconds);
    }

    public Task<RobotsTxtResult> GetResultAsync(CancellationToken cancellationToken) {
        var result = new RobotsTxtResult(_content, _maxAge);
        return Task.FromResult(result);
    }
}
