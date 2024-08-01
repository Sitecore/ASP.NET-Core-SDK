using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Benchmarks;

internal class Program
{
    private static void Main()
    {
        // ReSharper disable once JoinDeclarationAndInitializer
        IConfig config;

#if DEBUG
        config = new DebugInProcessConfig();
#else
        config = DefaultConfig.Instance;
#endif
        BenchmarkRunner.Run<ExperienceEditorMiddlewareBenchmarks>(config);
        BenchmarkRunner.Run<TrackingBenchmarks>(config);
    }
}