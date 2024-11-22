using SQLite;
using LocalUserSession = PlayField.Local.UserSession; // Alias para el modelo de sesión de usuario local

namespace PlayField.Local
{
    public class LocalDbService
    {
        private const string DB_NAME = "PlayField.db";
        private readonly SQLiteAsyncConnection _connection;

        public LocalDbService()
        {
            _connection = new SQLiteAsyncConnection(Path.Combine(FileSystem.AppDataDirectory, DB_NAME));

            // Crear la tabla UserSession si no existe
            try
            {
                _connection.CreateTableAsync<LocalUserSession>().Wait(); // Usa el alias para evitar ambigüedades
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear la tabla: {ex.Message}");
            }
        }

        // Gestión de sesión del usuario
        public async Task<LocalUserSession?> GetUserSession()
        {
            try
            {
                // Recupera la sesión actual del usuario
                return await _connection.Table<LocalUserSession>().FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al recuperar la sesión: {ex.Message}");
                return null;
            }
        }

        public async Task SaveUserSession(int userId, bool isLoggedIn)
        {
            try
            {
                // Elimina cualquier sesión previa y guarda la nueva
                await _connection.DeleteAllAsync<LocalUserSession>();
                var session = new LocalUserSession
                {
                    UserSessionId = userId,  // Usar nombres únicos
                    SessionActive = isLoggedIn // Usar nombres únicos
                };
                await _connection.InsertAsync(session);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al guardar la sesión: {ex.Message}");
            }
        }

        public async Task LogoutUser()
        {
            try
            {
                // Cierra la sesión eliminando los datos del usuario
                await _connection.DeleteAllAsync<LocalUserSession>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cerrar sesión: {ex.Message}");
            }
        }
    }

    // Clase simplificada para usuario y sesión
    [Table("userSession")]
    public class UserSession
    {
        [PrimaryKey]
        [Column("userSessionId")]
        public int UserSessionId { get; set; } // Identificador único de la sesión

        [Column("sessionActive")]
        public bool SessionActive { get; set; } // Indica si la sesión está activa
    }
}
