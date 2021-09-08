using System;
using BenchmarkDotNet;
using BenchmarkDotNet.Attributes;
using RobotsTxt;

namespace RobotsTxt.Benchmarks {
    [MemoryDiagnoser]
    public partial class Benchmarks {
        private readonly RobotsTxtOptionsBuilder _options = new RobotsTxtOptionsBuilder().AllowAll();

        public Benchmarks() {
        }

        [Benchmark]
        public void StaticRobotsTxtProvider() {
        }

        [Benchmark]
        public void ReusedOptions() {
        }
    }
}
