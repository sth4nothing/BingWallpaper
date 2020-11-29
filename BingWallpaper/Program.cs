using ShellProgressBar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BingWallpaper
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
#if DEBUG
#else
                Utils.RegisterStartup();
                Console.WriteLine("设置开启启动项");
#endif
                Console.Title = nameof(BingWallpaper);
                var wallpapers = Utils.LoadData();
                if (wallpapers == null || wallpapers.Length == 0)
                {
                    wallpapers = await Utils.QueryDataAsync();
                }
                if (wallpapers == null || wallpapers.Length == 0)
                {
                    Console.Error.WriteLine("无法获取壁纸列表");
                }
                else
                {
                    DownloadWallpapers(wallpapers);
#if DEBUG
#else
                    Utils.SetSlideshow();
                    Console.WriteLine("设置壁纸成功");
#endif
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
            }
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("请按回车键退出...");
            Console.ReadLine();
        }

        private static void DownloadWallpapers(Wallpaper[] wallpapers)
        {
            if (!Directory.Exists(Utils.WallpapersDir))
            {
                Directory.CreateDirectory(Utils.WallpapersDir);
            }
            var opts = new ProgressBarOptions
            {
                DisplayTimeInRealTime = true,
                ForegroundColor = ConsoleColor.Yellow,
                BackgroundColor = ConsoleColor.DarkYellow,
                EnableTaskBarProgress = true,
                ProgressCharacter = '=',
            };
            using (var pbar = new ProgressBar(wallpapers.Length, "下载中......", opts))
            {
                var childOpts = new ProgressBarOptions
                {
                    DisplayTimeInRealTime = true,
                    ForegroundColor = ConsoleColor.Green,
                    BackgroundColor = ConsoleColor.DarkGreen,
                    ProgressCharacter = '=',
                };
                new DirectoryInfo(Utils.WallpapersDir)
                    .EnumerateDirectories("wallpaper*")
                    .ForEach(x =>
                    {
                        if (x.Name != "wallpaper" + Utils.DateString) x.Delete(true);
                    });
                foreach (var wp in wallpapers)
                {
                    using (var cbar = pbar.Spawn(100, wp.Url, childOpts))
                    {
                        Utils.DownloadWallpaper(wp, x => cbar.Tick(x));
                    }
                    pbar.Tick();
                }
                pbar.Message = "下载完成";
            }
            Console.SetCursorPosition(0, wallpapers.Length * 2 + 2);
            Console.CursorVisible = true;
        }
    }
}
