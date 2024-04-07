using System;
using System.Collections.Generic;
using System.Linq;

namespace TakeControlPC.Classes
{
    internal class NetworkAdapter
    {
        public string? Name { get; set; }
        public string? IPv4Address { get; set; }
        public string? SubnetMask { get; set; }
        public string? DefaultGateway { get; set; }

        public static List<NetworkAdapter> ParseNetworkAdaptersInfo(string input)
        {
            var networkAdapters = new List<NetworkAdapter>();

            string[] rowLines = input.Split(new string[] { Environment.NewLine + Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            List<string> lines = new();
            for (int i = 2; i < rowLines.Length; i++)
            {
                if (i % 2 == 0)
                {
                    lines.Add(rowLines[i] + "\r\n" + rowLines[i + 1]);
                }
            }

            foreach (string adapterInfo in lines)
            {
                NetworkAdapter adapter = new NetworkAdapter();

                string[] linesInAdapter = adapterInfo.Split(Environment.NewLine);
                foreach (string line in linesInAdapter)
                {
                    if (line.Contains("adapter "))
                    {
                        string name = line.Substring(line.IndexOf("adapter ") + "adapter ".Length);
                        adapter.Name = name.Trim(':');
                    }
                    else if (line.Contains("IPv4 Address"))
                    {
                        string[] parts = line.Split(':');
                        adapter.IPv4Address = parts[1].Trim();
                    }
                    else if (line.Contains("Subnet Mask"))
                    {
                        string[] parts = line.Split(':');
                        adapter.SubnetMask = parts[1].Trim();
                    }
                    else if (line.Contains("Default Gateway"))
                    {
                        string[] parts = line.Split(':');
                        adapter.DefaultGateway = parts[1].Trim();
                    }
                }

                networkAdapters.Add(adapter);
            }

            return networkAdapters;
        }
    }
}
