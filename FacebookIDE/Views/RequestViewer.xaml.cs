using FacebookIDE.Model;
using FacebookIDE.ViewModels;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Indentation.CSharp;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FacebookIDE.Views
{
    /// <summary>
    /// Interação lógica para RequestViewer.xam
    /// </summary>
    public partial class RequestViewer : UserControl
    {
        public RequestViewer()
        {
            InitializeComponent();

            ViewModel = new RequestViewerViewModel();
            ViewModel.UpdateTextViewRequested += UpdateTextView;


            appId.Text = Properties.Settings.Default.appId;


            ConfigureEditor();
        }

        public RequestViewerViewModel ViewModel { get; set; }


        public static readonly DependencyProperty APIRequestProperty =
         DependencyProperty.Register("APIRequest", typeof(APIHttpRequest), typeof(RequestViewer), new
            PropertyMetadata(new PropertyChangedCallback(OnRequestChanged)));        

        public APIHttpRequest APIRequest
        {
            get { return (APIHttpRequest)GetValue(APIRequestProperty); }
            set 
            { 
                SetValue(APIRequestProperty, value); 
            }
        }

        private static void OnRequestChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RequestViewer requestViewer = d as RequestViewer;
            requestViewer.ViewModel.HttpRequest = (APIHttpRequest)e.NewValue;
        }



        public static readonly DependencyProperty QueryFieldsProperty =
        DependencyProperty.Register("QueryFields", typeof(IEnumerable<string>), typeof(RequestViewer), new
           PropertyMetadata(null));

        public IEnumerable<string> QueryFields
        {
            get { return (IEnumerable<string>)GetValue(QueryFieldsProperty); }
            set
            {
                SetValue(QueryFieldsProperty, value);
            }
        }


        public static readonly DependencyProperty HeaderFieldsProperty =
        DependencyProperty.Register("HeaderFields", typeof(IEnumerable<string>), typeof(RequestViewer), new
           PropertyMetadata(null));

        public IEnumerable<string> HeaderFields
        {
            get { return (IEnumerable<string>)GetValue(HeaderFieldsProperty); }
            set
            {
                SetValue(HeaderFieldsProperty, value);
            }
        }


        public static readonly DependencyProperty BodyFieldsProperty =
        DependencyProperty.Register("BodyFields", typeof(IEnumerable<string>), typeof(RequestViewer), new
           PropertyMetadata(null));

        public IEnumerable<string> BodyFields
        {
            get { return (IEnumerable<string>)GetValue(BodyFieldsProperty); }
            set
            {
                SetValue(BodyFieldsProperty, value);
            }
        }

        public static readonly DependencyProperty HelpUriProperty =
        DependencyProperty.Register("HelpUri", typeof(Uri), typeof(RequestViewer), new
           PropertyMetadata(null));

        public Uri HelpUri
        {
            get { return (Uri)GetValue(HelpUriProperty); }
            set
            {
                SetValue(HelpUriProperty, value);
            }
        }


        #region Fields

        BraceFoldingStrategy foldingStrategy;
        FoldingManager foldingManager;

        #endregion



        private void ConfigureEditor()
        {
            foldingManager = FoldingManager.Install(avalon_txt.TextArea);
            foldingStrategy = new BraceFoldingStrategy();
            UpdateTextView(avalon_txt.Document.Text);

            avalon_txt.Options.EnableHyperlinks = true;
            avalon_txt.TextArea.IndentationStrategy = new CSharpIndentationStrategy();
        }

        private void UpdateTextView(string value)
        {
            avalon_txt.Document.Text = value;
            foldingStrategy.UpdateFoldings(foldingManager, avalon_txt.Document);
        }

        private void Send_RequestClick(object sender, RoutedEventArgs e)
        {
             ViewModel.SendRequestAsync();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.GenerateChart();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.AbsoluteUri);
        }

        private async void Request_AccessToken(object sender, RoutedEventArgs e)
        {
            try
            {
                Permissions pm = new Permissions();
                pm.ShowDialog();

                if (pm.DialogResult == false)
                {
                    throw new Exception("Failed to get Token");
                }

                WebAuthenticationBroker w = new WebAuthenticationBroker(appId.Text, new Uri(redirectUri.Text), pm.Scopes);
                var token = await w.AuthenticateAsync();

                var t = ViewModel.HttpRequest.QueryParameters["access_token"];
                if(t == null)
                {
                    var p = new Model.APIParameter();
                    ViewModel.HttpRequest.AddQueryParameter("access_token", token.Token);
                }
                else
                {
                    t.Value = token.Token;
                }
            }
            catch
            {
                MessageBox.Show("Could not get access token.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void Token_LostFocus(object sender, RoutedEventArgs e)
        {
            var token = ViewModel.HttpRequest.QueryParameters["access_token"];
            if (token == null)
            {
                ViewModel.HttpRequest.AddQueryParameter("access_token", txt_token.Text);
            }
        }

        private void appId_LostFocus(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.appId = appId.Text;
            Properties.Settings.Default.Save();
        }

        private void Add_Param_Click(object sender, RoutedEventArgs e)
        {
            int i = 1;

            while(ViewModel.HttpRequest.QueryParameters.Any(x=> x.Name.Equals($"param{i}")))
            {
                i++;
            }
            ViewModel.HttpRequest.AddQueryParameter($"param{i}", "");
        }

        private void Add_Header_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.HttpRequest.HeaderFields.Add(new Model.APIParameter());
        }


        private void Add_Body_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.HttpRequest.BodyFields.Add(new Model.APIParameter());
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            if(HelpUri != null)
            {
                Process.Start(HelpUri.AbsoluteUri);
            }
        }
    }
}
