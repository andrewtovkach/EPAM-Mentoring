using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

namespace PostSharpNotifyChangedProject
{
    public partial class MainWindow : Window
    {
        List<User> items = new List<User>();

        public MainWindow()
        {
            InitializeComponent();
            items.Add(new User() { Name = "Andrei Toukach", PhoneNumber = "1234567", Mail = "andrei_toukach@epam.com" });
            items.Add(new User() { Name = "Dzmitry Safarau", PhoneNumber = "7654321", Mail = "dzmitry_safarau@epam.com" });
            listView.ItemsSource = items;
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
            if (listView.SelectedItems.Count > 0)
            {
                User user = (User)listView.SelectedItems[0];
                name.Text = user.Name;
                phone.Text = user.PhoneNumber;
                mail.Text = user.Mail;
            }
        }
    }

    public class User : INotifyPropertyChanged
    {
        private string name;
        private string phoneNumber;
        private string mail;

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }
        public string PhoneNumber
        {
            get { return phoneNumber; }
            set
            {
                phoneNumber = value;
                OnPropertyChanged("PhoneNumber");
            }
        }

        public string Mail
        {
            get { return mail; }
            set
            {
                mail = value;
                OnPropertyChanged("Mail");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }
    }
}
