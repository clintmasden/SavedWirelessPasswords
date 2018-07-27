using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SavedWirelessPasswords.UserControls
{
    /// <summary>
    /// Interaction logic for uWifi.xaml
    /// </summary>
    public partial class uWifi : UserControl
    {
        private string _wifiPassword = string.Empty;

        public uWifi()
        {
            InitializeComponent();
        }

        public uWifi(string name, int signal, string password = "")
        {
            InitializeComponent();

            lblName.Content = name;

            if (!string.IsNullOrWhiteSpace(password))
            {
                lblInformation.Content = $"Signal: {signal} | Password: {password}";
                _wifiPassword = password;
            }
            else
            {
                lblInformation.Content = $"Signal: {signal}";
            }

            SetDisplay(signal);
        }

        private void SetDisplay(int signal)
        {
            if (signal == 0)
            {
                imgWifi.Source = new BitmapImage(new Uri("pack://application:,,,/SavedWirelessPasswords;component/Resources/wifi-offline.png"));
            }
            else if (signal < 60)
            {
                imgWifi.Source = new BitmapImage(new Uri("pack://application:,,,/SavedWirelessPasswords;component/Resources/wifi-40.png"));
                uWifi1.Background = new SolidColorBrush(Color.FromArgb(100, 17, 158, 215));
            }
            else if (signal < 80)
            {
                imgWifi.Source = new BitmapImage(new Uri("pack://application:,,,/SavedWirelessPasswords;component/Resources/wifi-60.png"));
                uWifi1.Background = new SolidColorBrush(Color.FromArgb(100, 17, 158, 215));
            }
            else
            {
                imgWifi.Source = new BitmapImage(new Uri("pack://application:,,,/SavedWirelessPasswords;component/Resources/wifi-80.png"));
                uWifi1.Background = new SolidColorBrush(Color.FromArgb(100, 17, 158, 215));
            }
        }

        private void imgWifi_MouseDown(object sender, MouseButtonEventArgs e)
        {
            copyPassword();
        }

        private void uWifi_MouseDown(object sender, MouseButtonEventArgs e)
        {
            copyPassword();
        }

        private void imgWifi_MouseEnter(object sender, MouseEventArgs e)
        {
            setOpacity();
        }

        private void imgWifi_MouseLeave(object sender, MouseEventArgs e)
        {
            resetOpacity();
        }

        private void uWifi_MouseEnter(object sender, MouseEventArgs e)
        {
            setOpacity();
        }

        private void uWifi_MouseLeave(object sender, MouseEventArgs e)
        {
            resetOpacity();
        }

        private async void copyPassword()
        {
            if (!string.IsNullOrEmpty(_wifiPassword))
            {
                Clipboard.SetText(_wifiPassword);

                cn_Status.Visibility = Visibility.Visible;
                await Task.Delay(2000);
                cn_Status.Visibility = Visibility.Hidden;
            }
        }

        private void setOpacity()
        {
            if (!string.IsNullOrEmpty(_wifiPassword))
            {
                uWifi1.ToolTip = "Copy Password to Clipboard...";
                imgWifi.Opacity = 1;
                uWifi1.Opacity = 1;
            }
        }

        private void resetOpacity()
        {
            imgWifi.Opacity = .8;
            uWifi1.Opacity = .8;
        }
    }
}