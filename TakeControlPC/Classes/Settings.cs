using Microsoft.Win32;

namespace TakeControlPC
{
    internal class Settings
    {
        public static string SelectPort { get; set; } = string.Empty;
        public static string SelectIP { get; set; } = string.Empty;

        public static void WriteSettings()
        {
            string port = string.Empty;
            string ip = string.Empty;
            using (RegistryKey writeKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\TakeControlPC"))
            {
                if (writeKey.GetValue("Port") != null && writeKey.GetValue("IP") != null)
                {
                    port = writeKey.GetValue("Port").ToString();
                    ip = writeKey.GetValue("IP").ToString();
                }
                if (port != string.Empty && ip != string.Empty && SelectPort != string.Empty && SelectIP != string.Empty)
                {
                    if (port.Equals(SelectPort) && ip.Equals(SelectIP))
                        return;
                    writeKey.SetValue("Port", SelectPort);
                    writeKey.SetValue("IP", SelectIP);
                    return;
                }
                writeKey.SetValue("Port", SelectPort);
                writeKey.SetValue("IP", SelectIP);
            }
        }

        public static void ReadSettings()
        {
            using (RegistryKey readKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\TakeControlPC"))
            {
                if (readKey.GetValue("Port") != null && readKey.GetValue("IP") != null)
                {
                    SelectPort = readKey.GetValue("Port").ToString();
                    SelectIP = readKey.GetValue("IP").ToString();
                }
            }
        }
    }
}
