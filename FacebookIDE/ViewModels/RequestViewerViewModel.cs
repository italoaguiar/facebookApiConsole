using FacebookIDE.Model;
using LiveCharts;
using LiveCharts.Wpf;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;

namespace FacebookIDE.ViewModels
{
    public class RequestViewerViewModel : INotifyPropertyChanged
    {
        private APIHttpRequest httpRequest;
        private string response;
        private IEnumerable<APIParameter> headerResponse;
        private SeriesCollection chartColumnData;
        private SeriesCollection chartPieData;
        private string[] chartLabels;
        private HttpResponseMessage responseMessage;
        private bool isFormUrlContent;
        private bool isRawContent;
        private bool isEmptyContent = true;

        public Func<double, string> Formatter { get; set; } = value => value.ToString("N");

        public APIHttpRequest HttpRequest 
        { 
            get => httpRequest;
            set
            {
                httpRequest = value;
                OnPropertyChanged("HttpRequest");
            }
        }

        public string Response 
        { 
            get => response;
            set
            {
                response = value;
                OnPropertyChanged("Response");
                OnUpdateTextViewRequested(value);
            }
        }

        public IEnumerable<APIParameter> HeaderResponse
        { 
            get => headerResponse;
            set
            {
                headerResponse = value;
                OnPropertyChanged("HeaderResponse");
            }
        }

        public SeriesCollection ChartColumnData 
        { 
            get => chartColumnData;
            set
            {
                chartColumnData = value;
                OnPropertyChanged("ChartColumnData");
            }
        }

        public SeriesCollection ChartPieData
        {
            get => chartPieData;
            set
            {
                chartPieData = value;
                OnPropertyChanged("ChartPieData");
            }
        }

        public string[] ChartLabels
        {
            get => chartLabels;
            set
            {
                chartLabels = value;
                OnPropertyChanged("ChartLabels");
            }
        }

        public HttpResponseMessage ResponseMessage 
        { 
            get => responseMessage;
            set
            {
                responseMessage = value;
                OnPropertyChanged("ResponseMessage");
            }
        }

        public bool IsFormUrlContent 
        { 
            get => isFormUrlContent;
            set
            {
                isFormUrlContent = value;
                if(value)
                    HttpRequest.BodyType = BodyType.XWWWFormUrlEncoded;
            }
        }
        public bool IsRawContent 
        { 
            get => isRawContent;
            set
            {
                isRawContent = value;
                if(value)
                    HttpRequest.BodyType = BodyType.Raw;
            }
        }

        public bool IsEmptyContent 
        { 
            get => isEmptyContent;
            set
            {
                isEmptyContent = value;
                if(value)
                    HttpRequest.BodyType = BodyType.None;
            }
        }

        public async void SendRequestAsync()
        {
            if (HttpRequest != null)
            {
                try
                {
                    var req = await HttpRequest.SendRequestAsync();

                    ResponseMessage = req;

                    var mime = new string[] { "text/javascript", "text/json", "application/json", "application/javascript" };

                    if (mime.Contains(req.Content.Headers.ContentType.MediaType.ToLower()))
                    {
                        Response = IndentJson(await req.Content.ReadAsStringAsync());
                    }
                    else
                    {
                        Response = await req.Content.ReadAsStringAsync();
                    }

                    HeaderResponse = req.Headers.Select(x => new APIParameter()
                    {
                        Name = x.Key,
                        Value = string.Join(",", x.Value)
                    });
                }
                catch (HttpException http)
                {
                    MessageBox.Show("Error:" + http.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch(Exception e)
                {
                    MessageBox.Show($"Failed to submit informed request: {e.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
        }

        public void GenerateChart()
        {
            try
            {
                JObject o = JObject.Parse(Response);
                IEnumerable<JToken> t = o.SelectTokens(HttpRequest.Chart.DataPath);



                SeriesCollection chartColData = new SeriesCollection();
                SeriesCollection chartPieData = new SeriesCollection();
                List<string> labels = new List<string>();


                foreach (var item in t)
                {
                    string title = item.SelectToken(HttpRequest.Chart.LabelField).ToObject<string>();
                    IChartValues value = new ChartValues<double> { item.SelectToken(HttpRequest.Chart.SerieField).ToObject<double>() };

                    labels.Add(title);
                    
                    chartColData.Add(new ColumnSeries()
                    {
                        Title = title,
                        Values = value
                    });

                    chartPieData.Add(new PieSeries()
                    {
                        Title = title,
                        Values = value                        
                    });
                }

                ChartColumnData = chartColData;
                ChartPieData = chartPieData;
                ChartLabels = labels.ToArray();
            }
            catch
            {
                MessageBox.Show("Unable to generate chart. Check the parameters entered and the server response is in JSON format.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string IndentJson(string c)
        {
            try
            {
                dynamic r = JsonConvert.DeserializeObject<dynamic>(c);
                return JsonConvert.SerializeObject(r, Formatting.Indented);
            }
            catch
            {
                return c;
            }
        }

        


        public delegate void UpdateTextView(string text);
        public event UpdateTextView UpdateTextViewRequested;

        private void OnUpdateTextViewRequested(string text)
        {
            UpdateTextViewRequested?.Invoke(text);
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
