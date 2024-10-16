using System.Diagnostics;

long ParallelSum(List<long> numbers)
{
    int chunkSize = 100;
    if (numbers.Count <= chunkSize)
    {
        return SyncSum(numbers);
    }
    var sums = new long[(numbers.Count + chunkSize - 1) / chunkSize];
    Parallel.ForEach(Enumerable.Range(0, sums.Length), i =>
    {
        var chunk = numbers.Skip(i * chunkSize).Take(chunkSize).ToList();
        sums[i] = chunk.Sum();
    });
    return ParallelSum(sums.ToList());
}

long SyncSum(List<long> numbers)
{
    long sum = 0;
    foreach (var item in numbers)
    {
        sum += item;
    }
    return sum;
}

Random random = new Random();

Stopwatch stopwatch = new Stopwatch();
stopwatch.Start();
List<long> numbers = Enumerable.Range(0, int.MaxValue-100).Select(i => random.Next(0,1000)).ToList().ConvertAll(x => (long)x);
stopwatch.Stop();
Console.WriteLine($"Время на подготовку данных: {stopwatch.ElapsedMilliseconds} мс");

stopwatch.Restart();
long parallelSumResult = ParallelSum(numbers);
stopwatch.Stop();
Console.WriteLine($"Результат ParallelSum: {parallelSumResult}, Время: {stopwatch.ElapsedMilliseconds} мс");

stopwatch.Restart();
long sumResult = SyncSum(numbers);
stopwatch.Stop();
Console.WriteLine($"Результат Sum: {sumResult}, Время: {stopwatch.ElapsedMilliseconds} мс");
