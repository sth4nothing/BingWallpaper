using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BingWallpaper
{
    static class Extensions
    {
        public static void ForEach<T>(this IEnumerable<T> ts, Action<T> act)
        {
            foreach (var item in ts)
            {
                act(item);
            }
        }
    }
}
