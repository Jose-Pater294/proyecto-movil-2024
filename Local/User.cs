using SQLite;

namespace PlayField.Local
{
    [Table("userSession")] // Nombre de la tabla en la base de datos
    public class User
    {
        [PrimaryKey]
        [Column("idUsuario")]
        public int IdUsuario { get; set; } // Identificador único del usuario

        [Column("isLoggedIn")]
        public bool IsLoggedIn { get; set; } // Indica si el usuario está logueado
    }
}
