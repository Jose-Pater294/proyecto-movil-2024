namespace PlayField.API
{
    public class RestService
    {
        private readonly HttpClient _client;

        public RestService()
        {
            _client = new HttpClient();
        }

        public async Task<string> GetResource(string resourceUrl)
        {
            // Aquí se realiza la petición y se guarda en una variable
            
            HttpResponseMessage response = await _client.GetAsync(resourceUrl);

            // Condicional para verificar que la petición se ejecutó exitosamente

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                throw new Exception($"Error al obtener la respuesta: {response.StatusCode}");
            }
        }
    }
}
