namespace PlayField.API.ResponseModels
{
    public class UsersResponse
    {
        public int usuarioId { get; set; }
        public required string nombre { get; set; }
        public required string apellidos { get; set; }
        public required string email { get; set; }
        public required string clave { get; set; }
        public int rolId { get; set; }
        public required string Foto { get; set; }
    }
}
