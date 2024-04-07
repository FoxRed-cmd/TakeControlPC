using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using TakeControlPC.Classes;
using TakeControlPC.WindowsWPF;

namespace TakeControlPC
{
    public partial class MainWindow : Window
    {
        private CommandServer _command;
        private List<NetworkAdapter> _adapters;
        private string _ipconfig;
        private string _answer;
        private string _args;
        private NotifyIcon _notifyIcon;
        private ContextMenuStrip _contextMenu;
        private SettingsWindow _settingsWindow;

        public MainWindow()
        {
            InitializeComponent();

            _contextMenu = new ContextMenuStrip();
            _contextMenu.Items.Add("Show", null, (s, e) => { this.Show(); _notifyIcon.Visible = false; });
            _contextMenu.Items.Add("Stop", null, (s, e) =>
            {
                this.Show();
                _notifyIcon.Visible = false;
                if (_command == null)
                    return;
                btnStart.Content = "Start";
                _command.StopListening();
            });
            _contextMenu.Items.Add("Exit", null, (s, e) => { _notifyIcon.Visible = false; App.Current.Shutdown(); });

            _notifyIcon = new NotifyIcon()
            {
                Visible = false,
            };
            _notifyIcon.Icon = Properties.Resources.TCIcon1;
            _notifyIcon.ContextMenuStrip = _contextMenu;
            _notifyIcon.DoubleClick += (s, e) => { this.Show(); _notifyIcon.Visible = false; };

            using (var process = new System.Diagnostics.Process())
            {
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.Arguments = "/c chcp 65001 & ipconfig";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.StandardOutputEncoding = System.Text.Encoding.UTF8;

                process.Start();

                _ipconfig = process.StandardOutput.ReadToEnd();
                _adapters = NetworkAdapter.ParseNetworkAdaptersInfo(_ipconfig);

                foreach (var adapter in _adapters)
                {
                    adaptersBox.Items.Add(adapter.Name);
                }
            }

            adaptersBox.SelectedIndex = 0;
            adaptersBox.SelectionChanged += (s, e) =>
            {
                txtIP.Text = _adapters[adaptersBox.SelectedIndex].IPv4Address;
            };

            txtPort.TextChanged += (s, e) =>
            {
                if (txtPort.Text != string.Empty)
                {
                    if (txtPort.Text.Last() < 48 || txtPort.Text.Last() > 57)
                    {
                        txtPort.Text = txtPort.Text.Remove(txtPort.Text.LastIndexOf(txtPort.Text.Last()));
                        txtPort.SelectionStart = txtPort.Text.Length;
                    }
                    else if (txtPort.Text.First() < 48 || txtPort.Text.First() > 57)
                    {
                        txtPort.Text = txtPort.Text.Remove(txtPort.Text.LastIndexOf(txtPort.Text.First()), 1);
                        txtPort.SelectionStart = txtPort.Text.Length;
                    }
                }
            };
        }

        private void ButtonStartStop_Click(object sender, RoutedEventArgs e)
        {
            StartWork();
        }

        private async void StartWork()
        {
            if (btnStart.Content.Equals("Start"))
            {
                this.Hide();
                _notifyIcon.Visible = true;

                btnStart.Content = "Stop";
                _command = new CommandServer(txtIP.Text, txtPort.Text);
                do
                {
                    await _command.StartListeningAsync();
                    _answer = _command.Answer;
                    if (_answer.Contains("-"))
                    {
                        string[] temp = _answer.Split('-');
                        _answer = temp[0];
                        _args = temp[1];
                    }
                    switch (_answer)
                    {
                        case "/volup": Commands.SetVolumeUp(this); break;
                        case "/voldown": Commands.SetVolumeDown(this); break;
                        case "/voloff": Commands.SetVolumeMute(this); break;
                        case "/pause": Commands.PausePlay(); break;
                        case "/stop": Commands.StopTrack(); break;
                        case "/next": Commands.NextTrack(); break;
                        case "/nextStep": Commands.NextStepTrack(); break;
                        case "/previous": Commands.PreviousTrack(); break;
                        case "/previousStep": Commands.PreviousStepTrack(); break;
                        case "/sleep": Commands.SleepPC(); break;
                        case "/restart": Commands.RestartPC(); break;
                        case "/power": Commands.PowerOffPC(); break;
                        case "/start": Commands.LaunchApp(_args); break;
                        default:
                            break;
                    }
                    _command.Answer = string.Empty;
                    await Task.Delay(50);
                } while (_command.IsListening);
            }
            else
            {
                btnStart.Content = "Start";
                _command.StopListening();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            int boolean = 0;
            using (RegistryKey writeRead = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\TakeControlPC"))
            {
                if (writeRead.GetValue("Port") == null || writeRead.GetValue("IP") == null)
                {
                    Settings.SelectPort = txtPort.Text;
                    Settings.SelectIP = adaptersBox.SelectedIndex.ToString();
                    writeRead.SetValue("startWork", 0);
                    Settings.WriteSettings();
                }
                Settings.ReadSettings();
                txtPort.Text = Settings.SelectPort;
                txtIP.Text = _adapters[Convert.ToInt32(Settings.SelectIP)].IPv4Address;
                adaptersBox.SelectedIndex = Convert.ToInt32(Settings.SelectIP);
            }
            using (RegistryKey writeKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\TakeControlPC"))
            {
                boolean = (int)writeKey.GetValue("startWork");
            }
            if (boolean == 1)
            {
                StartWork();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Settings.SelectPort = txtPort.Text;
            Settings.SelectIP = adaptersBox.SelectedIndex.ToString();
            Settings.WriteSettings();
        }

        private void FontAwesome_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!System.Windows.Application.Current.Windows.OfType<SettingsWindow>().Any())
            {
                _settingsWindow = new SettingsWindow();
                _settingsWindow.Show();
                this.Hide();
                _notifyIcon.Visible = true;
            }
        }

        private void Exit_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e) => App.Current.Shutdown();

        private void Hide_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.Hide();
            _notifyIcon.Visible = true;
        }

        private void WrapPanel_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
    }
}