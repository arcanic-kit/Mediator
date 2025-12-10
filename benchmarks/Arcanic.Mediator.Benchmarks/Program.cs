using BenchmarkDotNet.Running;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.InProcess.Emit;
using Arcanic.Mediator.Benchmarks.Commands;
using Arcanic.Mediator.Benchmarks.Events;
using Arcanic.Mediator.Benchmarks.Queries;

namespace Arcanic.Mediator.Benchmarks;

/// <summary>
/// Entry point for the Arcanic Mediator performance benchmarking application.
/// Provides interactive and command-line interfaces for running performance tests on query, command, and event processing.
/// </summary>
public class Program
{
    /// <summary>
    /// Main entry point for the benchmarking application.
    /// Supports both interactive mode and command-line arguments for running specific benchmark categories.
    /// </summary>
    /// <param name="args">Command line arguments specifying which benchmarks to run. Valid options: query, command, event, all</param>
    public static void Main(string[] args)
    {
        Console.WriteLine("=== Arcanic Mediator Performance Benchmarks ===");
        Console.WriteLine();

        // Create a custom configuration
        var config = ManualConfig.Create(DefaultConfig.Instance)
            .AddJob(Job.Default.WithToolchain(InProcessEmitToolchain.Instance))
            .WithOptions(ConfigOptions.DisableOptimizationsValidator);

        if (args.Length == 0)
        {
            Console.WriteLine("Choose benchmark to run:");
            Console.WriteLine("1. Query Benchmarks");
            Console.WriteLine("2. Command Benchmarks");
            Console.WriteLine("3. Event Benchmarks");
            Console.WriteLine("4. All Benchmarks");
            Console.WriteLine();
            Console.Write("Enter your choice (1-4): ");
            
            var choice = Console.ReadLine();
            
            switch (choice)
            {
                case "1":
                    BenchmarkRunner.Run<QueryBenchmarks>(config);
                    break;
                case "2":
                    BenchmarkRunner.Run<CommandBenchmarks>(config);
                    break;
                case "3":
                    BenchmarkRunner.Run<EventBenchmarks>(config);
                    break;
                case "4":
                    RunAllBenchmarks(config);
                    break;
                default:
                    Console.WriteLine("Invalid choice. Running all benchmarks...");
                    RunAllBenchmarks(config);
                    break;
            }
        }
        else
        {
            // Handle command line arguments
            var benchmarkType = args[0].ToLowerInvariant();
            
            switch (benchmarkType)
            {
                case "query" or "queries":
                    BenchmarkRunner.Run<QueryBenchmarks>(config);
                    break;
                case "command" or "commands":
                    BenchmarkRunner.Run<CommandBenchmarks>(config);
                    break;
                case "event" or "events":
                    BenchmarkRunner.Run<EventBenchmarks>(config);
                    break;
                case "all":
                    RunAllBenchmarks(config);
                    break;
                default:
                    Console.WriteLine($"Unknown benchmark type: {benchmarkType}");
                    Console.WriteLine("Valid options: query, command, event, all, quick");
                    break;
            }
        }
        
        Console.WriteLine();
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }

    /// <summary>
    /// Executes all available benchmark categories in sequence.
    /// Runs query, command, and event benchmarks with the specified configuration.
    /// </summary>
    /// <param name="config">The benchmark configuration to use for all benchmark runs.</param>
    private static void RunAllBenchmarks(IConfig config)
    {
        Console.WriteLine("Running all benchmarks...");
        Console.WriteLine();
        
        Console.WriteLine("=== Query Benchmarks ===");
        BenchmarkRunner.Run<QueryBenchmarks>(config);
        
        Console.WriteLine();
        Console.WriteLine("=== Command Benchmarks ===");
        BenchmarkRunner.Run<CommandBenchmarks>(config);
        
        Console.WriteLine();
        Console.WriteLine("=== Event Benchmarks ===");
        BenchmarkRunner.Run<EventBenchmarks>(config);
    }
}