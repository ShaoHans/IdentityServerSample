using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ConsoleAppClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var discover = DiscoveryClient.GetAsync("http://localhost:5000").Result;
            if(discover.IsError)
            {
                Console.WriteLine(discover.Error);
                return;
            }

            var tokenClient = new TokenClient(discover.TokenEndpoint, "client1", "secret");
            // 客户端模式
            var tokenResponse = tokenClient.RequestClientCredentialsAsync("api1").Result;
            if(tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
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
