using System;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.InProcess.Emit;

namespace Arcanic.Mediator.Event.Benchmarks;

/// <summary>
/// Entry point for the Arcanic Mediator Event performance benchmarking application.
/// Provides interactive and command-line interfaces for running performance tests on event processing.
/// </summary>
public class Program
{
    /// <summary>
    /// Main entry point for the event benchmarking application.
    /// </summary>
    /// <param name="args">Command line arguments</param>
    public static void Main(string[] args)
    {
        Console.WriteLine("=== Arcanic Mediator Event Performance Benchmarks ===");
        Console.WriteLine();

        // Create a custom configuration
        var config = ManualConfig.Create(DefaultConfig.Instance)
            .AddJob(Job.Default.WithToolchain(InProcessEmitToolchain.Instance))
            .WithOptions(ConfigOptions.DisableOptimizationsValidator);

        Console.WriteLine("Running Event Benchmarks...");
        BenchmarkRunner.Run<EventBenchmarks>(config);
        
        Console.WriteLine();
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}
