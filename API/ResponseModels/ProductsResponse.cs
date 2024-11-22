using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace PlayField.API.ResponseModels
{
    public class ProductsResponse
    {

        [Key]
        public int ProductoId { get; set; }
        public required string Nombre { get; set; }
        public required string Descripcion { get; set; }
        public string? ImagenProducto { get; set; }
        public decimal PrecioCompra { get; set; }
        public decimal PrecioVenta { get; set; }
        public int CategoriaId { get; set; }

    }
}