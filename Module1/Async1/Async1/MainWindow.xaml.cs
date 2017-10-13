using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Async1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CancellationTokenSource _cancellationToken;

        private long _totalReadBytes, _totalBytes;

        private const int BufferSize = 4096, BytesCount = 1024;

        public MainWindow()
        {
            _totalReadBytes = 0;
            _totalBytes = -1;
            _cancellationToken = new CancellationTokenSource();

            InitializeComponent();
        }

        public async Task DownloadFileAsync(string url, IProgress<double> progress, CancellationToken token)
        {
            using (var client = new HttpClient())
            {
                using (var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, token))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception(string.Format(Properties.Resources.RequestReturnedWithStatusCodeMessage, response.StatusCode));
                    }

                    _totalBytes = response.Content.Headers.ContentLength ?? -1;

                    var fullFileName = GetFullFileName(url);

                    if (File.Exists(fullFileName))
                    {
                        File.Delete(fullFileName);
                    }

                    using (var stream = await response.Content.ReadAsStreamAsync())
                    {
                        var bufferArray = new byte[BufferSize];
                        var isCompleted = true;
                        _totalReadBytes = 0;

                        do
                        {
                            token.ThrowIfCancellationRequested();

                            var readerStream = await stream.ReadAsync(bufferArray, 0, bufferArray.Length, token);

                            if (readerStream == 0)
                            {
                                isCompleted = false;
                            }
                            else
                            {
                                var data = new byte[readerStream];
                                bufferArray.ToList().CopyTo(0, data, 0, readerStream);

                                using (var writerStream = new FileStream(fullFileName, FileMode.Append))
                                {
                                    await writerStream.WriteAsync(data, 0, data.Length, token);
                                }

                                _totalReadBytes += readerStream;

                                if (_totalBytes == -1 || progress == null)
                                {
                                    continue;
                                }

                                var progressValue = _totalReadBytes / (double)_totalBytes * 100;
                                progress.Report(progressValue);
                            }
                        }
                        while (isCompleted);
                    }
                }
            }
        }

        private static bool IsValidUrl(string url)
        {
            Uri uriResult;
            return Uri.TryCreate(url, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        public static string GetFullFileName(string url)
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var fileName = Path.GetFileName(url) ?? string.Empty;
            return Path.Combine(path, fileName);
        }

        public static double GetMegabytes(long bytes)
        {
            return (double) bytes / (BytesCount * BytesCount);
        }

        private void urlTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            DownloadButton.IsEnabled = IsValidUrl(UrlTextBox.Text);
        }
        
        private async void downloadButton_Click(object sender, RoutedEventArgs e)
        {
            var url = UrlTextBox.Text;
            CancelButton.IsEnabled = true;
            ProgressBar.Visibility = Visibility.Visible;

            try
            {
                var progress = new Microsoft.Progress<double>();
                progress.ProgressChanged += (sender1, value) =>
                {
                    ProgressBar.Value = value;
                    var totalReadMb = GetMegabytes(_totalReadBytes);
                    var totalMb = GetMegabytes(_totalBytes);
                    TotalSizeLabel.Content = $"{totalReadMb:0.00} / {totalMb:0.00}";
                };

                await DownloadFileAsync(url, progress, _cancellationToken.Token);

                CancelButton.IsEnabled = false;
                ProgressBar.Visibility = Visibility.Hidden;
                TotalSizeLabel.Content = string.Empty;

                MessageBox.Show(Properties.Resources.FileWasDownloadedMessage, Properties.Resources.InformationCaption, 
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (TaskCanceledException)
            {
                CancelButton.IsEnabled = false;
                ProgressBar.Visibility = Visibility.Hidden;
                TotalSizeLabel.Content = string.Empty;

                _cancellationToken = new CancellationTokenSource();

                MessageBox.Show(Properties.Resources.FileDownloadingWasCanceledMessage, Properties.Resources.InformationCaption, 
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, Properties.Resources.ErrorCaption, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            _cancellationToken.Cancel();
        }
    }
}
