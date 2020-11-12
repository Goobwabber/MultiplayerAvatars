using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerAvatars
{
    public static class Utilities
    {
        private static HttpClient? _httpClient;

        public static HttpClient HttpClient
        {
            get
            {
                if (_httpClient == null)
                {
                    _httpClient = new HttpClient();
                    _httpClient.DefaultRequestHeaders.UserAgent
                        .ParseAdd(Plugin.UserAgent);
                }
                return _httpClient;
            }
        }
    }
}
