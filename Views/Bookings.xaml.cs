using Newtonsoft.Json;
using PlayField.API;
using PlayField.API.ResponseModels;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace PlayField.Views;

public partial class Bookings : ContentPage, INotifyPropertyChanged
{
    private readonly RestService _restService;
    private readonly HttpClient _client;

    private ObservableCollection<BookingsResponse> _bookingsCollection;
    public ObservableCollection<BookingsResponse> BookingsCollection
    {
        get => _bookingsCollection;
        set
        {
            if (_bookingsCollection != value)
            {
                _bookingsCollection = value;
                OnPropertyChanged();
            }
        }
    }

    public Bookings()
    {
        InitializeComponent();
        _restService = new RestService();
        _client = new HttpClient();
        BookingsCollection = new ObservableCollection<BookingsResponse>();

        BindingContext = this;

        this.Appearing += async (sender, e) =>
        {
            await GetBookings();
        };
    }

    private void CancelBooking(object sender, EventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(async () => {
            bool result = await DisplayAlert("Confirmación", "¿Realmente deseas cancelar la reserva?", "Sí", "No");

            if (result)
            {
                var button = sender as Button;
                BookingsResponse? booking = button?.CommandParameter as BookingsResponse;

                if (booking != null)
                {
                    try
                    {
                        var json = new StringContent(JsonConvert.SerializeObject(new
                        {
                            Estado = "Cancelada"
                        }), Encoding.UTF8, "application/json");

                        HttpResponseMessage response = await _client.PutAsync(Constants.BaseUrl + Constants.AddBooking + booking.ReservaId, json);

                        if (response.IsSuccessStatusCode)
                        {
                            Extra.ShowToast("Reserva cancelada");
                            await GetBookings();
                        }
                        else
                        {
                            await DisplayAlert("Error", "No se ha podido cancelar la reserva", "OK");
                        }
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Error", ex.Message, "OK");
                    }
                }
                else
                {
                    await DisplayAlert("Error", "No se pudo obtener la información de la reserva a cancelar.", "OK");
                }
            }
        });
    }

    private async Task GetBookings()
    {
        Loader.IsVisible = true;

        try
        {
            string response = await _restService.GetResource(Constants.BaseUrl + Constants.Bookings + App.UserID);

            if (string.IsNullOrEmpty(response))
            {
                noBookings.IsVisible = true;
                return;
            }

            List<BookingsResponse> bookings = JsonConvert.DeserializeObject<List<BookingsResponse>>(response)!;

            if (bookings == null || !bookings.Any())
            {
                noBookings.IsVisible = true;
                return;
            }

            foreach (var booking in bookings)
            {
                if (booking?.Estado == "Cancelada" || booking?.Estado == "Finalizada")
                {
                    booking.CancelButtonVisibility = false;
                }

                // Obtener detalles de la cancha
                string responseTipoCancha = await _restService.GetResource(Constants.BaseUrl + Constants.Fields + booking.CanchaId);
                if (!string.IsNullOrEmpty(responseTipoCancha))
                {
                    FieldsResponse fieldsResponse = JsonConvert.DeserializeObject<FieldsResponse>(responseTipoCancha)!;
                    if (fieldsResponse != null)
                    {
                        booking.TipoCancha = fieldsResponse.Tipo;

                        
                    }
                }
            }

            noBookings.IsVisible = false;
            BookingsCollection = new ObservableCollection<BookingsResponse>(
                bookings.Where(b => b != null && b.Estado != "Cancelada").OrderByDescending(x => x.Estado == "Pendiente")
            );
        }
        catch (Exception ex)
        {
            noBookings.IsVisible = true;
            await DisplayAlert("Error", ex.Message, "OK");
        }
        finally
        {
            Loader.IsVisible = false;
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
