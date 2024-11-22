using Newtonsoft.Json;
using PlayField.API.ResponseModels;
using PlayField.API;
using System.Collections.ObjectModel;

namespace PlayField.Views;

public partial class Home : ContentPage
{
    private readonly RestService _restService;
    public Home()
	{
		InitializeComponent();
        _restService = new RestService();

        this.Appearing += async (sender, e) =>
        {
            Loader.IsVisible = true;
            try
            {
                string response = await _restService.GetResource(Constants.BaseUrl + Constants.Fields);

                List<FieldsResponse> fields = JsonConvert.DeserializeObject<List<FieldsResponse>>(response)!;


                ObservableCollection<FieldsResponse> fieldsCollection = new ObservableCollection<FieldsResponse>(fields);

                BindingContext = fieldsCollection;

                Loader.IsVisible = false;


                string nameResponse = await _restService.GetResource(Constants.BaseUrl + Constants.Users + App.UserID);
                UsersResponse user = JsonConvert.DeserializeObject<UsersResponse>(nameResponse)!;
                WelcomeName.Text = "Bienvenido, " + user.nombre + ".";
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        };
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

    private async void OpenFieldView(object sender, EventArgs e)
    {
        ImageButton? button = sender as ImageButton;
        int? fieldId = button?.CommandParameter as int?;

        if (fieldId.HasValue)
        {
            await Navigation.PushAsync(new FieldView(fieldId.Value));
        }
        else
        {
            await DisplayAlert("Error", "CanchaId no válido", "OK");
        }
    }
}