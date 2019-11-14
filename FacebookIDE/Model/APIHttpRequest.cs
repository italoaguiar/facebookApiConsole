using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.ComponentModel;
using System.Web;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace FacebookIDE.Model
{
    public class APIHttpRequest: INotifyPropertyChanged
    {
        public APIHttpRequest()
        {
            QueryParameters = new ParameterCollection();
            QueryParameters.ItemChanged += (s, a) => UpdateUri();

            HeaderFields.Add(new APIParameter() { IsEnabled = false });
            BodyFields.Add(new APIParameter() { IsEnabled = false });
        }

        private ParameterCollection queryParameters;
        private Uri requestUri;
        private HttpMethod method = HttpMethod.Get;
        private string bodyContent;
        private string contentType = "text/plain";
        private BodyType bodyType = BodyType.None;

        public ObservableCollection<APIParameter> HeaderFields { get; set; } = new ObservableCollection<APIParameter>();
        public ObservableCollection<APIParameter> BodyFields { get; set; } = new ObservableCollection<APIParameter>();


        public ChartPath Chart { get; set; }

        [JsonIgnore]
        public ParameterCollection QueryParameters
        {
            get => queryParameters;
            set
            {
                queryParameters = value;
                OnPropertyChanged("QueryParameters");
            }
        }

        public Uri RequestUri
        {
            get => requestUri;
            set
            {
                requestUri = value;
                OnPropertyChanged("RequestUri");
                UpdateQueryParams(value);
            }
        }

        public HttpMethod Method 
        { 
            get => method;
            set
            {
                method = value;
                OnPropertyChanged("Method");
            }
        }

        public string BodyContent 
        { 
            get => bodyContent;
            set
            {
                bodyContent = value;
                OnPropertyChanged("BodyContent");
            }
        }

        public string ContentType 
        { 
            get => contentType;
            set
            {
                contentType = value;
                OnPropertyChanged("ContentType");
            }
        }

        public BodyType BodyType 
        { 
            get => bodyType;
            set
            {
                bodyType = value;
                OnPropertyChanged("BodyType");
            }
        }

        public async Task<HttpResponseMessage> SendRequestAsync()
        {
            using (HttpClient http = new HttpClient())
            {
                return await http.SendAsync(GetHttpMessage());
            }
        }

        private void UpdateQueryParams(Uri uri)
        {
            QueryParameters.Clear();

            try
            {
                var q = HttpUtility.ParseQueryString(uri.Query);

                foreach (var item in q.AllKeys)
                {
                    var param = item;
                    var value = q[item];

                    QueryParameters.Add(new APIParameter() { Name = param, Value = value });
                }
            }
            catch { }
        }

        public void AddQueryParameter(string name, string value)
        {
            QueryParameters.Add(new APIParameter() { Name = name, Value = value });
            UpdateUri();
        }

        private void UpdateUri()
        {
            try
            {
                string query = "?";
                foreach (var item in QueryParameters.Where(x => x.IsEnabled))
                {
                    query += $"{item.Name}={item.Value}&";
                }
                query = query.Substring(0, query.Length - 1);

                string u = RequestUri.OriginalString;
                u = u.Substring(0, u.IndexOf("?"));
                u += query;
                RequestUri = new Uri(u);
            }
            catch { }
        }


        private HttpRequestMessage GetHttpMessage()
        {
            var req = new HttpRequestMessage()
            {
                Method = this.Method,
                RequestUri = this.RequestUri
            };
            foreach(var h in HeaderFields.Where(x=> x.IsEnabled))
            {
                req.Headers.Add(h.Name, h.Value);
            }
            switch (this.BodyType)
            {
                case BodyType.Raw:
                    req.Content = new StringContent(BodyContent, Encoding.UTF8, ContentType);
                    break;
                case BodyType.XWWWFormUrlEncoded:
                    var b = BodyFields.Select(x => new KeyValuePair<string, string>(x.Name, x.Value));
                    req.Content = new FormUrlEncodedContent(b);
                    break;
            }

            return req;
        }

        public APIHttpRequest Clone()
        {
            APIHttpRequest c = new APIHttpRequest();
            c.RequestUri = new Uri(RequestUri.OriginalString);
            c.Method = Method;
            c.ContentType = (string)ContentType.Clone();
            c.BodyType = this.BodyType;
            c.Chart = new ChartPath() { DataPath = Chart.DataPath, LabelField = Chart.LabelField, SerieField = Chart.SerieField };
            c.QueryParameters = new ParameterCollection(QueryParameters);
            c.HeaderFields = new ObservableCollection<APIParameter>(HeaderFields);
            c.BodyFields = new ObservableCollection<APIParameter>(BodyFields);
            return c;
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class ChartPath:INotifyPropertyChanged
    {
        private string dataField;
        private string serieField;
        private string labelField;

        public string DataPath 
        { 
            get => dataField;
            set
            {
                dataField = value;
                OnPropertyChanged("DataPath");
            }
        }

        public string SerieField 
        { 
            get => serieField;
            set
            {
                serieField = value;
                OnPropertyChanged("SerieField");
            }
        }

        public string LabelField 
        { 
            get => labelField;
            set
            {
                labelField = value;
                OnPropertyChanged("LabelField");
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public enum BodyType
    {
        None,
        XWWWFormUrlEncoded,
        Raw
    }


}
