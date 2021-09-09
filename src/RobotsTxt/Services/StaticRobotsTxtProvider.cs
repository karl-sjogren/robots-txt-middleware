using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RobotsTxt.Contracts;

namespace RobotsTxt.Services {
    public class StaticRobotsTxtProvider : IRobotsTxtProvider {
        private readonly Memory<byte> _content;
        private readonly TimeSpan _maxAge;

        public StaticRobotsTxtProvider(RobotsTxtOptions options) {
            var content = options.Build().ToString()?.TrimEnd();

            if(string.IsNullOrWhiteSpace(content))
                content = "# This file didn't get any instructions so everyone is allowed";

            _content = Encoding.UTF8.GetBytes(content).AsMemory();

            _maxAge = options.MaxAge;
        }

        public Task<Memory<byte>> GetRobotsTxtAsync(CancellationToken cancellationToken) {
            return Task.FromResult(_content);
        }

        public Task<TimeSpan> GetMaxAgeAsync(CancellationToken cancellationToken) {
            return Task.FromResult(_maxAge);
        }
    }
}
