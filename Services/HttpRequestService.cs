using System;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WebApp.Services
{
    public class HttpRequestService
    {
        private HttpClient httpClient = new HttpClient();
        // public async void Post<T>(string url, T model)
        // {
        //     string jsonObj = JsonConvert.SerializeObject(model, Formatting.Indented);
        //     StringContent content = new StringContent(jsonObj, Encoding.UTF8, "application/json");

        //     HttpResponseMessage response = await httpClient.PostAsync(url, content);
        // }

        public async Task<T> Get<T>(string url)
        {
            HttpResponseMessage response = await httpClient.GetAsync(url);

            response.Content.Headers.ContentType = 
                new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            
            response.EnsureSuccessStatusCode();
            string result = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(result);
        }
    }
}
