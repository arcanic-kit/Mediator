using BenchmarkDotNet.Running;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.InProcess.Emit;

namespace Arcanic.Mediator.Command.Benchmarks;

/// <summary>
/// Entry point for the Arcanic Mediator Command performance benchmarking application.
/// Provides interactive and command-line interfaces for running performance tests on command processing.
/// </summary>
public class Program
{
    /// <summary>
    /// Main entry point for the command benchmarking application.
    /// </summary>
    /// <param name="args">Command line arguments</param>
    public static void Main(string[] args)
    {
        Console.WriteLine("=== Arcanic Mediator Command Performance Benchmarks ===");
        Console.WriteLine();

        // Create a custom configuration
        var config = ManualConfig.Create(DefaultConfig.Instance)
            .AddJob(Job.Default.WithToolchain(InProcessEmitToolchain.Instance))
            .WithOptions(ConfigOptions.DisableOptimizationsValidator);

        Console.WriteLine("Running Command Benchmarks...");
        BenchmarkRunner.Run<CommandBenchmarks>(config);
        
        Console.WriteLine();
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}
