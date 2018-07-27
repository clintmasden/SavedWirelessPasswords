using NativeWifi;
using SavedWirelessPasswords.Models;
using SavedWirelessPasswords.UserControls;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SavedWirelessPasswords
{
    public class WifiRoutines
    {
        private List<Profile> _profiles = new List<Profile>();

        public async Task<List<uWifi>> WifiProfiles()
        {
            List<uWifi> wifiProfiles = new List<uWifi>();

            await GetSignals();
            await GetProfiles();

            _profiles = _profiles.OrderByDescending(p => p.SignalStrength).ToList();

            foreach (var profile in _profiles)
            {
                uWifi uWifi = new uWifi(profile.Name, profile.SignalStrength, profile.Password);
                wifiProfiles.Add(uWifi);
            }

            return wifiProfiles;
        }

        private async Task GetSignals()
        {
            await Task.Delay(10);

            WlanClient client = new WlanClient();
            foreach (WlanClient.WlanInterface wlanIface in client.Interfaces)
            {
                Wlan.WlanAvailableNetwork[] networks = wlanIface.GetAvailableNetworkList(0);
                foreach (Wlan.WlanAvailableNetwork network in networks)
                {
                    string profileName = Encoding.ASCII.GetString(network.dot11Ssid.SSID, 0, (int)network.dot11Ssid.SSIDLength);
                    int signalStrength = (int)network.wlanSignalQuality;

                    if (!_profiles.Any(p => p.Name == profileName))
                    {
                        _profiles.Add(new Profile()
                        {
                            Name = profileName,
                            Password = string.Empty,
                            SignalStrength = signalStrength
                        });
                    }
                }
            }
        }

        private async Task GetProfiles()
        {
            await Task.Delay(10);

            string savedProfiles = await GetCommandOutput("netsh", @"wlan show profile");

            Regex profileRegex = new Regex(@"\:(.*)", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
            MatchCollection profileMatches = profileRegex.Matches(savedProfiles);
            foreach (Match profile in profileMatches)
            {
                if (profile.Success)
                {
                    string profileName = profile.Groups[1].ToString().Trim();

                    if (!string.IsNullOrWhiteSpace(profileName))
                    {
                        string savedInformation = await GetCommandOutput("netsh", $"wlan show profile \"{profileName}\" key=clear");

                        Regex keyContentRegex = new Regex(@"Key Content            :(.*)", RegexOptions.IgnoreCase);
                        MatchCollection keyContentMatches = keyContentRegex.Matches(savedInformation);
                        foreach (Match keyContent in keyContentMatches)
                        {
                            if (keyContent.Success)
                            {
                                string profilePassword = keyContent.Groups[1].ToString().Trim();

                                UpdateProfile(profileName, profilePassword);
                            }
                        }
                    }
                }
            }
        }

        private void UpdateProfile(string profileName, string profilePassword)
        {
            var profile = _profiles.Where(p => p.Name == profileName).SingleOrDefault();

            if (profile == null)
            {
                _profiles.Add(new Profile()
                {
                    Name = profileName,
                    Password = profilePassword,
                    SignalStrength = 0
                });
            }
            else
            {
                _profiles.Where(p => p.Name == profileName).SingleOrDefault().Password = profilePassword;
            }
        }

        private async Task<string> GetCommandOutput(string fileName, string arguments)
        {
            await Task.Delay(10);

            Process p = new Process();
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;

            p.StartInfo.FileName = fileName;
            p.StartInfo.Arguments = arguments;
            p.Start();

            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            return output;
        }
    }
}