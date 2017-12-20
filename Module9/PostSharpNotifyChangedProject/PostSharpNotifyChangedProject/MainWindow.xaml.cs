using System.Collections.Generic;
using System.Windows;

namespace PostSharpNotifyChangedProject
{
    public partial class MainWindow : Window
    {
        readonly List<User> _items = new List<User>();

        public MainWindow()
        {
            InitializeComponent();
            _items.Add(new User { Name = "Andrei Toukach", PhoneNumber = "1234567", Mail = "andrei_toukach@epam.com" });
            _items.Add(new User { Name = "Dzmitry Safarau", PhoneNumber = "7654321", Mail = "dzmitry_safarau@epam.com" });
            listView.ItemsSource = _items;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (listView.SelectedItems.Count > 0) {
                User user = (User)listView.SelectedItems[0];
                user.Name = name.Text;
                user.PhoneNumber = phone.Text;
                user.Mail = mail.Text;
            }
            else
            {
                MessageBox.Show("You should select some item from the list!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ListView_Selected(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (listView.SelectedItems.Count <= 0)
                return;

            User user = (User)listView.SelectedItems[0];
            name.Text = user.Name;
            phone.Text = user.PhoneNumber;
            mail.Text = user.Mail;
        }
    }
}
