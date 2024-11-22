using PlayField.API;
using PlayField.API.ResponseModels;
using Newtonsoft.Json;
using PlayField.Local;

namespace PlayField.Views;

public partial class Account : ContentPage
{
    private readonly RestService _restService;
    private readonly LocalDbService _localDbService;

    public Account()
    {
        InitializeComponent();

        _restService = new RestService();
        _localDbService = new LocalDbService();

        // Verificar detalles del usuario logueado al aparecer
        this.Appearing += async (sender, e) =>
        {
            try
            {
                // Obtener detalles del usuario logueado desde la API
                string response = await _restService.GetResource(Constants.BaseUrl + Constants.Users + App.UserID);

                if (string.IsNullOrEmpty(response))
                {
                    await DisplayAlert("Error", "No se pudo obtener la información del usuario. Intente nuevamente.", "OK");
                    return;
                }

                UsersResponse user = JsonConvert.DeserializeObject<UsersResponse>(response);

                if (user == null)
                {
                    await DisplayAlert("Error", "Los datos del usuario no son válidos.", "OK");
                    return;
                }

                // Actualizar la interfaz con los datos del usuario
                FullName.Text = $"{user.nombre} {user.apellidos}";
                name.Text = user.nombre ?? "N/A";
                lastName.Text = user.apellidos ?? "N/A";
                id.Text = user.usuarioId.ToString();
                email1.Text = user.email ?? "N/A";
                email2.Text = user.email ?? "N/A";
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Error al cargar los datos: " + ex.Message, "OK");
            }
        };
    }

    private async void Logout(object sender, EventArgs e)
    {
        try
        {
            // Eliminar la sesión del usuario de la base de datos local
            await _localDbService.LogoutUser();

            // Reemplazar la pila de navegación con LoginView
            Application.Current.MainPage = new NavigationPage(new LoginView());
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "No se pudo cerrar sesión: " + ex.Message, "OK");
        }
    }


}
