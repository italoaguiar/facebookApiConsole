using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacebookIDE.Model
{
    public class APIParameter : INotifyPropertyChanged
    {
        private string paramName;
        private string value;
        private bool isEnabled = true;

        public string Name 
        { 
            get => paramName;
            set
            {
                paramName = value;
                OnPropertyChanged("Name");
            }
        }

        public string Value 
        { 
            get => value;
            set
            {
                this.value = value;
                OnPropertyChanged("Value");
            }
        }

        public bool IsEnabled 
        { 
            get => isEnabled;
            set
            {
                isEnabled = value;
                OnPropertyChanged("IsEnabled");
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
