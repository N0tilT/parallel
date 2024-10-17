using System.Collections.Concurrent;

namespace Pyramidal.Core
{
    public class Summator
    {

        public long ParallelSumTasks(List<long> numbers, int threadCount)
        {
            if (numbers == null || numbers.Count == 0)
                return 0;

            int originalCount = numbers.Count;
            int remainder = originalCount % threadCount;
            if (remainder != 0)
            {
                int elementsToAdd = threadCount - remainder;
                numbers.AddRange(Enumerable.Repeat(0L, elementsToAdd));
            }

            long totalSum = 0;
            Task[] tasks = new Task[threadCount];
            ConcurrentBag<long> sums = new ConcurrentBag<long>();

            int chunkSize = numbers.Count / threadCount;

            for (int i = 0; i < threadCount; i++)
            {
                int start = i * chunkSize;
                int end = start + chunkSize;

                tasks[i] = Task.Run(() =>
                {
                    long sum = 0;
                    for (int j = start; j < end; j++)
                    {
                        sum += numbers[j];
                    }
                    sums.Add(sum);
                });
            }
            foreach (var task in tasks)
            {
                task.Wait();
            }
            totalSum = sums.Sum();

            return totalSum;
        }

        public long ParallelSum(List<long> numbers, int threadCount)
        {
            if (numbers == null || numbers.Count == 0)
                return 0;

            int originalCount = numbers.Count;
            int remainder = originalCount % threadCount;
            if (remainder != 0)
            {
                int elementsToAdd = threadCount - remainder;
                numbers.AddRange(Enumerable.Repeat(0L, elementsToAdd));
            }

            long totalSum = 0;
            var tasks = new List<Task<long>>();

            int chunkSize = numbers.Count / threadCount;
            ConcurrentBag<long> sums = new ConcurrentBag<long>();
            Thread[] threads = new Thread[threadCount];

            for (int i = 0; i < threadCount; i++)
            {
                int start = i * chunkSize;
                int end = start + chunkSize;
                int index = i;
                threads[index] = new Thread(() =>
                {

                    long sum = 0;
                    for (int j = start; j < end; j++)
                    {
                        sum += numbers[j];
                    }
                    sums.Add(sum);
                });
                threads[index].Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
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

        public long ParallelSumTasksRecursive(List<long> numbers, int threadCount)
        {
            if (numbers == null || numbers.Count == 0)
                return 0;

            int originalCount = numbers.Count;
            int remainder = originalCount % threadCount;
            if (remainder != 0)
            {
                int elementsToAdd = threadCount - remainder;
                numbers.AddRange(Enumerable.Repeat(0L, elementsToAdd));
            }

            long totalSum = 0;
            Task[] tasks = new Task[threadCount];
            ConcurrentBag<long> sums = new ConcurrentBag<long>();

            int chunkSize = numbers.Count / threadCount;

            for (int i = 0; i < threadCount; i++)
            {
                int start = i * chunkSize;
                int end = start + chunkSize;

                tasks[i] = Task.Run(() =>
                {
                    long sum = 0;
                    for (int j = start; j < end; j++)
                    {
                        sum += numbers[j];
                    }
                    sums.Add(sum);
                });
            }
            foreach (var task in tasks)
            {
                task.Wait();
            }

            return sums.Count() > 100 ? ParallelSumRecursive(sums.ToList(), threadCount) : sums.Sum();
        }

        public long ParallelSumRecursive(List<long> numbers, int threadCount)
        {
            if (numbers == null || numbers.Count == 0)
                return 0;

            int originalCount = numbers.Count;
            int remainder = originalCount % threadCount;
            if (remainder != 0)
            {
                int elementsToAdd = threadCount - remainder;
                numbers.AddRange(Enumerable.Repeat(0L, elementsToAdd));
            }

            long totalSum = 0;
            var tasks = new List<Task<long>>();

            int chunkSize = numbers.Count / threadCount;
            ConcurrentBag<long> sums = new ConcurrentBag<long>();
            Thread[] threads = new Thread[threadCount];

            for (int i = 0; i < threadCount; i++)
            {
                int start = i * chunkSize;
                int end = start + chunkSize;
                int index = i;
                threads[index] = new Thread(() =>
                {

                    long sum = 0;
                    for (int j = start; j < end; j++)
                    {
                        sum += numbers[j];
                    }
                    sums.Add(sum);
                });
                threads[index].Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }

            return sums.Count() > 100 ? ParallelSumRecursive(sums.ToList(), threadCount) : sums.Sum();
        }
    }
}
