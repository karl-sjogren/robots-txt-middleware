using BenchmarkDotNet.Running;

namespace RobotsTxt.Benchmarks;

public static class Program {
    public static void Main() => BenchmarkRunner.Run<Benchmarks>();
}
