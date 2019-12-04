using ICSharpCode.AvalonEdit.AddIn;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.AvalonEdit.Indentation.CSharp;
using LiveCharts;
using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.ComponentModel.Design;

namespace FacebookIDE.Views
{
    /// <summary>
    /// Interação lógica para ScriptViewer.xam
    /// </summary>
    public partial class ScriptViewer : UserControl
    {
        public ScriptViewer()
        {
            InitializeComponent();

            ConfigureEditor();
        }

        public ObservableCollection<CompilerError> ErrorList { get; set; } = new ObservableCollection<CompilerError>();

        BraceFoldingStrategy foldingStrategy;
        FoldingManager foldingManager;

        private void ConfigureEditor()
        {
            foldingManager = FoldingManager.Install(editor.TextArea);
            foldingStrategy = new BraceFoldingStrategy();
            UpdateTextView(editor.Document.Text);

            editor.Options.EnableHyperlinks = true;
            editor.TextArea.IndentationStrategy = new CSharpIndentationStrategy();

            InitializeTextMarkerService();
        }

        private void UpdateTextView(string value)
        {
            editor.Document.Text = value;
            foldingStrategy.UpdateFoldings(foldingManager, editor.Document);
        }

        private ITextMarkerService textMarkerService;

        void InitializeTextMarkerService()
        {
            var textMarkerService = new TextMarkerService(editor.Document);
            editor.TextArea.TextView.BackgroundRenderers.Add(textMarkerService);
            editor.TextArea.TextView.LineTransformers.Add(textMarkerService);
            IServiceContainer services = (IServiceContainer)editor.Document.ServiceProvider.GetService(typeof(IServiceContainer));
            if (services != null)
                services.AddService(typeof(ITextMarkerService), textMarkerService);
            this.textMarkerService = textMarkerService;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CSharpCodeProvider provider = new CSharpCodeProvider();
                CompilerParameters parameters = new CompilerParameters();
                parameters.ReferencedAssemblies.Add("System.dll");
                parameters.ReferencedAssemblies.Add("System.Net.Http.dll");
                //parameters.ReferencedAssemblies.Add("PresentationFramework.dll");
                //parameters.ReferencedAssemblies.Add("PresentationCore.dll");
                //parameters.ReferencedAssemblies.Add("System.dll");
                parameters.ReferencedAssemblies.Add("FacebookIDE.exe");
                parameters.GenerateInMemory = true;
                parameters.GenerateExecutable = false;


                CompilerResults results = provider.CompileAssemblyFromSource(parameters, editor.Document.Text);

                ErrorList.Clear();
                textMarkerService.RemoveAll(m => true);
                if (results.Errors.Count > 0)
                {
                    foreach (CompilerError err in results.Errors)
                    {
                        ErrorList.Add(err);

                        var line = editor.Document.GetLineByNumber(err.Line);
                        ITextMarker marker = textMarkerService.Create(line.Offset + err.Column, line.Length);
                        marker.MarkerTypes = TextMarkerTypes.SquigglyUnderline;
                        marker.MarkerColor = Colors.Red;
                    }
                    return;
                }

                Assembly assembly = results.CompiledAssembly;
                Type program = assembly.GetType("Script");
                var method = program.GetMethod("GetSeries");
                var method2 = program.GetMethod("GetCollection");
                var instance = assembly.CreateInstance("Script");

                method.Invoke(instance, null);

                SeriesCollection s = (SeriesCollection)method2.Invoke(instance, null);

                chart.Series = s;

                tab_control.SelectedIndex = 1;
            }
            catch(TargetInvocationException tie)
            {
                MessageBox.Show(tie.Message + "\r\n\r\n" + tie.InnerException.Message, "Script Error", MessageBoxButton.OK, MessageBoxImage.Error); ;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        public static readonly DependencyProperty CodeProperty =
         DependencyProperty.Register("Code", typeof(string), typeof(ScriptViewer), new
            PropertyMetadata(new PropertyChangedCallback(OnCodeChanged)));

        private static void OnCodeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ScriptViewer).UpdateTextView((string)e.NewValue);
        }

        public string Code
        {
            get { return (string)GetValue(CodeProperty); }
            set
            {
                SetValue(CodeProperty, value);
            }
        }

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        
    }
}
