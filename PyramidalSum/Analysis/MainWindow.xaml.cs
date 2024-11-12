using LiveCharts;
using LiveCharts.Wpf;
using Pyramidal.Core;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Analysis
{
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            
        }


        private void IntegratePage_Click(object sender, RoutedEventArgs e)
        {
            Integration integration = new Integration();
            Main.Content = integration;
        }

        private void SummPage_Click(object sender, RoutedEventArgs e)
        {
            PyramidalSumm summ = new PyramidalSumm();
            Main.Content = summ;
        }
    }
}
