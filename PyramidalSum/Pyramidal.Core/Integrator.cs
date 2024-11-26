using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyramidal.Core
{
    public static class Integrator
    {
        private static double SafeEvaluate(Func<double, double> f, double x)
        {
            double result = f(x);
            return double.IsNaN(result) ? 0 : double.IsInfinity(result) ? 0 : result;
        }
        public static double Integrate(Func<double, double> func, double start, double end, double eps = 0.001)
        {
            double step = (end - start) / 2;
            double S0 = 0, S = 0;
            for (double x = start; x < end; x += step)
            {
                S += SafeEvaluate(func, x);
            }
            S *= step;
            do
            {
                S0 = S;
                step /= 2;
                S = 0;
                for (double x = start + step; x < end; x += step * 2)
                {
                    S += SafeEvaluate(func, x);
                }
                S = S * step + S0 / 2;
            }
            while (Math.Abs(S - S0) > eps);

            return S;

        }
        public static double IntegrateSegments(Func<double, double> func, double start, double end, double eps = 0.001, int segments = 1)
        {
            double step = (end - start) / segments;
            double result = 0;
            for (double p = 0; p < segments; p++)
            {
                double local_start = start + p * step;
                double local_end = (segments == 1) ? end : (local_start + step);
                result += Integrate(func, local_start, local_end, eps);
            }
            return result;
        }

        public static double ParallelIntegrateSegmentsThreads(Func<double, double> func, double start, double end, double eps = 0.001, int threadCount = 1)
        {
            double step = (end - start) / threadCount;
            double result = 0;
            object lockObj = new object();
            List<Thread> threads = new List<Thread>();
            for (double p = 0; p < threadCount; p++)
            {
                double local_start = start + p * step;
                double local_end = (threadCount == 1) ? end : (local_start + step);
                Thread thread = new Thread(() =>
                {
                    double local_res = Integrate(func, local_start, local_end, eps);
                    lock (lockObj)
                    {
                        result += local_res;
                    }
                });
                threads.Add(thread);
                thread.Start();
            }
            foreach (var thread in threads)
            {
                thread.Join();
            }

            return result;
        }


        public static double ParallelIntegrateSegmentsTasks(Func<double, double> func, double start, double end, double eps = 0.001, int threadCount = 1)
        {
            double result = 0;
            object lockObj = new object();
            double step = (end - start) / threadCount;
            List<Task> tasks = new List<Task>();
            for (double p = 0; p < threadCount; p++)
            {
                double local_start = start + p * step;
                double local_end = (threadCount == 1) ? end : (local_start + step);
                Task task = Task.Run(() =>
                {
                    double local_res = Integrate(func, local_start, local_end, eps);
                    lock (lockObj)
                    {
                        result += local_res;
                    }
                });
                tasks.Add(task);
            }
            Task.WaitAll(tasks.ToArray());

            return result;
        }

        public static double ParallelIntegrateThreads(Func<double, double> func, double start, double end, double eps = 0.001, int threadCount = 1)
        {

            double step = (end - start) / 2;
            double S0 = 0, S = 0;
            for (double x = start; x < end; x += step)
            {
                S += SafeEvaluate(func, x);
            }
            S *= step;
            Thread[] threads = new Thread[threadCount];
            object lockObj = new object();
            do
            {
                S0 = S;
                step /= 2;
                S = 0;

                int intervals = (int)((end - start) / (step * 2));
                threads = new Thread[threadCount];
                double[] threadResults = new double[threadCount];

                for (int i = 0; i < threadCount; i++)
                {
                    int threadIndex = i;
                    threads[threadIndex] = new Thread(() =>
                    {
                        double threadSum = 0;
                        int iterationsPerThread = (intervals + threadCount - 1) / threadCount;
                        int startIdx = threadIndex * iterationsPerThread;
                        int endIdx = Math.Min(startIdx + iterationsPerThread, intervals);

                        for (int j = startIdx; j < endIdx; j++)
                        {
                            double x = start + step + j * step * 2;
                            if (x < end)
                            {
                                threadSum += SafeEvaluate(func, x);
                            }
                        }
                        threadResults[threadIndex] = threadSum;
                    });
                    threads[i].Start();
                }

                foreach (var thread in threads)
                {
                    thread.Join();
                }

                for (int i = 0; i < threadCount; i++)
                {
                    S += threadResults[i];
                }

                S = S * step + S0 / 2;
            }
            while (Math.Abs(S - S0) > eps);

            return S;
        }


        public static double ParallelIntegrateTasks(Func<double, double> func, double start, double end, double eps = 0.001, int numThreads = 1)
        {
            double step = (end - start) / 2;
            double S0 = 0, S = 0;
            bool success = false;
            for (double x = start; x < end; x += step)
            {
                S += SafeEvaluate(func, x);
            }
            S *= step;

            object lockObj = new object();
            do
            {
                S0 = S;
                step /= 2;
                S = 0;

                // Количество интервалов для обработки
                int intervals = (int)((end - start) / (step * 2));

                // Параллельная обработка
                Parallel.For(0, numThreads, i =>
                {
                    double threadSum = 0;
                    // Разделяем работы между потоками
                    int iterationsPerThread = (intervals + numThreads - 1) / numThreads; // Делим, округляя вверх
                    int startIdx = i * iterationsPerThread;
                    int endIdx = Math.Min(startIdx + iterationsPerThread, intervals);

                    for (int j = startIdx; j < endIdx; j++)
                    {
                        double x = start + step + j * step * 2;
                        if (x < end)
                        {
                            threadSum += SafeEvaluate(func, x);
                        }
                    }

                    // Суммируем результаты в общем контексте
                    lock (lockObj) // Используем блокировку для предотвращения гонки
                    {
                        S += threadSum;
                    }
                });

                S = S * step + S0 / 2;
            }
            while (Math.Abs(S - S0) > eps);

            return S;
        }
    }
}
