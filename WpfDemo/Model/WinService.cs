using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;

namespace Model
{
    public class WinService
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }

        public static List<WinService> CurrentList()
        {
            try
            {
                ManagementObjectCollection objects = new ManagementObjectSearcher(@"root\cimv2", "SELECT * FROM Win32_Service").Get();
                List<WinService> list = new List<WinService>();
                using (ManagementObjectCollection.ManagementObjectEnumerator enumerator = objects.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        WinService newService = new WinService();
                        newService.Name = (string)enumerator.Current.GetPropertyValue("Name");
                        newService.DisplayName = (string)enumerator.Current.GetPropertyValue("DisplayName");
                        list.Add(newService);
                    }
                }
                return list;
            }
            catch (Exception exception)
            {
                return new List<WinService>();
            }
        }
    }
}
