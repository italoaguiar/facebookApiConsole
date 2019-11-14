using FacebookIDE.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacebookIDE.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public MainPageViewModel()
        {
            Requests = new ObservableCollection<StoredRequest>();
            LoadRequestsFromJson();
        }

        private List<StoredRequest> storedRequests;
        private List<StoredRequest> sidebarRequests;

        private void LoadRequestsFromJson()
        {
            using(StreamReader sr = new StreamReader("api.json"))
            {
                var json = sr.ReadToEnd();
                storedRequests = Newtonsoft.Json.JsonConvert.DeserializeObject<List<StoredRequest>>(json);
                Filter();
            }
        }

        public void Filter(string text = null)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                SidebarRequests = storedRequests;
            }
            else
            {
                SidebarRequests = storedRequests.Where(x => x.Name.Contains(text) || x.Description.Contains(text)).ToList();
            }
        }


        int selectedIndex;
        ObservableCollection<StoredRequest> requests;

        public int SelectedIndex 
        { 
            get => selectedIndex;
            set
            {
                selectedIndex = value;
                OnPropertyChanged("SelectedIndex");
            }
        }

        public ObservableCollection<StoredRequest> Requests 
        { 
            get => requests;
            set
            {
                requests = value;
                OnPropertyChanged("Requests");
            }
        }

        public List<StoredRequest> SidebarRequests 
        { 
            get => sidebarRequests;
            set
            {
                sidebarRequests = value;
                OnPropertyChanged("SidebarRequests");
            }
        }

        public void RemoveRequest(StoredRequest r)
        {
            Requests.Remove(r);
        }

        public void AddRequest(StoredRequest r)
        {
            Requests.Add(r);
            SelectedIndex = Requests.Count - 1;
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
