using FacebookIDE.Model;
using FacebookIDE.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
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

namespace FacebookIDE
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            ViewModel = new MainPageViewModel();
            DataContext = ViewModel;
        }

        public MainPageViewModel ViewModel { get; set; }




        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = this.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Move_Window(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void items_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            StoredRequest item = (sender as ListBox).SelectedItem as StoredRequest;
            ViewModel.AddRequest(item.Clone());
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            rootGrid.Margin = WindowState == WindowState.Maximized ? new Thickness(6) : new Thickness(0);
        }

        private void CloseTab_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.RemoveRequest((StoredRequest)(sender as Button).Tag);
        }
    }
}
