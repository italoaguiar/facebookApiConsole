using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace FacebookIDE
{
    /// <summary>
    /// Lógica interna para Splash.xaml
    /// </summary>
    public partial class Splash : Window
    {
        public Splash()
        {
            InitializeComponent();

            this.Loaded += Splash_Loaded;
        }

        private async void Splash_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Delay(6000);

            MainWindow mw = new MainWindow();
            mw.InitializeComponent();
            mw.Show();

            this.Close();
        }
    }
}
