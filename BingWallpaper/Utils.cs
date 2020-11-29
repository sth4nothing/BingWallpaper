using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.IO;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Flurl;
using Flurl.Http;
using System.Dynamic;
using System.Windows.Forms;
using System.Drawing;
using Newtonsoft.Json.Linq;

namespace BingWallpaper
{
    static class Utils
    {
        public const string url = "http://www.bing.com/HPImageArchive.aspx";
        public static readonly string WorkDir = AppDomain.CurrentDomain.BaseDirectory;
        public static readonly string WallpapersDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "Wallpapers");
        public static readonly string DataDir = Path.Combine(WorkDir, "data");

        public static string DateString
        {
            get => DateTime.Today.ToString("yyyyMMdd");
        }

        [DllImport("Wallpaper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern int SetWallpaper([MarshalAs(UnmanagedType.LPWStr)] string path);
        [DllImport("Shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int SHCreateItemFromParsingName(string pszPath, IntPtr pbc, [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid, out IShellItem ppv);


        [DllImport("Shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int SHCreateShellItemArrayFromShellItem(IShellItem psi, [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid, out IShellItemArray ppv);

        public static bool RegisterStartup()
        {
            try
            {
                var proc = System.Diagnostics.Process.GetCurrentProcess();
                var path = new FileInfo(proc.MainModule.FileName).FullName;
                using (var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
                {
                    var value = $"\"{path}\"";
                    if (key.GetValue(nameof(BingWallpaper)) as string != value)
                    {
                        key.SetValue(nameof(BingWallpaper), value);
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        public static bool SetSlideshow()
        {
            try
            {
                string wallpaperDir = Path.Combine(WallpapersDir, "wallpaper" + DateString);
                if (!Directory.Exists(wallpaperDir)) return false;

                int hr = 0;
                if ((hr = SHCreateItemFromParsingName(wallpaperDir, IntPtr.Zero, typeof(IShellItem).GUID, out IShellItem pShellItem)) < 0)
                {
                    return false;
                }

                if ((hr = SHCreateShellItemArrayFromShellItem(pShellItem, typeof(IShellItemArray).GUID, out IShellItemArray pShellItemArray)) < 0)
                {
                    return false;
                }
                var wallpaper = (IDesktopWallpaper)new DesktopWallpaperClass();
                wallpaper.SetSlideshow(pShellItemArray);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public static Wallpaper[] LoadData()
        {
            var name = DateString + ".json";
            var dir = new DirectoryInfo(DataDir);
            if (!dir.Exists) dir.Create();

            dir.EnumerateFiles("*.json")
               .ForEach(x =>
               {
                   if (x.Name != name) x.Delete();
               });

            var path = Path.Combine(dir.FullName, name);
            if (!File.Exists(path))
            {
                return null;
            }
            return JsonConvert.DeserializeObject<Wallpaper[]>(File.ReadAllText(path));
        }
        public static void SaveData(Wallpaper[] wallpapers)
        {
            var name = DateString + ".json";
            var dir = new DirectoryInfo(DataDir);
            if (!dir.Exists) dir.Create();
            var path = Path.Combine(dir.FullName, name);
            File.WriteAllText(path, JsonConvert.SerializeObject(wallpapers));
        }

        public static async Task<Wallpaper[]> QueryDataAsync()
        {
            var idx = 0;
            var dict = new Dictionary<string, Wallpaper>();
            while (true)
            {
                var text = await url.SetQueryParams(new
                {
                    format = "js",
                    mbl = "1",
                    idx,
                    n = "1",
                    video = "1",
                    cc = "en"
                }).GetStringAsync();
                var json = JsonConvert.DeserializeObject<JObject>(text);
                if (!json.ContainsKey("images") || json["images"] is not JArray || json["images"].Count() == 0)
                {
                    break;
                }
                var wp = Wallpaper.FromJObject(json["images"].First() as JObject);
                if (dict.ContainsKey(wp.Hash))
                {
                    break;
                }
                dict[wp.Hash] = wp;
                ++idx;
            }
            var wallpapers = dict.Values.ToArray();
            SaveData(wallpapers);
            return wallpapers;
        }

        public static void DownloadWallpaper(Wallpaper wp, Action<int> tick = null)
        {
            var remote = new Uri(new Uri(url), wp.Url);
            var localDir = Path.Combine(WallpapersDir, "wallpaper" + DateString);
            if (!Directory.Exists(localDir))
            {
                Directory.CreateDirectory(localDir);
            }
            var localFile = Path.Combine(localDir, wp.Hash + ".jpg");
            if (File.Exists(localFile))
            {
                tick(100);
                return;
            }
            var webClient = new WebClientEx();
            webClient.DownloadProgressChanged += (o, e) => tick(e.ProgressPercentage);
            webClient.DownloadFileCompleted += (o, e) =>
            {
                if (e.Error != null)
                {
                    new Task(() =>
                    {
                        if (File.Exists(localFile))
                            File.Delete(localFile);
                    }).Start();
                }
            };
            webClient.DownloadFileAsync(remote, localFile);
            while (webClient.IsBusy)
            {
                System.Threading.Thread.Sleep(100);
            }
        }
    }
}
