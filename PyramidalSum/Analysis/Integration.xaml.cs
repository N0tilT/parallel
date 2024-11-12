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
    /// Логика взаимодействия для Integration.xaml
    /// </summary>
    public partial class Integration : Page
    {
        public SeriesCollection Series { get; set; }
        public List<string> AxisXLabels { get; set; } = new List<string>();

        List<Func<double, double>> functions = new List<Func<double, double>> {
            (x) => x * x,
            (x) => x * x,
            (x) => x * x,
            (x) => x * x,
            (x) => x * x
        };


        public Integration()
        {
            InitializeComponent();
            Series = new SeriesCollection();
            chart.Series = Series;

            for (int i = 1; i <= Environment.ProcessorCount; i++)
            {
                comboBoxThreadCount.Items.Add(new ComboBoxItem { Content = i });
            }

            comboBoxThreadCount.SelectedIndex = 0;

            comboBoxFunction.Items.Add(new ComboBoxItem { Content = "A" });
            comboBoxFunction.Items.Add(new ComboBoxItem { Content = "B" });
            comboBoxFunction.Items.Add(new ComboBoxItem { Content = "C" });
            comboBoxFunction.Items.Add(new ComboBoxItem { Content = "D" });
            comboBoxFunction.Items.Add(new ComboBoxItem { Content = "E" });

            comboBoxFunction.SelectedIndex = 0;
        }

        private void btnRun_Click(object sender, RoutedEventArgs e)
        {
            int selectedThreadCount = int.Parse((comboBoxThreadCount.SelectedItem as ComboBoxItem).Content.ToString());
            int selectedFunctionIndex = comboBoxFunction.SelectedIndex;

            int startValue = int.Parse(textBoxStartValue.Text);
            int maxValue = int.Parse(textBoxMaxValue.Text);

            PerformBenchmark(selectedThreadCount,selectedFunctionIndex, startValue, maxValue);

        }

        private void PerformBenchmark(int selectedThreadCount,int selectedFunctionIndex, int startValue, int maxValue)
        {
            AxisXLabels.Clear();
            Series.Clear();

            var parallelTimes = new ChartValues<double>();
            var parallelTasksTimes = new ChartValues<double>();
            var syncTimes = new ChartValues<double>();



            for (int i = 10; i <= 200; i += 10)
            {
                Random random = new Random();

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                double syncSumResult = Integrator.Integrate(functions[selectedFunctionIndex], startValue, maxValue, (double)Math.Abs(maxValue - startValue) / i);
                stopwatch.Stop();
                double syncTime = stopwatch.Elapsed.TotalMilliseconds;
                syncTimes.Add(syncTime);

                stopwatch.Restart();
                double threadResult = Integrator.ParallelIntegrateThreads(selectedThreadCount, functions[selectedFunctionIndex], startValue, maxValue, (double)Math.Abs(maxValue - startValue) / i);
                stopwatch.Stop();
                double parallelSumTime = stopwatch.Elapsed.TotalMilliseconds;
                parallelTimes.Add(parallelSumTime);

                stopwatch.Restart();
                double taskResult = Integrator.ParallelIntegrateThreads(selectedThreadCount, functions[selectedFunctionIndex], startValue, maxValue, (double)Math.Abs(maxValue - startValue) / i);
                stopwatch.Stop();
                double parallelTasksTime = stopwatch.Elapsed.TotalMilliseconds;
                parallelTasksTimes.Add(parallelTasksTime);

                AxisXLabels.Add($"кол-во отрезков: {i}");
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


        }
    }
}
