using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using RobotsTxt.Contracts;

namespace Samples {
    public class CoolRobotsTxtProvider : IRobotsTxtProvider {
        private readonly CoolContext _context;

        public CoolRobotsTxtProvider(CoolContext context) {
            _context = context;
        }

        public Task<RobotsTxtResult> GetResultAsync(CancellationToken cancellationToken) {
            var settings = await _context.Settings.FirstAsync();

            var builder = new RobotsTxtOptionsBuilder();
            string content;

            if(settings.RobotsTxt.AllowAll)
                content = builder.AllowAll().Build().ToString();
            else
                content = builder.DenyAll().Build().ToString();

            var buffer = Encoding.UTF8.GetBytes(content).AsMemory();
            return new RobotsTxtResult(buffer, settings.RobotsTxt.MaxAge);
        }
    }
}
