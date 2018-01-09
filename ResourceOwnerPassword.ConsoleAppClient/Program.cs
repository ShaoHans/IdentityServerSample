using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;

namespace ResourceOwnerPassword.ConsoleAppClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var discover = DiscoveryClient.GetAsync("http://localhost:5000").Result;
            if(discover.IsError)
            {
                Console.WriteLine(discover.Error);
                Console.ReadKey();
                return;
            }

            var tokenClient = new TokenClient(discover.TokenEndpoint, "ro.client", "secret");
            // 密码模式
            var tokenResponse = tokenClient.RequestResourceOwnerPasswordAsync("shh", "123456", "api1").Result;
            if(tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                Console.ReadKey();
                return;
            }

            Console.WriteLine(tokenResponse.Json);

            var client = new HttpClient();
            client.SetBearerToken(tokenResponse.AccessToken);
            var response = client.GetAsync("http://localhost:5001/identity").Result;
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
            }
            else
            {
                var content = response.Content.ReadAsStringAsync().Result;
                Console.WriteLine(JArray.Parse(content));
            }

            Console.ReadKey();
        }
    }
}
