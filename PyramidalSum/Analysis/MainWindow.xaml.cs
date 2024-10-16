using LiveCharts.Wpf;
using LiveCharts;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Pyramidal.Core;

namespace Analysis
{
    public partial class MainWindow : Window
    {
        public SeriesCollection Series { get; set; }
        public List<string> AxisXLabels { get; set; } = new List<string>();

        public MainWindow()
        {
            InitializeComponent();
            Series = new SeriesCollection();
            chart.Series = Series;
        }

        private void btnRun_Click(object sender, RoutedEventArgs e)
        {
            PerformBenchmark();
        }

        private void PerformBenchmark()
        {
            List<int> sizes = Enumerable.Range(1, Environment.ProcessorCount).Select(i => i * 100000).ToList();
            sizes.Add(int.MaxValue - 100);
            AxisXLabels.Clear();
            Series.Clear();

            var parallelTimes = new ChartValues<double>();
            var syncTimes = new ChartValues<double>();
            Summator summator = new Summator();

            foreach (var size in sizes)
            {
                Random random = new Random();
                List<long> numbers = Enumerable.Range(0, size).Select(i => (long)random.Next(0, 1000)).ToList();

                // Измеряем время выполнения SyncSum
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                long syncSumResult = summator.SyncSum(numbers);
                stopwatch.Stop();
                double syncTime = stopwatch.Elapsed.TotalMilliseconds;
                syncTimes.Add(syncTime);

                // Измеряем время выполнения ParallelSum с разным количеством потоков
                for (int threadCount = 1; threadCount <= Environment.ProcessorCount; threadCount++)
                {
                    stopwatch.Restart();
                    long parallelSumResult = summator.ParallelSum(numbers, threadCount);
                    stopwatch.Stop();
                    double parallelTime = stopwatch.Elapsed.TotalMilliseconds;
                    parallelTimes.Add(parallelTime);
                }

                // Добавляем метку для оси X
                AxisXLabels.Add($"Size: {size.ToString()}");
            }

            // Добавляем линии для параллельного и синхронного суммирования
            Series.Add(new LineSeries
            {
                Title = "Синхронная сумма",
                Values = syncTimes,
                PointGeometry = null // Можно скрыть точки, если не нужно
            });

            for (int i = 1; i <= Environment.ProcessorCount; i++)
            {
                var valuesForThreadCount = parallelTimes.Where((v, index) => index % Environment.ProcessorCount == i - 1).ToList();

                Series.Add(new LineSeries
                {
                    Title = $"Параллельная сумма ({i} потоков)",
                    Values = new ChartValues<double>(valuesForThreadCount), // Создаем новый ChartValues<double>
                    PointGeometry = null
                });
            }

        }


    }
}
