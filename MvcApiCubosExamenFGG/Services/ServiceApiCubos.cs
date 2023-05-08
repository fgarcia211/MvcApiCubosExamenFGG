using ApiCubosExamenFGG.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace MvcApiCubosExamenFGG.Services
{
    public class ServiceApiCubos
    {
        private MediaTypeWithQualityHeaderValue Header;
        private string UrlApiCubos;

        public ServiceApiCubos(IConfiguration configuration)
        {
            this.UrlApiCubos = configuration.GetValue<string>("ApiUrls:ApiOAuthCubos");
            this.Header = new MediaTypeWithQualityHeaderValue("application/json");
        }

        private async Task<T> CallApiAsync<T>(string request)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApiCubos);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                HttpResponseMessage response =
                    await client.GetAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    T data = await response.Content.ReadAsAsync<T>();
                    return data;
                }
                else
                {
                    return default(T);
                }
            }
        }

        private async Task<T> CallApiAsync<T>
            (string request, string token)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApiCubos);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Add
                    ("Authorization", "bearer " + token);
                HttpResponseMessage response =
                    await client.GetAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    T data = await response.Content.ReadAsAsync<T>();
                    return data;
                }
                else
                {
                    return default(T);
                }
            }
        }

        #region METODOSCUBOS

        public async Task<List<Cubo>> GetAllCubosAPI()
        {
            string request = "/api/Cubos";
            return await this.CallApiAsync<List<Cubo>>(request);
        }

        public async Task<List<Cubo>> GetCubosMarcaAPI(string marca)
        {
            string request = "/api/Cubos/GetCubosMarca/" + marca;
            return await this.CallApiAsync<List<Cubo>>(request);
        }

        public async Task CreateCuboAPI(Cubo cubo)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "/api/Cubos";
                client.BaseAddress = new Uri(this.UrlApiCubos);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);

                string json = JsonConvert.SerializeObject(cubo);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(request, content);
            }
        }

        #endregion

        #region METODOSUSUARIO

        public async Task<UsuarioCubo> PerfilUsuarioAPI(string token)
        {
            string request = "/api/UsuariosCubo/PerfilUsuario";
            return await this.CallApiAsync<UsuarioCubo>(request, token);
        }

        public async Task CreateUsuarioAPI(UsuarioCubo user)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "/api/UsuariosCubo";
                client.BaseAddress = new Uri(this.UrlApiCubos);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);

                string json = JsonConvert.SerializeObject(user);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(request, content);
            }
        }

        #endregion

        #region METODOSPEDIDO

        public async Task<List<CompraCubo>> ComprasUsuarioAPI(string token)
        {
            string request = "/api/ComprasCubo/ComprasUsuario";
            return await this.CallApiAsync<List<CompraCubo>>(request, token);
        }

        public async Task CreatePedido(int idcubo, string token)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "/api/ComprasCubo/" + idcubo;
                client.BaseAddress = new Uri(this.UrlApiCubos);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);
                HttpResponseMessage response = await client.PostAsync(request, null);
            }
        }

        #endregion

        #region METODOSLOGIN

        public async Task<string> GetTokenAsync(string email, string pass)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "/api/auth/login";
                client.BaseAddress = new Uri(this.UrlApiCubos);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);

                LoginModel model = new LoginModel
                {
                    Email = email,
                    Pass = pass
                };

                string jsonModel = JsonConvert.SerializeObject(model);
                StringContent content = new StringContent(jsonModel, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(request, content);

                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    JObject jsonObject = JObject.Parse(data);
                    string token = jsonObject.GetValue("response").ToString();
                    return token;

                    //SE GUARDA EN EL CONTROLLER DEL LOGIN, EN SESION
                }
                else
                {
                    return null;
                }
            }
        }
        #endregion
    }
}
