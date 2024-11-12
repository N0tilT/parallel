using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyramidal.Core
{
    public static class Integrator
    {
        public static double Integrate(Func<double, double> func, double start, double end, double step = 0.01)
        {
            double result = 0;
            for (double x = start; x < end; x += step)
            {
                result += func(x) * step;
            }
            return result;
        }

        public static double ParallelIntegrateThreads(int threadCount,Func<double, double> func, double start, double end, double step = 0.01)
        {
            double range = (end-start)/threadCount;
            double result = 0;
            object lockObj = new object();
            List<Thread> threads = new List<Thread>();
            for (int i = 0; i < threadCount; i++)
            {
                double threadStart = start + i * range;
                double threadEnd = (i == threadCount - 1) ? end : threadStart + range;

                Thread thread = new Thread(() =>
                {
                    double partialResult = 0;
                    for (double x = threadStart; x < threadEnd; x += step)
                    {
                        partialResult += func(x) * step;
                    }
                    lock (lockObj)
                    {
                        result += partialResult;
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

        public static double ParallelIntegrateTasks(int taskCount,Func<double, double> func, double start, double end, double step = 0.01)
        {
            double range = (end - start) / taskCount;
            var tasks = new List<Task<double>>();

            for (int i = 0; i < taskCount; i++)
            {
                double taskStart = start + i * range;
                double taskEnd = (i == taskCount - 1) ? end : taskStart + range;

                Task<double> task = Task.Run(() =>
                {
                    double partialResult = 0;
                    for (double x = taskStart; x < taskEnd; x += step)
                    {
                        partialResult += func(x) * step;
                    }
                    return partialResult;
                });

                tasks.Add(task);
            }

            Task.WaitAll(tasks.ToArray());

            double result = 0;
            foreach (var task in tasks)
            {
                result += task.Result;
            }

            return result;
        }
    }
}
