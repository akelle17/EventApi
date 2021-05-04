using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace EventsAPI.Services
{
    public class HttpLookupEmployees : ILookupEmployees
    {
        private readonly HttpClient _client;

        public HttpLookupEmployees(HttpClient client, IOptions<ApiOptions> config)
        {
            _client = client;
            var url = config.Value.EmployeeApiUrl;
            _client.BaseAddress = new Uri(url);
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<bool> CheckEmployeeIsActive(int id  )
        {
            var request = new HttpRequestMessage(HttpMethod.Head, "employees/" + id);
            var response = await _client.SendAsync(request);
            
            return response.IsSuccessStatusCode;
        }
    }
}
