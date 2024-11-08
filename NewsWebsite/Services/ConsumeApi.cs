using NewsWebsite.Helpers;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace NewsWebsite.Services
{
    public interface IConsumeApi
    {
        Task<ResponseData> GetAsync(string endpoint, string accessToken = "");
        Task<ResponseData> GetAsync(string endpoint, Dictionary<string, dynamic> dictPars, string accessToken = "");
        Task<ResponseData> PutAsync(string endpoint, Dictionary<string, dynamic> dictPars, string accessToken = "");
        Task<ResponseData> PostAsync(string endpoint, Dictionary<string, dynamic> dictPars, string accessToken = "");
        Task<ResponseData> DeleteAsync(string endpoint, string accessToken = "");
        Task<ResponseData> DeleteAsync(string endpoint, Dictionary<string, dynamic> dictPars, string accessToken = "");
        Task<ResponseData> PaginationAsync(string endpoint, Dictionary<string, dynamic> dictPars, string accessToken = "");
        Task<ResponseData> LoginAsync(string endpoint, Dictionary<string, dynamic> dictPars);
    }
    public class ConsumeApi : IConsumeApi
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly Uri _baseAddress;
        public ConsumeApi(IConfiguration configuration, IHttpClientFactory clientFactory)
        {
            _configuration = configuration;
            string urlApi = _configuration["ApiSettings:UrlApi"]!;
            _baseAddress = new Uri(urlApi);
            _httpClient = clientFactory.CreateClient();
        }

        public async Task<ResponseData> GetAsync(string endpoint, string accessToken = "")
        {
            ResponseData res = new ResponseData();
            string uriApi = _baseAddress.ToString() + endpoint;

            if (!string.IsNullOrEmpty(accessToken))
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            HttpResponseMessage response = await _httpClient.GetAsync(uriApi);

            if (response.IsSuccessStatusCode)
            {
                var jsonRes = await response.Content.ReadAsStringAsync();
                if (jsonRes != null)
                {
                    res = JsonConvert.DeserializeObject<ResponseData>(jsonRes)!;
                }
            }
            return res;
        }

        public async Task<ResponseData> GetAsync(string endpoint, Dictionary<string, dynamic> dictPars, string accessToken = "")
        {
            ResponseData res = new ResponseData();
            string uriApi = _baseAddress.ToString() + endpoint;
            if (dictPars != null)
            {
                string parameters = "?";
                int i = 0;
                foreach (KeyValuePair<string, dynamic> item in dictPars)
                {
                    parameters += (i == 0 ? "" : "&") + string.Format("{0}={1}", item.Key, item.Value == null ? "" : item.Value.ToString());
                    i++;
                }
                uriApi += parameters;
            }

            if (!string.IsNullOrEmpty(accessToken))
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            HttpResponseMessage response = await _httpClient.GetAsync(uriApi);
            if (response.IsSuccessStatusCode)
            {
                var jsonRes = await response.Content.ReadAsStringAsync();
                if (jsonRes != null)
                {
                    res = JsonConvert.DeserializeObject<ResponseData>(jsonRes)!;
                }
            }
            return res;
        }

        public async Task<ResponseData> PaginationAsync(string endpoint, Dictionary<string, dynamic> dictPars, string accessToken = "")
        {
            ResponseData res = new ResponseData();
            string uriApi = _baseAddress.ToString() + endpoint;
            if (dictPars != null)
            {
                string parameters = "?";
                int i = 0;
                foreach (KeyValuePair<string, dynamic> item in dictPars)
                {
                    parameters += (i == 0 ? "" : "&") + string.Format("{0}={1}", item.Key, item.Value == null ? "" : item.Value.ToString());
                    i++;
                }
                uriApi += parameters;
            }

            if (!string.IsNullOrEmpty(accessToken))
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            HttpResponseMessage response = await _httpClient.GetAsync(uriApi);
            // Phần phân trang lấy StatusCode làm tổng số record
            ///res.statusCode = ((int)response.StatusCode); 
            if (response.IsSuccessStatusCode)
            {
                var jsonRes = await response.Content.ReadAsStringAsync();
                if (jsonRes != null)
                {
                    res = JsonConvert.DeserializeObject<ResponseData>(jsonRes)!;
                }
            }
            return res;
        }

        public async Task<ResponseData> PutAsync(string endpoint, Dictionary<string, dynamic> dictPars, string accessToken = "")
        {
            // Authorization 
            if (!string.IsNullOrEmpty(accessToken))
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            ResponseData res = new ResponseData();
            string uriApi = _baseAddress.ToString() + endpoint;

            // Prepare JSON body
            var jsonBody = JsonConvert.SerializeObject(dictPars);
            StringContent content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            // Send PUT request
            HttpResponseMessage response = await _httpClient.PutAsync(uriApi, content);
            if (response.IsSuccessStatusCode)
            {
                string jsonRes = await response.Content.ReadAsStringAsync();
                if (jsonRes != null)
                {
                    res = JsonConvert.DeserializeObject<ResponseData>(jsonRes)!;
                }
            }
            return res;
        }

        public async Task<ResponseData> PostAsync(string endpoint, Dictionary<string, dynamic> dictPars, string accessToken = "")
        {
            // Authorization 
            if (!string.IsNullOrEmpty(accessToken))
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            ResponseData res = new ResponseData();
            string uriApi = _baseAddress.ToString() + endpoint;

            // Prepare JSON body
            string jsonBody = JsonConvert.SerializeObject(dictPars);
            StringContent content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            // Send PUT request
            HttpResponseMessage response = await _httpClient.PostAsync(uriApi, content);
            if (response.IsSuccessStatusCode)
            {
                var jsonRes = await response.Content.ReadAsStringAsync();
                if (jsonRes != null)
                {
                    res = JsonConvert.DeserializeObject<ResponseData>(jsonRes)!;
                }
            }
            return res;
        }

        public async Task<ResponseData> LoginAsync(string endpoint, Dictionary<string, dynamic> dictPars)
        {
            ResponseData res = new ResponseData();
            string uriApi = _baseAddress.ToString() + endpoint;
            if (dictPars != null)
            {
                string parameters = "?";
                int i = 0;
                foreach (KeyValuePair<string, dynamic> item in dictPars)
                {
                    parameters += (i == 0 ? "" : "&") + string.Format("{0}={1}", item.Key, item.Value == null ? "" : item.Value.ToString());
                    i++;
                }
                uriApi += parameters;
            }

            // Prepare JSON body
            string jsonBody = JsonConvert.SerializeObject(dictPars);
            StringContent content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync(uriApi, content);
            if (response.IsSuccessStatusCode)
            {
                var jsonRes = await response.Content.ReadAsStringAsync();
                if (jsonRes != null)
                {
                    res = JsonConvert.DeserializeObject<ResponseData>(jsonRes)!;
                }
            }
            return res;
        }

        public async Task<ResponseData> DeleteAsync(string endpoint, Dictionary<string, dynamic> dictPars, string accessToken = "")
        {
            // Authorization 
            if (!string.IsNullOrEmpty(accessToken))
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            ResponseData res = new ResponseData();
            string uriApi = _baseAddress.ToString() + endpoint;
            if (dictPars != null)
            {
                string parameters = "?";
                int i = 0;
                foreach (KeyValuePair<string, dynamic> item in dictPars)
                {
                    parameters += (i == 0 ? "" : "&") + string.Format("{0}={1}", item.Key, item.Value == null ? "" : item.Value.ToString());
                    i++;
                }
                uriApi += parameters;
            }
            // Send Delete request
            HttpResponseMessage response = await _httpClient.DeleteAsync(uriApi);
            if (response.IsSuccessStatusCode)
            {
                var jsonRes = await response.Content.ReadAsStringAsync();
                if (jsonRes != null)
                {
                    res = JsonConvert.DeserializeObject<ResponseData>(jsonRes)!;
                }
            }
            return res;
        }

        public async Task<ResponseData> DeleteAsync(string endpoint, string accessToken = "")
        {
            // Authorization 
            if (!string.IsNullOrEmpty(accessToken))
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            ResponseData res = new ResponseData();
            string uriApi = _baseAddress.ToString() + endpoint;
            // Send Delete request
            HttpResponseMessage response = await _httpClient.DeleteAsync(uriApi);
            if (response.IsSuccessStatusCode)
            {
                var jsonRes = await response.Content.ReadAsStringAsync();
                if (jsonRes != null)
                {
                    res = JsonConvert.DeserializeObject<ResponseData>(jsonRes)!;
                }
            }
            return res;
        }
    }
    }
