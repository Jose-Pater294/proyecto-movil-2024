using Newtonsoft.Json;
using PlayField.API;
using System.Text;

namespace PlayField;

public partial class RegisterView : ContentPage
{
    private static readonly HttpClient client = new HttpClient();

    public RegisterView()
    {
        InitializeComponent();
    }

    private void BackToLogin(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }

    private async void Register(object sender, EventArgs e)
    {
        Loader.IsVisible = true;

        try
        {
            if (Password.Text != passwordVerification.Text)
            {
                await DisplayAlert("Error", "Las contraseñas no coinciden", "Ok");
                return;
            }

            if (string.IsNullOrEmpty(Id.Text) || string.IsNullOrEmpty(Name.Text) || string.IsNullOrEmpty(LastName.Text) || string.IsNullOrEmpty(Email.Text) || string.IsNullOrEmpty(Password.Text))
            {
                await DisplayAlert("Error", "Por favor, llene todos los campos", "Ok");
                return;
            }

            StringContent newUser = new StringContent(JsonConvert.SerializeObject(new
            {
                usuarioId = Id.Text,
                nombre = Name.Text,
                apellidos = LastName.Text,
                email = Email.Text,
                clave = Password.Text
            }), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(Constants.BaseUrl + Constants.Users, newUser);

            if (response.IsSuccessStatusCode)
            {
                await DisplayAlert("Success", $"Bienvenido, {Name.Text}, su registro se efectuó de manera satisfactoria.", "Ok");
            }
            else
            {
                await DisplayAlert("Error", $"Error al registrar el usuario: {response.StatusCode}", "Ok");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "Ok");
        }
        finally
        {
            Loader.IsVisible = false;
        }
    }
}
