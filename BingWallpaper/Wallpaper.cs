using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BingWallpaper
{
    class Wallpaper
    {
        public static readonly Rectangle rect = Screen.PrimaryScreen.Bounds;
        //public static readonly string resolution = $"{rect.Width}x{rect.Height}";
        public const string resolution = "1920x1080";
        public string Hash { get; }
        public string Url { get; }
        public Wallpaper(string hash, string url)
        {
            Hash = hash;
            Url = url;
        }
        public override string ToString()
        {
            return $"Wallpaper({Hash}->{Url})";
        }
        public static Wallpaper FromJObject(JObject obj)
        {
            if (obj.ContainsKey("hsh") && obj.ContainsKey("url"))
            {
                string hash = obj["hsh"].ToObject<string>();
                string url = null;
                if (obj.ContainsKey("wp") && obj["wp"].ToObject<bool>())
                {
                    var urlbase = obj["urlbase"].ToObject<string>();
                    url = $"{urlbase}_{resolution}.jpg";
                }
                else
                {
                    url = obj["url"].ToObject<string>();
                }
                return new Wallpaper(hash, url);
            }
            return null;
        }
    }
}
