using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyramidal.Core
{
    public class Summator
    {
        public long ParallelSum(List<long> numbers, int threadCount)
        {
            int chunkSize = (numbers.Count + threadCount - 1) / threadCount; // Размеры чанков
            var sums = new long[threadCount];
            Thread[] threads = new Thread[threadCount];

            for (int i = 0; i < threadCount; i++)
            {
                int index = i; // используем локальную переменную для замыкания
                threads[index] = new Thread(() =>
                {
                    var chunk = numbers.Skip(index * chunkSize).Take(chunkSize).ToList();
                    sums[index] = SyncSum(chunk);
                });
                threads[index].Start();
            }

            foreach (var thread in threads)
            {
                thread.Join(); // ждем завершения потоков
            }

            return SyncSum(sums.ToList()); // собираем итоговую сумму
        }

        public long SyncSum(List<long> numbers)
        {
            long sum = 0;
            foreach (var item in numbers)
            {
                sum += item;
            }
            return sum;
        }

    }
}
