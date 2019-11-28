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
    /// Lógica interna para Permissions.xaml
    /// </summary>
    public partial class Permissions : Window
    {
        public Permissions()
        {
            InitializeComponent();
        }

        public string Scopes { get; private set; }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string r = "";
            foreach(UIElement item in mainGrid.Children)
            {
                if(item is CheckBox)
                {
                    CheckBox cb = item as CheckBox;
                    if(cb.IsChecked == true)
                    {
                        r += $"{cb.Content.ToString().Replace("__", "_")},";
                    }
                }
            }
            if(r.Length > 0)
            {
                r = r.Substring(0, r.Length - 1);
            }
            Scopes = r;

            DialogResult = true;
        }
    }
}
