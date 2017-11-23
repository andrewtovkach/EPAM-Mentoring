using System;
using System.IO;
using System.ServiceModel.Syndication;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using Saxon.Api;

namespace Task2WPFApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var xsltFileName = xslt.Text;
                var inputFileName = input.Text;

                ProcessXmlFile(inputFileName, xsltFileName, "result.xml");
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error message", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ProcessXmlFile(string inputFileName, string xsltFileName, string outputFileName)
        {
            Processor processor = new Processor(true);

            Xslt30Transformer transformer = processor.NewXsltCompiler().Compile(new Uri(Path.GetFullPath(xsltFileName))).Load30();

            Serializer serializer = new Serializer();

            StreamWriter writer = new StreamWriter(outputFileName);

            serializer.SetOutputWriter(writer);

            using (FileStream fileStream = new FileStream(inputFileName, FileMode.Open))
            {
                transformer.ApplyTemplates(fileStream, serializer);
            }

            writer.Close();

            using (XmlReader reader = XmlReader.Create(outputFileName))
            {
                SyndicationFeed feed = SyndicationFeed.Load(reader);
                listBoxTodoList.ItemsSource = feed.Items;
            }
        }

        private void Input_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(input.Text) && !string.IsNullOrEmpty(xslt.Text))
            {
                Button.IsEnabled = true;
            }
            else
            {
                Button.IsEnabled = false;
            }
        }

        private void Xslt_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(input.Text) && !string.IsNullOrEmpty(xslt.Text))
            {
                Button.IsEnabled = true;
            }
            else
            {
                Button.IsEnabled = false;
            }
        }
    }
}
