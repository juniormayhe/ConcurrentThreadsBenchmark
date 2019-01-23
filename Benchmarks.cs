using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AutoFixture;

using BenchmarkDotNet.Attributes;

using ConcurrentThreadsBenchmark.Models;

using Polly;

namespace ConcurrentThreadsBenchmark
{

    [CoreJob]
    [MemoryDiagnoser, RankColumn]
    public class Benchmarks
    {
        private readonly int MAX_THREADS = 8;
        private IEnumerable<Order> orders;
        private Fixture fixture;

        public Benchmarks()
        {
            
        }

        [GlobalSetup]
        public void Setup()
        {
            this.fixture = new Fixture();
            this.orders = fixture.CreateMany<Order>(100000);
        }

        /// <summary>
        /// Start with one task for each thread in loop trying to read queue.
        /// exists when there are no more items left in queue
        /// </summary>
        /// <returns></returns>
        [Benchmark]
        public async Task RunConcurrentQueue()
        {
            var queue = new ConcurrentQueue<Order>(this.orders);
            var tasks = new List<Task>();

            for (int n = 0; n < MAX_THREADS; n++)
            {
                tasks.Add(Task.Run(async () =>
                {
                    while (queue.TryDequeue(out Order order))
                    {
                        double orderTotal = order.OrderItems.Sum(x => x.ItemValue);
                        Console.WriteLine($"Total of order {order.Id} is {orderTotal}");
                    }
                }));
            }
            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Run with an initialcount of maximum number of threads. 
        /// WaitAsync to wait until it is ok to queue up another item.
        /// Immediataly kick off 4 tasks, each task for each thread and wait 
        /// first of them to finish before get past wait async 
        /// to add the next in queue
        /// </summary>
        /// <returns></returns>

        [Benchmark]
        public async Task RunWithSemaphoreSlim()
        {
            var allTasks = new List<Task>();
            var throttler = new SemaphoreSlim(initialCount: MAX_THREADS);
            foreach (var order in this.orders)
            {
                await throttler.WaitAsync();
                allTasks.Add(
                    Task.Run(async () =>
                    {
                        try
                        {
                            double orderTotal = order.OrderItems.Sum(x => x.ItemValue);
                            Console.WriteLine($"Total of order {order.Id} is {orderTotal}");
                        }
                        finally
                        {
                            throttler.Release();
                        }
                    }));
            }
            await Task.WhenAll(allTasks);
        }

        [Benchmark]
        public void RunWithParallelForeach()
        {
            var options = new ParallelOptions() { MaxDegreeOfParallelism = MAX_THREADS };
            Parallel.ForEach(this.orders, options, order =>
            {
                double orderTotal = order.OrderItems.Sum(x => x.ItemValue);
                Console.WriteLine($"Total of order {order.Id} is {orderTotal}");
            });
        }

        /// <summary>
        /// Polly builkhead policy restrict number of concurrent threads
        /// and optionally lets you queue up calls that exceed that number
        /// </summary>
        /// <returns></returns>

        [Benchmark]
        public async Task RunWithPolly()
        {
            var bulkhead = Policy.BulkheadAsync(MAX_THREADS, Int32.MaxValue);
            var tasks = new List<Task>();
            foreach (var order in this.orders)
            {
                // either run immediately or queue new item up if more than 4 threads
                var t = bulkhead.ExecuteAsync(async () =>
                {
                    double orderTotal = order.OrderItems.Sum(x => x.ItemValue);
                    Console.WriteLine($"Total of order {order.Id} is {orderTotal}");
                });
                tasks.Add(t);
            }
            await Task.WhenAll(tasks);
        }
    }
}