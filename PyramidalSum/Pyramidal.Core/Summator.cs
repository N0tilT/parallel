using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Pyramidal.Core
{
    public class Summator
    {
        public long ParallelSum(List<long> numbers, int threadCount)
        {
            int chunkSize = (numbers.Count + threadCount - 1) / threadCount; 
            var sums = new long[threadCount];
            Thread[] threads = new Thread[threadCount];

            for (int i = 0; i < threadCount; i++)
            {
                int index = i;
                threads[index] = new Thread(() =>
                {
                    var chunk = numbers.Skip(index * chunkSize).Take(chunkSize);
                    long sum = 0;
                    for (int j = 0; j < chunk.Count(); j++)
                    {
                        sum += chunk.ElementAt(j);
                    }
                    sums[index] = sum;
                });
                threads[index].Start();
            }

            foreach (var thread in threads)
            {
                thread.Join(); 
            }

            return sums.Sum();
        }

        public long ParallelSumTasks(List<long> numbers, int threadCount)
        {
            int chunkSize = (numbers.Count + threadCount - 1) / threadCount;
            var sums = new long[threadCount];
            Task[] threads = new Task[threadCount];

            for (int i = 0; i < threadCount; i++)
            {
                int index = i;
                threads[index] = new Task(() =>
                {
                    var chunk = numbers.Skip(index * chunkSize).Take(chunkSize);
                    long sum = 0;
                    for (int j = 0; j < chunk.Count(); j++)
                    {
                        sum += chunk.ElementAt(j);
                    }
                    sums[index] = sum;
                });
                threads[index].Start();
            }

            foreach (var thread in threads)
            {
                thread.Wait();
            }

            return sums.Sum();
        }

        public long SyncSum(List<long> numbers)
        {
            long sum = 0;
            for (int i = 0; i < numbers.Count; i++)
            {
                sum += numbers[i];
            }
            return sum;
        }

    }
}
