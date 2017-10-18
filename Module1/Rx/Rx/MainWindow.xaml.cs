using System;
using System.Configuration;
using System.IO;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Rx
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Observable.FromEventPattern<TextChangedEventArgs>(textBox, "TextChanged")
                      .Where(textBox => GetQueryPredicate(this.textBox.Text))
                      .Throttle(TimeSpan.FromMilliseconds(2000))
                      .ObserveOnDispatcher()
                      .Subscribe(HandleTextChanged);
        }

        private bool GetQueryPredicate(string query)
        {
            return !string.IsNullOrEmpty(query) && query.Length > 3;
        }

        private void HandleTextChanged(object eventArgs)
        {
            Task.Delay(2000);
            WriteMessageToLog($"Message '{textBox.Text}' sent");
        }

        public static void WriteMessageToLog(string logText)
        {
            var logFilePath = ConfigurationManager.AppSettings["logFilePath"];
            if (string.IsNullOrEmpty(logFilePath))
            {
                return;
            }

            var streamWriter = new StreamWriter(logFilePath, true);
            streamWriter.WriteLine(logText);
            streamWriter.Close();
        }
    }
}
