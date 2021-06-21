using Newtonsoft.Json;
using ParkyWeb.Repository.IRepository;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ParkyWeb.Repository
{
    /// <summary>
    /// Generic call API
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly IHttpClientFactory _clientFactory;

        public Repository(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        /// <summary>
        /// generic API call to create
        /// </summary>
        /// <param name="url">api url</param>
        /// <param name="objToCreate">object data to create</param>
        /// <param name="token">token</param>
        /// <returns></returns>
        public async Task<bool> CreateAsync(string url, T objToCreate, string token="")
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            if(objToCreate!=null)
            {
                request.Content = new StringContent(
                    JsonConvert.SerializeObject(objToCreate), Encoding.UTF8, "application/json");
            }
            else
            {
                return false;
            }
            //Create user
            var client = _clientFactory.CreateClient();
            //check if token exsit
            if (token != null && token.Length != 0)
            {
                //Add Authorization of client
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            
            HttpResponseMessage response = await client.SendAsync(request);
            if (response.StatusCode == HttpStatusCode.Created)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// generic API call to delete
        /// </summary>
        /// <param name="url">url api</param>
        /// <param name="Id">id value to remove</param>
        /// <param name="token">token</param>
        /// <returns></returns>
        public async Task<bool> DeleteAsync(string url, int Id, string token="")
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, url+Id);

            var client = _clientFactory.CreateClient();
            if (token != null && token.Length != 0)
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            HttpResponseMessage response = await client.SendAsync(request);
            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// generic API call to get all
        /// </summary>
        /// <param name="url">url api</param>
        /// <param name="token">token</param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> GetAllAsync(string url,string token="")
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            var client = _clientFactory.CreateClient();
            if (token!=null &&  token.Length != 0)
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            HttpResponseMessage response = await client.SendAsync(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<T>>(jsonString);
            }

            return null;
        }

        /// <summary>
        /// generic API call to get value with parameter
        /// </summary>
        /// <param name="url"></param>
        /// <param name="Id"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<T> GetAsync(string url, int Id, string token="")
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url+Id);

            var client = _clientFactory.CreateClient();
            if (token != null && token.Length != 0)
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            HttpResponseMessage response = await client.SendAsync(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(jsonString);
            }

            return null;
        }

        /// <summary>
        /// generic API call to update
        /// </summary>
        /// <param name="url"></param>
        /// <param name="objToUpdate"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<bool> UpdateAsync(string url, T objToUpdate,string token="")
        {
            var request = new HttpRequestMessage(HttpMethod.Patch, url);
            if (objToUpdate != null)
            {
                request.Content = new StringContent(
                    JsonConvert.SerializeObject(objToUpdate), Encoding.UTF8, "application/json");
            }
            else
            {
                return false;
            }

            var client = _clientFactory.CreateClient();
            if (token != null && token.Length != 0)
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            HttpResponseMessage response = await client.SendAsync(request);
            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
