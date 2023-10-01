using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RobotsTxt;

public class StaticRobotsTxtProvider : IRobotsTxtProvider {
    private readonly Memory<byte> _content;
    private readonly Int32 _maxAge;

    public StaticRobotsTxtProvider(RobotsTxtOptions options) {
        var content = options.ToString();

        if(string.IsNullOrWhiteSpace(content))
            content = "# This file didn't get any instructions so everyone is allowed";

        _content = Encoding.UTF8.GetBytes(content).AsMemory();

        _maxAge = Convert.ToInt32(options.MaxAge.TotalSeconds);
    }

    public Task<RobotsTxtResult> GetResultAsync(CancellationToken cancellationToken) {
        var result = new RobotsTxtResult(_content, _maxAge);
        return Task.FromResult(result);
    }
}
