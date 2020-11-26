using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BingWallpaper
{
    class WebClientEx: WebClient
    {
        public WebClientEx(int timeout = 5000)
        {
            Timeout = timeout;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request =  base.GetWebRequest(address);
            request.Timeout = Timeout;
            return request;
        }

        public int Timeout { get; }
    }
}
