namespace PlayField.API.ResponseModels
{
    public class BookingsResponse
    {
        public int ReservaId { get; set; }
        public int UsuarioId { get; set; }
        public int CanchaId { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan Hora { get; set; }
        public required string Estado { get; set; }
        public required string ColorEstado { get; set; }
        public required string ImagenCancha { get; set; }
        public required string TipoCancha { get; set; }
        public string FechaFormateada => Fecha.ToString("yyyy/MM/dd");
        public bool CancelButtonVisibility { get; set; } = true;
    }
}
