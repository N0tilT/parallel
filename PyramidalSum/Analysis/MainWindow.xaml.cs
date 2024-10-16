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
            AxisXLabels.Clear();
            Series.Clear();

            var parallelTimes = new List<ChartValues<double>>() { new(),new(),new(), new() };
            var parallelTasksTimes = new List<ChartValues<double>>() { new(), new(), new(), new() };
            var syncTimes = new ChartValues<double>();
            Summator summator = new Summator();

            for(int i = 4;i<=9;i++)
            {
                Random random = new Random();
                List<long> numbers = Enumerable.Range(0, (int)Math.Pow(10,i)).Select(i => (long)random.Next(0, 100)).ToList();

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                long syncSumResult = summator.SyncSum(numbers);
                stopwatch.Stop();
                double syncTime = stopwatch.Elapsed.TotalMilliseconds;
                syncTimes.Add(syncTime);

                for (int threadCount = 5; threadCount <= 8; threadCount++)
                {
                    stopwatch.Restart();
                    long parallelSumResult = summator.ParallelSum(numbers, threadCount);
                    stopwatch.Stop();
                    double parallelTime = stopwatch.Elapsed.TotalMilliseconds;
                    parallelTimes[threadCount-5].Add(parallelTime);
                }

                for (int threadCount = 5; threadCount <= 8; threadCount++)
                {
                    stopwatch.Restart();
                    long parallelSumResult = summator.ParallelSumTasks(numbers, threadCount);
                    stopwatch.Stop();
                    double parallelTime = stopwatch.Elapsed.TotalMilliseconds;
                    parallelTasksTimes[threadCount - 5].Add(parallelTime);
                }

                // Добавляем метку для оси X
                AxisXLabels.Add($"Size: {(int)Math.Pow(10,i)}");
            }

            // Добавляем линии для параллельного и синхронного суммирования
            Series.Add(new LineSeries
            {
                Title = "Синхронная сумма",
                Values = syncTimes,
                PointGeometry = null // Можно скрыть точки, если не нужно
            });

            for (int i = 0; i <= 3; i++)
            {
                Series.Add(new LineSeries
                {
                    Title = $"Параллельная сумма ({i+5} потоков)",
                    Values = new ChartValues<double>(parallelTimes[i]), // Создаем новый ChartValues<double>
                    PointGeometry = null
                });
                Series.Add(new LineSeries
                {
                    Title = $"Параллельная сумма ({i + 5} потоков)",
                    Values = new ChartValues<double>(parallelTasksTimes[i]), // Создаем новый ChartValues<double>
                    PointGeometry = null
                });
            }

        }


    }
}
