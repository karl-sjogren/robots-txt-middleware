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

        public ValueTask<Memory<byte>> GetRobotsTxtAsync(CancellationToken cancellationToken) {
            return new ValueTask<Memory<byte>>(_content);
        }

        public ValueTask<TimeSpan> GetMaxAgeAsync(CancellationToken cancellationToken) {
            return new ValueTask<TimeSpan>(_maxAge);
        }
    }
}
