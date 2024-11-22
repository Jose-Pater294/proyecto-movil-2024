using Newtonsoft.Json;
using PlayField.API.ResponseModels;
using System.Text;

namespace PlayField.API
{
    public class Auth
    {
        private static readonly HttpClient client = new HttpClient();

        public async Task<LoginResponse> AuthorizeAsync(string email, string password)
        {
            // Crear el objeto con los datos de login
            var loginData = new
            {
                email = email,
                clave = password
            };

            // Serializar el objeto a JSON
            string jsonContent = JsonConvert.SerializeObject(loginData);

            // Crear el contenido de la solicitud
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Realizar la solicitud POST
            HttpResponseMessage response = await client.PostAsync(Constants.BaseUrl + Constants.Login, content);

            if (response.IsSuccessStatusCode)
            {
                // Deserializar y retornar la respuesta
                string jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<LoginResponse>(jsonResponse);
            }
            else
            {
                return new LoginResponse { Message = "Hubo un fallo al procesar la información." };
            }
        }
    }
}
