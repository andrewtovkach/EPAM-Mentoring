using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace Async2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int _countTasks, _countUnfinishedTasks;

        public MainWindow()
        {
            _countTasks = 0;
            _countUnfinishedTasks = 0;

            InitializeComponent();
        }

        private async void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            var tasksList = new List<Task>
            {
                FirsLongTask(),
                SecondLongTask(),
                ThirdLongTask(),
                FourthLongTask(),
                FifthLongTask()
            };

            await MultipleTaskRun(tasksList, () => 
                tasksCountLabel.Content = $"{_countUnfinishedTasks} / {_countTasks} tasks are in progress");

            loadingImage.Visibility = Visibility.Hidden;
            tasksCountLabel.Visibility = Visibility.Hidden;
            loadedLabel.Visibility = Visibility.Visible;
        }

        private async Task MultipleTaskRun(ICollection<Task> tasksList, Action action)
        {
            _countTasks = tasksList.Count;
            _countUnfinishedTasks = tasksList.Count;

            while (tasksList.Count > 0)
            {
                action();

                var firstFinishedTask = await Task.WhenAny(tasksList);

                tasksList.Remove(firstFinishedTask);
                _countUnfinishedTasks = tasksList.Count;

                await firstFinishedTask;
            }
        }

        private async Task FirsLongTask()
        {
            await Task.Delay(10000);
        }

        private async Task SecondLongTask()
        {
            await Task.Delay(5000);
        }

        private async Task ThirdLongTask()
        {
            await Task.Delay(2000);
        }

        private async Task FourthLongTask()
        {
            await Task.Delay(7000);
        }

        private async Task FifthLongTask()
        {
            await Task.Delay(15000);
        }
    }
}
