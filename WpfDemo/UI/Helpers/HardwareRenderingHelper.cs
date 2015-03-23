namespace UI.Helpers
{
//  using Akavache;
//  using NLog;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Management;
//  using System.Reactive.Linq;
//  using System.Reactive.Threading.Tasks;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;
    using System.Windows.Interop;
    using System.Windows.Media;

    public static class HardwareRenderingHelper
    {
//         private static readonly Logger log = LogManager.GetCurrentClassLogger();
//         private static readonly Tuple<int, string>[] videoCardBlacklist = new Tuple<int, string>[] { Tuple.Create<int, string>(0x25b, "8086:0a16") };
// 
//         public static void DisableHwRenderingForCrapVideoCards(IBlobCache blobCache)
//         {
//             int osVersion;
//             if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("GH_FORCE_HW_RENDERING")))
//             {
//                 if (!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("GH_FORCE_SW_RENDERING")))
//                 {
//                     EnableSoftwareMode();
//                 }
//                 else
//                 {
//                     osVersion = (Environment.OSVersion.Version.Major * 100) + Environment.OSVersion.Version.Minor;
//                     if (osVersion < 0x259)
//                     {
//                         log.Warn("Hardware acceleration is much more glitchy on OS's earlier than Vista");
//                         log.Warn("If you believe this isn't the case, set the GH_FORCE_HW_RENDERING environment variable");
//                         EnableSoftwareMode();
//                     }
//                     else if (File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "GitHub", ".myvideocardisawful")))
//                     {
//                         if (TaskObservableExtensions.ToTask<string>(blobCache.GetOrFetchObject<string>("MyVideoCardIsAwful", (Func<IObservable<string>>) (() => Observable.Return<string>("NotYet")), null)).Result == "NotYet")
//                         {
//                             log.Warn("Hardware acceleration has been disabled because we detected a crash with your graphics card");
//                             log.Warn("We're sending some details back to our bugtracker so we can fix this real quick");
//                             log.Warn("If you're reading this, we'd love to hear from you - drop us a line at support@github.com");
//                             string[] source = getVideoCardIdentifiers();
//                             string str3 = source.Any<string>() ? string.Join(" - ", source) : "empty";
//                             string message = string.Format("VIDEO-CARDS: we had a rendering issue, these were the cards found: " + str3, new object[0]);
//                             LogEventInfo logEvent = LogEventInfo.Create(NLog.LogLevel.Error, log.Name, message);
//                             logEvent.Properties["IsFatalException"] = true;
//                             logEvent.Properties["VideoCards"] = str3;
//                             log.Log(logEvent);
//                             blobCache.InsertObject<string>("MyVideoCardIsAwful", "true", null);
//                         }
//                         EnableSoftwareMode();
//                     }
//                     else
//                     {
//                         string[] result = TaskObservableExtensions.ToTask<string[]>(blobCache.GetOrFetchObject<string[]>("VideoCardIdentifiers", (Func<IObservable<string[]>>) (() => Observable.Return<string[]>(getVideoCardIdentifiers())), new DateTimeOffset?(DateTimeOffset.Now.AddDays(7.0)))).Result;
//                         for (int i = 0; i < result.Length; i++)
//                         {
//                             Func<Tuple<int, string>, bool> predicate = null;
//                             string pnpId = result[i];
//                             if (predicate == null)
//                             {
//                                 predicate = x => (x.Item1 == osVersion) && (x.Item2 == pnpId);
//                             }
//                             if (videoCardBlacklist.Any<Tuple<int, string>>(predicate))
//                             {
//                                 log.Warn("Your video card is known to cause graphical glitches, so we have disabled hardware rendering");
//                                 log.Warn("If you believe this isn't the case, set the GH_FORCE_HW_RENDERING environment variable");
//                                 EnableSoftwareMode();
//                             }
//                         }
//                         log.Info("Your video card appears to support hardware rendering. If this isn't the case and you see glitches");
//                         log.Info("set the GH_FORCE_SW_RENDERING environment variable to 1");
//                     }
//                 }
//             }
//         }

        private static void EnableSoftwareMode()
        {
            RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
            IsInSoftwareMode = true;
        }

//         private static string[] getVideoCardIdentifiers()
//         {
//             log.Info("Making a WMI query to discover any video PNPID entries");
//             try
//             {
//                 Regex regex = new Regex("VEN_([0-9A-Z]{4})&DEV_([0-9A-Z]{4})");
//                 ManagementObjectCollection objects = new ManagementObjectSearcher(@"root\cimv2", "SELECT * FROM win32_videocontroller").Get();
//                 List<string> list = new List<string>();
//                 using (ManagementObjectCollection.ManagementObjectEnumerator enumerator = objects.GetEnumerator())
//                 {
//                     while (enumerator.MoveNext())
//                     {
//                         string propertyValue = (string) enumerator.Current.GetPropertyValue("PNPDeviceID");
//                         Match match = regex.Match(propertyValue);
//                         if (match.Success && (match.Captures.Count == 3))
//                         {
//                             string argument = string.Format("{0}:{1}", match.Groups[1].Value, match.Groups[2].Value);
//                             log.Info("Found a video PNPID: {0}", argument);
//                             list.Add(argument);
//                         }
//                     }
//                 }
//                 return list.ToArray();
//             }
//             catch (Exception exception)
//             {
//                 log.WarnException("The WMI query for available graphics cards has failed, skipping", exception);
//                 return new string[0];
//             }
//         }

        public static bool IsInSoftwareMode { get; private set; }
    }
}

