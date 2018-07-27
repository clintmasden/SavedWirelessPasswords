using MahApps.Metro.Controls;
using System.Threading.Tasks;
using System.Windows;

namespace SavedWirelessPasswords.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private WifiRoutines _wifiRoutines;

        public MainWindow()
        {
            InitializeComponent();

            _wifiRoutines = new WifiRoutines();
        }

        private async void wMain_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadWifiProfiles();
        }

        private async Task LoadWifiProfiles()
        {
            cnStatus.Visibility = Visibility.Visible;
            lblStatus.Content = "Refreshing Network...";
            await Task.Delay(500);

            this.spProfiles.Children.Clear();

            var wifiProfiles = await _wifiRoutines.WifiProfiles();

            foreach (var profile in wifiProfiles)
            {
                this.spProfiles.Children.Add(profile);
            }

            await Task.Delay(500);
            cnStatus.Visibility = Visibility.Hidden;
        }

        private async void miRefresh_Click(object sender, RoutedEventArgs e)
        {
            await LoadWifiProfiles();
        }

        private void miInfo_Click(object sender, RoutedEventArgs e)
        {
            var wInfo = new wInformation();
            wInfo.ShowDialog();
        }
    }
}