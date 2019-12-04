
using LiveCharts;
using LiveCharts.Wpf;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Facebook
{

    public abstract class BaseRequest
    {
        public string ToJson(object item)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(item);
        }

        public string Error { get; set; }

        public SeriesCollection chartColData = new SeriesCollection();
        HttpClient client = new HttpClient();

        public double RequestSerieValue(HttpRequestMessage msg, string datapath)
        {
            var r = client.SendAsync(msg).Result;
            string c = r.Content.ReadAsStringAsync().Result;

            try
            {
                JObject o = JObject.Parse(c);
                return o.SelectToken(datapath).ToObject<double>();
            }
            catch
            {
                Error = c;
                throw new Exception(c);
            }
        }

        public void AddSerie(double value, string serieName)
        {
            chartColData.Add(new ColumnSeries()
            {
                Title = serieName,
                Values = new ChartValues<double> { value}
            });
        }

        public void ClearSeries()
        {
            chartColData.Clear();
        }

        public SeriesCollection GetCollection()
        {
            return chartColData;
        }

        public abstract void GetSeries();
    }
}
