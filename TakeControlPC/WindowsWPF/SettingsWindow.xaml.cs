using Microsoft.Win32;
using System;
using System.Reflection;
using System.Windows;

namespace TakeControlPC.WindowsWPF
{
    public partial class SettingsWindow : Window
    {
        private string _startupPath;
        public SettingsWindow()
        {
            InitializeComponent();
            Info.Text = "1. Для работы приложения укажите свободный порт\r\n\r\n2. Выберите сетевой адаптер отвечающий за ваше подключение к сети\r\n\r\n3. Укажите соответсвующие порт и ip в приложении на смартфоне";
            _startupPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup) + "\\TakeControlPC.ink";
            using (RegistryKey writeKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\TakeControlPC"))
            {
                if (writeKey.GetValue("startWork") != null)
                {
                    int boolean = (int)writeKey.GetValue("startWork");
                    checkWork.IsChecked = boolean == 1 ? true : false;
                }

            }
            using (RegistryKey writeKey = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run"))
            {
                if (writeKey.GetValue("TakeControlPC") != null)
                {
                    checkStart.IsChecked = true;
                }
            }

            if (checkStart.IsChecked == true)
            {
                checkWork.IsEnabled = true;
            }
            else
            {
                checkWork.IsChecked = false;
                checkWork.IsEnabled = false;
            }

            checkStart.Click += (s, e) =>
            {
                if (checkStart.IsChecked == true)
                {
                    checkWork.IsEnabled = true;
                }
                else
                {
                    checkWork.IsChecked = false;
                    checkWork.IsEnabled = false;
                }
            };
        }

        private void ButtonApply_Click(object sender, RoutedEventArgs e)
        {
            if (checkStart.IsChecked == true && System.IO.File.Exists(_startupPath) != true)
            {
                using (RegistryKey writeKey = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
                {
                    string appPath = Assembly.GetExecutingAssembly().Location.Replace(".dll", ".exe");
                    writeKey.SetValue("TakeControlPC", appPath);
                }
            }
            else
            {
                using (RegistryKey writeKey = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
                {
                    string appPath = Assembly.GetExecutingAssembly().Location.Replace(".dll", ".exe");
                    writeKey.DeleteValue("TakeControlPC");
                }
            }
            if (checkWork.IsChecked == true)
            {
                using (RegistryKey writeKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\TakeControlPC"))
                {
                    writeKey.SetValue("startWork", 1);
                }
            }
            else
            {
                using (RegistryKey writeKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\TakeControlPC"))
                {
                    writeKey.SetValue("startWork", 0);
                }
            }
            this.Close();
        }
    }
}
