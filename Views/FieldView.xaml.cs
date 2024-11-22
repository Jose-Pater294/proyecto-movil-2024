using PlayField.API;
using Newtonsoft.Json;
using PlayField.API.ResponseModels;
using System.Text;
using Plugin.Maui.Calendar.Models;

namespace PlayField.Views;

public partial class FieldView : ContentPage
{
    public Dictionary<DateTime, List<EventModel>> Events { get; set; } = new Dictionary<DateTime, List<EventModel>>();
    private readonly RestService _restService;
    private readonly HttpClient _client = new HttpClient();
    private int _fieldId;

    public FieldView(int fieldId)
    {
        InitializeComponent();
        _fieldId = fieldId;
        _restService = new RestService();
        ReserveDate.MinimumDate = DateTime.Now;
        ReserveHour.Time = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
        bookingCalendar.Culture = new System.Globalization.CultureInfo("es");
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await InitializeTableDetails(_fieldId);
        await LoadBookingsAsync(_fieldId);
    }

    private async Task InitializeTableDetails(int fieldId)
    {
        try
        {
            string response = await _restService.GetResource(Constants.BaseUrl + Constants.Fields + fieldId);
            FieldsResponse field = JsonConvert.DeserializeObject<FieldsResponse>(response)!;

            CanchaTitle.Text = "Cancha #" + field.CanchaId;
            CanchaId.Text = field.CanchaId.ToString();

        }
        catch (Exception ex)
        {
            await DisplayAlert("Error al obtener los datos", ex.Message, "Cerrar");
        }
    }

    private async Task LoadBookingsAsync(int fieldId)
    {
        try
        {
            string response = await _client.GetStringAsync(Constants.BaseUrl + "Reservas/cancha/" + fieldId);

            var bookings = JsonConvert.DeserializeObject<List<BookingsResponse>>(response);

            EventCollection eventCollection = new EventCollection();

            foreach (var booking in bookings)
            {
                DateTime bookingDate = DateTime.Parse(booking.FechaFormateada);

                if (eventCollection.ContainsKey(bookingDate))
                {
                    var existingEvents = new List<EventModel>((IEnumerable<EventModel>)eventCollection[bookingDate]);
                    existingEvents.Add(new EventModel { Name = "Cancha ocupada a las ", Hour = booking.Hora });

                    // Reasignar la nueva lista al diccionario
                    eventCollection[bookingDate] = existingEvents;
                }
                else
                {
                    // Si no hay eventos para esa fecha, crea una nueva lista y agrega el evento
                    eventCollection[bookingDate] = new List<EventModel>
        {
            new EventModel { Name = "Cancha ocupada a las ", Hour = booking.Hora }
        };
                }
            }


            bookingCalendar.Events = eventCollection;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error al obtener las reservas en el calendario", ex.Message, "Cerrar");
        }
    }



    private void GoBack(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }

    private async void Reserve(object sender, EventArgs e)
    {
        try
        {

            // Construir el JSON para Reserva

            var json = new StringContent(JsonConvert.SerializeObject(new
            {
                UsuarioId = App.UserID,
                CanchaId = int.Parse(CanchaId.Text),
                Fecha = ReserveDate.Date,
                Hora = ReserveHour.Time,
                Estado = "Pendiente"
            }), Encoding.UTF8, "application/json");

            // Enviar el JSON de Reserva a la bd mediante la API

            var AddBookingResponse = await _client.PostAsync(Constants.BaseUrl + Constants.AddBooking, json);


            // Construir el JSON para Factura

            var jsonBill = new StringContent(JsonConvert.SerializeObject(new
            {
                UsuarioId = App.UserID,
                CanchaId = int.Parse(CanchaId.Text),
                Estado = "Pendiente",
                Impuestos = 1.19,
                Total = 0,
                Fecha = DateTime.Now,
            }), Encoding.UTF8, "application/json");

            // Enviar el JSON de Reserva a la bd mediante la API

            var AddBillResponse = await _client.PostAsync(Constants.BaseUrl + Constants.Bill, jsonBill);


            if (AddBillResponse.IsSuccessStatusCode)
            {
                Extra.ShowToast("Factura agregada exitosamente");
            }
            else
            {
                await DisplayAlert("Error", "Error al agregar la factura", "Ok");
            }

            if (AddBookingResponse.IsSuccessStatusCode)
            {
                Extra.ShowToast($"Reserva agregada exitosamente para el {ReserveDate.Date} a las {ReserveHour.Time}.");
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Error", "Error al agregar la reserva", "Ok");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "Ok");
        }
    }

    public class EventModel
    {
        public required string Name { get; set; }
        public TimeSpan Hour { get; set; }
    }
}