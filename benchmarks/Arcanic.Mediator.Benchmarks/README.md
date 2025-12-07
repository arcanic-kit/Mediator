# Arcanic Mediator vs MediatR Benchmarks

This project contains comprehensive performance benchmarks comparing the **Arcanic Mediator** library against **MediatR**, two popular mediator pattern implementations for .NET.

## Overview

The benchmarks compare the performance characteristics of both libraries across different scenarios:

- **Query Processing**: Single and complex queries, high throughput scenarios
- **Command Processing**: Commands with and without return values, batch operations
- **Event Processing**: Single and multiple event handlers, event publishing patterns

## Benchmark Categories

### Query Benchmarks
- **SimpleQuery**: Basic user retrieval operations
- **ComplexQuery**: Multi-parameter search operations
- **HighThroughput**: Processing 100 concurrent queries

### Command Benchmarks
- **CommandWithResult**: Commands that return results
- **VoidCommand**: Commands that perform actions without return values
- **ComplexCommand**: Commands with multiple parameters and validation
- **HighThroughput**: Processing 100 concurrent commands

### Event Benchmarks
- **SimpleEvent**: Basic event publishing with multiple handlers
- **ComplexEvent**: Events with complex data structures
- **EventWithCleanup**: Events that trigger cleanup operations
- **HighThroughput**: Publishing 100 concurrent events

## Running the Benchmarks

### Prerequisites
- .NET 8.0 SDK or later
- Visual Studio 2022 or VS Code

### Command Line Usage

```bash
# Navigate to the benchmark project directory
cd benchmarks/Arcanic.Mediator.Benchmarks

# Run all benchmarks
dotnet run --configuration Release

# Run specific benchmark types
dotnet run --configuration Release -- query
dotnet run --configuration Release -- command
dotnet run --configuration Release -- event
dotnet run --configuration Release -- all
dotnet run --configuration Release -- quick
```

### Interactive Mode

Run the project without arguments for an interactive menu:

```bash
dotnet run --configuration Release
```

You'll be presented with options to run specific benchmark categories.

## Benchmark Results

The benchmarks measure:

- **Mean execution time** per operation
- **Memory allocation** per operation
- **Throughput** (operations per second)
- **Allocation rate** (bytes allocated per operation)

Results are exported in multiple formats:
- Console output with summary statistics
- HTML reports with detailed charts
- CSV files for further analysis

## Project Structure

```
benchmarks/Arcanic.Mediator.Benchmarks/
??? Benchmarks/           # Benchmark test classes
?   ??? QueryBenchmarks.cs
?   ??? CommandBenchmarks.cs
?   ??? EventBenchmarks.cs
??? Models/               # Shared data models
?   ??? Queries.cs
?   ??? Commands.cs
?   ??? Events.cs
??? MediatR/             # MediatR implementations
?   ??? Queries/
?   ??? Commands/
?   ??? Events/
??? Arcanic/             # Arcanic Mediator implementations
?   ??? Queries/
?   ??? Commands/
?   ??? Events/
??? Configuration/       # DI setup for both libraries
?   ??? MediatRConfiguration.cs
?   ??? ArcanicMediatorConfiguration.cs
??? Program.cs          # Benchmark runner
```

## Understanding the Results

### Performance Metrics

- **Mean**: Average execution time per operation
- **Error**: Half of 99.9% confidence interval
- **StdDev**: Standard deviation of all measurements
- **Median**: Value separating the higher half from the lower half
- **Allocated**: Total memory allocated per operation

### Baseline Comparison

MediatR is typically used as the baseline (marked with `Baseline = true`), making it easier to see relative performance differences.

### Memory Diagnostics

The `[MemoryDiagnoser]` attribute provides detailed memory allocation information, helping identify:
- Memory pressure differences
- GC collection frequency
- Allocation patterns

## Key Performance Areas

### Throughput Testing
Both libraries are tested under high-load scenarios (100 concurrent operations) to evaluate:
- Scalability characteristics
- Resource utilization
- Performance degradation under load

### Cold Start Performance
Initial operation performance is measured to understand:
- DI container resolution overhead
- Handler instantiation costs
- Framework initialization impact

### Steady State Performance
Repeated operations are measured to understand:
- JIT compilation benefits
- Caching effects
- Memory allocation patterns

## Contributing

To add new benchmarks:

1. Create test models in the `Models/` folder
2. Implement handlers for both libraries
3. Add benchmark methods to appropriate benchmark classes
4. Follow the existing naming conventions

## Notes

- All benchmarks run in **Release** configuration for accurate performance measurement
- The benchmarks use BenchmarkDotNet's statistical analysis for reliable results
- Memory diagnostics are enabled to track allocation patterns
- Multiple iterations ensure statistical significance

## Results Interpretation

When analyzing results, consider:

1. **Relative Performance**: How much faster/slower is one implementation
2. **Memory Efficiency**: Which implementation allocates less memory
3. **Consistency**: Which implementation has lower variance in execution time
4. **Scalability**: How performance changes under high load

The benchmarks aim to provide objective performance data to help make informed decisions when choosing between these mediator implementations.