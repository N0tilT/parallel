using Pyramidal.Core;
using System.Diagnostics;


Random random = new Random();
Stopwatch stopwatch = new Stopwatch();

double start = 1;
double end = 10;
List<double> accuracy_list = new List<double>() { 0.1, 0.001, 0.0001 };
List<Func<double, double>> functions = new List<Func<double, double>> {
            (x) => {return (double)x/Math.Sqrt(x*x-16); },
            (x) => Math.Pow(x,4)*Math.Log(x)*Math.Log(x),
            (x) => 1/(3*Math.Cos(x)*Math.Cos(x)+2*Math.Sin(x)*Math.Sin(x)),
            (x) => Math.Sqrt(x)/(Math.Pow(x,3/4)+1),
            (x) => (x * x*x - 2*x*x-12*x-7)/(x*x*x-x*x)
        };

for (int i = 0; i < functions.Count; i++)
{
    Func<double, double>? function = functions[i];
    Console.WriteLine($"Функция {i}.");
    foreach (var accuracy in accuracy_list)
    {
        Console.WriteLine($"Точность {accuracy}.");
        stopwatch.Start();
        Console.WriteLine("SYNCRONOUS");
        stopwatch.Restart();
        double sumResult = Integrator.IntegrateSegments(function, start, end, accuracy, 4);
        stopwatch.Stop();
        Console.WriteLine($"Результат: {sumResult}, Время: {stopwatch.ElapsedMilliseconds} мс");
        Console.WriteLine("THREADSSegments");
        for (int threadCount = 2; threadCount <= Environment.ProcessorCount; threadCount++)
        {
            stopwatch.Restart();
            double parallelSumResult = Integrator.ParallelIntegrateSegmentsThreads(function, start, end, accuracy, threadCount);
            stopwatch.Stop();
            Console.WriteLine($"Результат: {parallelSumResult}, ThreadCount: {threadCount}, Время: {stopwatch.ElapsedMilliseconds} мс");
        }
        Console.WriteLine("TASKSSegments");
        for (int threadCount = 2; threadCount <= Environment.ProcessorCount; threadCount++)
        {
            stopwatch.Restart();
            double parallelSumResult = Integrator.ParallelIntegrateSegmentsTasks(function, start, end, accuracy, threadCount);
            stopwatch.Stop();
            Console.WriteLine($"Результат: {parallelSumResult}, TaskCount: {threadCount}, Время: {stopwatch.ElapsedMilliseconds} мс");
        }
        Console.WriteLine("THREADS");
        for (int threadCount = 2; threadCount <= Environment.ProcessorCount; threadCount++)
        {
            stopwatch.Restart();
            double parallelSumResult = Integrator.ParallelIntegrateThreads(function, start, end, accuracy, threadCount);
            stopwatch.Stop();
            Console.WriteLine($"Результат: {parallelSumResult}, ThreadCount: {threadCount}, Время: {stopwatch.ElapsedMilliseconds} мс");
        }
        Console.WriteLine("TASKS");
        for (int threadCount = 2; threadCount <= Environment.ProcessorCount; threadCount++)
        {
            stopwatch.Restart();
            double parallelSumResult = Integrator.ParallelIntegrateTasks(function, start, end, accuracy, threadCount);
            stopwatch.Stop();
            Console.WriteLine($"Результат: {parallelSumResult}, TaskCount: {threadCount}, Время: {stopwatch.ElapsedMilliseconds} мс");
        }


    }

}