using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;

namespace Projecto.Helper
{
    public class ChatAPI
    {
        public HttpClient Initial()
        {
            var client = new HttpClient
            {
                // BaseAddress = new Uri("http://localhost:24733/")
                BaseAddress = new Uri("https://localhost:44336/")
            };
            return client;
        }
    }
}
