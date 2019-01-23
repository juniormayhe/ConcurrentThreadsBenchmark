using System;
using BenchmarkDotNet.Running;

namespace ConcurrentThreadsBenchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Bechmark with Concurrent Threads");

            Console.ForegroundColor = ConsoleColor.Green;
            var summary = BenchmarkRunner.Run<Benchmarks>();   
        }
    }
}
