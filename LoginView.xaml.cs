using PlayField.API;
using PlayField.API.ResponseModels;
using PlayField.Views;
using PlayField.Local;

namespace PlayField
{
    public partial class LoginView : ContentPage
    {
        private readonly LocalDbService _localDbService;
        private readonly RestService _restService;
        Auth authService = new Auth();

        public LoginView()
        {
            InitializeComponent();
            _localDbService = new LocalDbService();
            _restService = new RestService();
            CheckExistingSession(); // Verificar si ya existe una sesión activa
        }

        private async void CheckExistingSession()
        {
            try
            {
                var session = await _localDbService.GetUserSession();
                if (session != null && session.SessionActive) // Cambiado a SessionActive
                {
                    App.UserID = session.UserSessionId; // Cambiado a UserSessionId
                    await Navigation.PushAsync(new Menu()); // Redirigir al menú principal
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Error al verificar la sesión: " + ex.Message, "OK");
            }
        }

        private async void Login(object sender, EventArgs e)
        {
            Loader.IsVisible = true;

            try
            {
                if (string.IsNullOrEmpty(Email.Text) || string.IsNullOrEmpty(Password.Text))
                {
                    await DisplayAlert("Campos vacíos", "Por favor, rellene todos los campos", "OK");
                    Loader.IsVisible = false;
                    return;
                }

                LoginResponse result = await authService.AuthorizeAsync(Email.Text, Password.Text);

                if (result != null && result.Message == "Login successful")
                {
                    // Guardar la sesión en la base de datos local
                    await _localDbService.SaveUserSession(result.UserId, true);

                    // Actualizar el ID del usuario en la aplicación
                    App.UserID = result.UserId;

                    // Redirigir al menú principal
                    await Navigation.PushAsync(new Menu());
                }
                else
                {
                    Extra.ShowToast("Email o contraseña incorrectos.");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error interno", ex.Message, "OK");
            }
            finally
            {
                Loader.IsVisible = false;
            }
        }

        private void OpenRegisterView(object sender, EventArgs e)
        {
            Navigation.PushAsync(new RegisterView());
        }

        protected override bool OnBackButtonPressed()
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                bool result = await DisplayAlert("Salir", "¿Deseas salir de la aplicación?", "Sí", "No");

                if (result)
                {
                    System.Diagnostics.Process.GetCurrentProcess().Kill();
                }
            });

            return true;
        }
    }
}
