using LiveCharts;
using LiveCharts.Wpf;
using Pyramidal.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Analysis
{
    /// <summary>
    /// Логика взаимодействия для PyramidalSumm.xaml
    /// </summary>
    public partial class PyramidalSumm : Page
    {
        public SeriesCollection Series { get; set; }
        public List<string> AxisXLabels { get; set; } = new List<string>();
        public PyramidalSumm()
        {
            InitializeComponent();
            Series = new SeriesCollection();
            chart.Series = Series;

            for (int i = 1; i <= Environment.ProcessorCount; i++)
            {
                comboBoxThreadCount.Items.Add(new ComboBoxItem { Content = i });
            }

            comboBoxThreadCount.SelectedIndex = 0;
        }

        private void btnRun_Click(object sender, RoutedEventArgs e)
        {
            int selectedThreadCount = int.Parse((comboBoxThreadCount.SelectedItem as ComboBoxItem).Content.ToString());

            int startValue = int.Parse(textBoxStartValue.Text);
            int step = int.Parse(textBoxStep.Text);
            int maxValue = int.Parse(textBoxMaxValue.Text);

            PerformBenchmark(selectedThreadCount, startValue, step, maxValue);

        }

        private void PerformBenchmark(int selectedThreadCount, int startValue, int step, int maxValue)
        {
            AxisXLabels.Clear();
            Series.Clear();

            var parallelTimes = new ChartValues<double>();
            var parallelTasksTimes = new ChartValues<double>();
            var parallelTimesRecursive = new ChartValues<double>();
            var parallelTasksTimesRecursive = new ChartValues<double>();
            var syncTimes = new ChartValues<double>();
            Summator summator = new Summator();

            for (int i = startValue; i <= maxValue; i += step)
            {
                Random random = new Random();
                List<long> numbers = Enumerable.Range(0, i).Select(i => (long)random.Next(0, 100)).ToList();

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                long syncSumResult = summator.SyncSum(numbers);
                stopwatch.Stop();
                double syncTime = stopwatch.Elapsed.TotalMilliseconds;
                syncTimes.Add(syncTime);

                stopwatch.Restart();
                long parallelSumResult = summator.ParallelSum(numbers, selectedThreadCount);
                stopwatch.Stop();
                double parallelSumTime = stopwatch.Elapsed.TotalMilliseconds;
                parallelTimes.Add(parallelSumTime);

                stopwatch.Restart();
                long parallelTasksSumResult = summator.ParallelSumTasks(numbers, selectedThreadCount);
                stopwatch.Stop();
                double parallelTasksTime = stopwatch.Elapsed.TotalMilliseconds;
                parallelTasksTimes.Add(parallelTasksTime);

                stopwatch.Restart();
                long parallelSumResultRecursive = summator.ParallelSumRecursive(numbers, selectedThreadCount);
                stopwatch.Stop();
                double parallelSumTimeRecursive = stopwatch.Elapsed.TotalMilliseconds;
                parallelTimesRecursive.Add(parallelSumTimeRecursive);

                stopwatch.Restart();
                long parallelTasksSumResultRecursive = summator.ParallelSumTasksRecursive(numbers, selectedThreadCount);
                stopwatch.Stop();
                double parallelTasksTimeRecursive = stopwatch.Elapsed.TotalMilliseconds;
                parallelTasksTimesRecursive.Add(parallelTasksTimeRecursive);

                AxisXLabels.Add($"Size: {(int)Math.Pow(10, i)}");
            }

            Series.Add(new LineSeries
            {
                Title = "Синхронная сумма",
                Values = syncTimes,
                PointGeometry = null
            });

            Series.Add(new LineSeries
            {
                Title = $"Параллельная сумма ({selectedThreadCount} потоков)",
                Values = parallelTimes,
                PointGeometry = null
            });

            Series.Add(new LineSeries
            {
                Title = $"Параллельная сумма задачами ({selectedThreadCount} потоков)",
                Values = parallelTasksTimes,
                PointGeometry = null
            });

            Series.Add(new LineSeries
            {
                Title = $"Параллельная сумма c рекурсией ({selectedThreadCount} потоков)",
                Values = parallelTimesRecursive,
                PointGeometry = null
            });

            Series.Add(new LineSeries
            {
                Title = $"Параллельная сумма задачами c рекурсией ({selectedThreadCount} потоков)",
                Values = parallelTasksTimesRecursive,
                PointGeometry = null
            });

        }
    }
}
