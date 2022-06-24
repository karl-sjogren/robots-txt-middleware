using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RobotsTxt.Samples;

public class CoolRobotsTxtProvider : IRobotsTxtProvider {
    private readonly CoolContext _context;

    public CoolRobotsTxtProvider(CoolContext context) {
        _context = context;
    }

    public async Task<RobotsTxtResult> GetResultAsync(CancellationToken cancellationToken) {
        var settings = await _context.Settings.FirstAsync();

        var builder = new RobotsTxtOptionsBuilder();

        RobotsTxtOptions options;
        if(settings.AllowAllRobots)
            options = builder.AllowAll().Build();
        else
            options = builder.DenyAll().Build();

        var content = options.ToString();
        var buffer = Encoding.UTF8.GetBytes(content).AsMemory();
        return new RobotsTxtResult(buffer, settings.RobotsTxtMaxAge);
    }
}

/// <summary>
/// Fake dbcontext to get the sample provider to compile
/// </summary>
public class CoolContext {
    public CoolDbSet Settings { get; set; }
}

public class CoolDbSet {
    public Task<CoolSettings> FirstAsync() {
        return Task.FromResult(new CoolSettings());
    }
}

public class CoolSettings {
    public bool AllowAllRobots => true;
    public Int32 RobotsTxtMaxAge => 3600;
}
