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
            (x) => {return (double)x/Math.Sqrt(x*x-16); },
            (x) => {return Math.Pow(x,4)*Math.Log(x)*Math.Log(x); },
            (x) => {return 1/(3*Math.Cos(x)*Math.Cos(x)+2*Math.Sin(x)*Math.Sin(x)); },
            (x) => {return Math.Sqrt(x)/(Math.Pow(x,3/4)+1); },
            (x) => {return (x * x*x - 2*x*x-12*x-7)/(x*x*x-x*x); }
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
        private void comboBoxFunction_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedFunctionIndex = comboBoxFunction.SelectedIndex;
            if(selectedFunctionIndex == 1)
            {
                textBoxStartValue.Text = "0";
                textBoxMaxValue.Text = "2";
            }
            else
            {
                textBoxStartValue.Text = "5";
                textBoxMaxValue.Text = "10";
            }
        }

        private void btnRun_Click(object sender, RoutedEventArgs e)
        {
            int selectedThreadCount = int.Parse((comboBoxThreadCount.SelectedItem as ComboBoxItem).Content.ToString());
            int selectedFunctionIndex = comboBoxFunction.SelectedIndex;

            int startValue = int.Parse(textBoxStartValue.Text);
            int maxValue = int.Parse(textBoxMaxValue.Text);

            PerformBenchmark(selectedThreadCount, selectedFunctionIndex, startValue, maxValue);

        }

        private void PerformBenchmark(int selectedThreadCount, int selectedFunctionIndex, int startValue, int maxValue)
        {
            AxisXLabels.Clear();
            Series.Clear();

            var parallelTimes = new ChartValues<double>();
            var parallelTasksTimes = new ChartValues<double>();
            var syncTimes = new ChartValues<double>();

            List<double> accuracy_list = new List<double>() { 0.1,0.01, 0.001, 0.0001,0.00001,0.000001,0.0000001 };

            foreach (var accuracy in accuracy_list)
            {
                Random random = new Random();

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                double syncSumResult = Integrator.Integrate(functions[selectedFunctionIndex], startValue, maxValue,accuracy);
                stopwatch.Stop();
                double syncTime = stopwatch.Elapsed.TotalMilliseconds;
                syncTimes.Add(syncTime);

                stopwatch.Restart();
                double threadResult = Integrator.ParallelIntegrateSegmentsTasks(functions[selectedFunctionIndex], startValue, maxValue, accuracy, selectedThreadCount);
                stopwatch.Stop();
                double parallelSumTime = stopwatch.Elapsed.TotalMilliseconds;
                parallelTimes.Add(parallelSumTime);

                stopwatch.Restart();
                double taskResult = Integrator.ParallelIntegrateTasks(functions[selectedFunctionIndex], startValue, maxValue, accuracy, selectedThreadCount);
                stopwatch.Stop();
                double parallelTasksTime = stopwatch.Elapsed.TotalMilliseconds;
                parallelTasksTimes.Add(parallelTasksTime);

                AxisXLabels.Add($"Точность {accuracy}");
            }

            Series.Add(new LineSeries
            {
                Title = "Синхронное интегрирование",
                Values = syncTimes,
                PointGeometry = null
            });

            Series.Add(new LineSeries
            {
                Title = $"Параллельное интегрирование сегментами ({selectedThreadCount} потоков)",
                Values = parallelTimes,
                PointGeometry = null
            });

            Series.Add(new LineSeries
            {
                Title = $"Интегрирование с параллельным суммированием ({selectedThreadCount} потоков)",
                Values = parallelTasksTimes,
                PointGeometry = null
            });

            chart.AxisX[0].Labels = AxisXLabels;

        }

        
    }
}
