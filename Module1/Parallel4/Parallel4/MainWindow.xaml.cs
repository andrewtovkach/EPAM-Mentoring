using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Parallel4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            WorkingWithTasks();
        }

        private static void WorkingWithTasks()
        {
            Task.Run(() => TaskOperation())
                .ContinueWith(task =>
                {
                    Debug.WriteLine("The continuation task is executing...");
                });

            Task.Run(() => TaskOperationWithException())
               .ContinueWith(task =>
               {
                   Debug.WriteLine("The parent task has completed with error");
               }, TaskContinuationOptions.OnlyOnFaulted);

            Task.Run(() => TaskOperationWithException())
               .ContinueWith((task, obj) =>
               {
                   Debug.WriteLine("The parent task has completed with error");
                   Debug.WriteLine("Parent task thread is reusing...");
                   task.Wait();
               }, TaskContinuationOptions.OnlyOnFaulted);

            var cancellationToken = new CancellationToken(true);

            Task.Run(() => TaskOperation(), cancellationToken)
                .ContinueWith((task, obj) =>
                {
                    Debug.WriteLine("The parent task was cancelled");
                    Debug.WriteLine("IsThreadPoolThread = {0}", Thread.CurrentThread.IsThreadPoolThread);
                }, TaskContinuationOptions.NotOnCanceled, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private static void TaskOperation()
        {
            Debug.WriteLine("TaskOperation is executing...");
            Task.Delay(5000).Wait();
        }

        private static void TaskOperationWithException()
        {
            Debug.WriteLine("TaskOperationWithException is executing...");
            Task.Delay(3000).Wait();
            throw new Exception();
        }
    }
}
