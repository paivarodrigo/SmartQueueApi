using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Api.Utils
{
    public class Client : IClient
    {

        public async Task<List<int>> BuscarTemposPrevistos(int quantidadeDeTempos)
        {
            //Uri requestUri = new Uri($"http://sqpythonia.azurewebsites.net/run/{quantidadeDeTempos}");
            Uri requestUri = new Uri($"http://127.0.0.1:5000/run/{quantidadeDeTempos}");
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(requestUri);

            List<int> tempos = new List<int>();
            if (response.IsSuccessStatusCode)
            {
                string responseString = await response.Content.ReadAsStringAsync();

                dynamic responseJson = JsonConvert.DeserializeObject(responseString);
                foreach (dynamic prop in responseJson)
                {
                    tempos = prop.First.ToObject<List<int>>();
                }
            }
            else
            {
                for (int i = 0; i < quantidadeDeTempos; i++)
                {
                    tempos.Add((i * 5) + 20);
                }
            }

            return tempos;
        }
    }
}
