using MauiTimeApp.Models;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Security.Cryptography;
using System.Text;
using WorkBackendApi.Models;

namespace MauiTimeApp;

public partial class WaPage : ContentPage
{

    // Muuttujan alustaminen: aliasnimi ocWa observable coll... WorkA...
    ObservableCollection<WorkAssignment>? ocWa;

    int eId;
    string lat;
    string lon;
    // MAUI Geolocation dokumentaation ohjeen mukaan laitettu:
    private CancellationTokenSource _cancelTokenSource;
    private bool _isCheckingLocation;


    public WaPage(Employee emp)
	{
		InitializeComponent();

        eId = emp.IdEmployee;

        // Kutsutaan metodeita heti alussa
        LoadDataFromRestAPI();
        GetCurrentLocation();


        // Annetaan ilmoitus latauksesta
        wa_lataus.Text = "Ladataan ty�teht�vi�...";
        lon_label.Text = "Lataan sijaintia...";
    }

    // Sijainnin haku
    public async Task GetCurrentLocation()
    {
        try
        {
            _isCheckingLocation = true;

            GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));

            _cancelTokenSource = new CancellationTokenSource();

            Location location = await Geolocation.Default.GetLocationAsync(request, _cancelTokenSource.Token);

            if (location != null)
            {

                lat = location.Latitude.ToString();
                lon = location.Longitude.ToString();

                lat_label.Text = $"Latitude: {location.Latitude}";
                lon_label.Text = $"Longitude: {location.Longitude}";
            }
        }
       
        catch (Exception ex)
        {
            // Unable to get location
        }
        finally
        {
            _isCheckingLocation = false;
        }
    }



    // Latausmetodi
    private async void LoadDataFromRestAPI()
    {
        try
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri("https://workbackendapi.azurewebsites.net/");
            string json = await client.GetStringAsync("api/workassignments");

            // Deserialisoidaan json muotoinen data C# muotoiseksi
            IEnumerable<WorkAssignment>? wa = JsonConvert.DeserializeObject<WorkAssignment[]>(json);


            // observableCollection on alustettukin jo ylh��ll� p��tasolla
            if (wa != null)
                ocWa = new ObservableCollection<WorkAssignment>(wa);

            // Asetetaan datat n�kyviin xaml tiedostossa olevalle listalle
            waList.ItemsSource = ocWa;

            // Tyhjennet��n latausilmoitus label
            wa_lataus.Text = "";

        }

        catch (Exception e)
        {
            await DisplayAlert("Virhe", e.Message.ToString(), "SELV�!");

        }
    }


    //----------------START--------------------------------------------------

    private async void startbutton_Clicked(object sender, EventArgs e)
    {
        WorkAssignment wa = (WorkAssignment)waList.SelectedItem;

        if (wa == null)
        {
            await DisplayAlert("Valinta puuttuu", "Valitse ty�teht�v�.", "OK");
            return;
        }

        // Pyydet��n kommenttia
        string answer = await DisplayPromptAsync("Palaute", "Voit j�tt�� nyt kommentin halutessasi", "Valmis");
        if (string.IsNullOrEmpty(answer))
        {
            answer = "Aloitettu";
        }

        // Luodaan operation luokan instanssi eli objekti joka sis�lt�� start metodissa v�litett�v�n datan
        var op = new Operation();

        op.EmployeeID = eId;
        op.WorkAssignmentID = wa.IdWorkAssignment;
        op.OperationType = "start";
        op.Comment = answer;
        op.Latitude = lat;
        op.Longitude = lon;

        // Alustetaan http luokan instanssi
        HttpClient client = new HttpClient();

        client.BaseAddress = new Uri("https://workbackendapi.azurewebsites.net/");

        // Muutetaan em. data objekti Jsoniksi
        var input = JsonConvert.SerializeObject(op);

        HttpContent content = new StringContent(input, Encoding.UTF8, "application/json");

        // L�hetet��n serialisoitu objekti back-endiin Post pyynt�n�
        HttpResponseMessage message = await client.PostAsync("/api/workassignments/start", content);

        // Otetaan vastaan palvelimen vastaus
        string reply = await message.Content.ReadAsStringAsync();

        //Asetetaan vastaus de-serialisoituna success muuttujaan
        bool success = JsonConvert.DeserializeObject<bool>(reply);

        // Ilmoitus k�ytt�j�lle aloitetusta ty�st� / virheilmoitus
        if (success == false)
        {
            await DisplayAlert("Ei voida aloittaa", "Ty� on jo k�ynniss�", "OK");
        }
        else if (success == true)
        {
            await DisplayAlert("Ty� aloitettu", "Ty� on aloitettu", "OK");
        }


    }



    //----------------STOP--------------------------------------------------
    private void stopbutton_Clicked(object sender, EventArgs e)
    {

    }
}