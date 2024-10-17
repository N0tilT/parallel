using Pyramidal.Core;
using System.Diagnostics;


Random random = new Random();
Summator summator = new Summator();

Stopwatch stopwatch = new Stopwatch();

List<int> sizes = new List<int>() { 100000, 200000, 300000, 500000, 1000000, 2000000, 5000000, 10000000 };

for (int i = 6; i < 10; i++)
{
    int size = (int)Math.Pow(10, i);
    stopwatch.Start();
    List<long> numbers = Enumerable.Range(0, size).Select(i => random.Next(0, 100)).ToList().ConvertAll(x => (long)x);
    stopwatch.Stop();
    Console.WriteLine($"Время на подготовку данных ({numbers.Count} элементов): {stopwatch.ElapsedMilliseconds} мс");
    Console.WriteLine("THREADS");
    for (int threadCount = 2; threadCount <= Environment.ProcessorCount; threadCount++)
    {
        stopwatch.Restart();
        long parallelSumResult = summator.ParallelSum(numbers, threadCount);
        stopwatch.Stop();
        Console.WriteLine($"Результат ParallelSum: {parallelSumResult}, ThreadCount: {threadCount}, Время: {stopwatch.ElapsedMilliseconds} мс");
    }
    Console.WriteLine("TASKS");
    for (int threadCount = 2; threadCount <= Environment.ProcessorCount; threadCount++)
    {
        stopwatch.Restart();
        long parallelSumResult = summator.ParallelSumTasks(numbers, threadCount);
        stopwatch.Stop();
        Console.WriteLine($"Результат ParallelSum: {parallelSumResult}, TaskCount: {threadCount}, Время: {stopwatch.ElapsedMilliseconds} мс");
    }

    Console.WriteLine("RECURSION\n----------\n");
    Console.WriteLine("THREADS");
    for (int threadCount = 2; threadCount <= Environment.ProcessorCount; threadCount++)
    {
        stopwatch.Restart();
        long parallelSumResult = summator.ParallelSumRecursive(numbers, threadCount);
        stopwatch.Stop();
        Console.WriteLine($"Результат ParallelSum: {parallelSumResult}, ThreadCount: {threadCount}, Время: {stopwatch.ElapsedMilliseconds} мс");
    }
    Console.WriteLine("TASKS");
    for (int threadCount = 2; threadCount <= Environment.ProcessorCount; threadCount++)
    {
        stopwatch.Restart();
        long parallelSumResult = summator.ParallelSumTasksRecursive(numbers, threadCount);
        stopwatch.Stop();
        Console.WriteLine($"Результат ParallelSum: {parallelSumResult}, TaskCount: {threadCount}, Время: {stopwatch.ElapsedMilliseconds} мс");
    }

    Console.WriteLine("SYNCRONOUS");
    stopwatch.Restart();
    long sumResult = summator.SyncSum(numbers);
    stopwatch.Stop();
    Console.WriteLine($"Результат Sum: {sumResult}, Время: {stopwatch.ElapsedMilliseconds} мс");

}
