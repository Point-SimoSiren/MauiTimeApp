using Newtonsoft.Json;
using System.Collections.ObjectModel;
using WorkBackendApi.Models;

namespace MauiTimeApp;

public partial class EmployeePage : ContentPage
{
    // Muuttujan alustaminen
    ObservableCollection<Employee>? ocEmployees = new ObservableCollection<Employee>();

    public EmployeePage()
	{
		InitializeComponent();

        // Kutsutaan latausmetodia heti alussa
        LoadDataFromRestAPI();

        // Annetaan ilmoitus latauksesta
        emp_lataus.Text = "Ladataan työntekijöitä...";
	}
    
    // Latausmetodi
    private async void LoadDataFromRestAPI()
    {
        try
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri("https://workbackendapi.azurewebsites.net/");
            string json = await client.GetStringAsync("api/employees");

                // Deserialisoidaan json muotoinen data C# muotoiseksi
                IEnumerable<Employee>? employees = JsonConvert.DeserializeObject<Employee[]>(json);

                // dataa -niminen observableCollection on alustettukin jo ylhäällä päätasolla että hakutoiminto,
                // pääsee siihen käsiksi.
                if (employees != null)
                ocEmployees = new ObservableCollection<Employee>(employees);

                // Asetetaan datat näkyviin xaml tiedostossa olevalle listalle
                employeeList.ItemsSource = ocEmployees;

                // Tyhjennetään latausilmoitus label
                emp_lataus.Text = "";
            
        }

        catch (Exception e)
        {
            await DisplayAlert("Virhe", e.Message.ToString(), "SELVÄ!");

        }
    }

    // Hakutoiminto
    private void OnSearchBarTextChanged(object sender, EventArgs args)
    {
        // SearchBar searchBar = (SearchBar)sender; alla sama tietotyyppi "cast" eri syntaksilla
        SearchBar searchBar = sender as SearchBar;

        string searchText = searchBar.Text;

        // Työntekijälistaukseen valitaan nyt vain ne joiden etu- tai sukunimeen sisältyy annettu hakutermi
        // "var ocemployees" on tiedoston päätasolla alustettu muuttuja, johon sijoitettiin alussa koko lista työntekijöistä.
        // Nyt siihen sijoitetaan vain hakuehdon täyttävät työntekijät
        employeeList.ItemsSource = ocEmployees.Where(emp => emp.LastName.ToLower().Contains(searchText.ToLower())
        || emp.FirstName.ToLower().Contains(searchText.ToLower()));

    }

    async private void navibutton_Clicked(object sender, EventArgs e)
    {

        Employee emp = (Employee)employeeList.SelectedItem;

        if (emp == null)
        {
            await DisplayAlert("Valinta puuttuu", "Valitse työntekijä.", "OK"); // (otsikko, teksti, kuittausnapin teksti)
            return;
        }
        else
        {

            //int id = emp.IdEmployee;
            await Navigation.PushAsync(new WaPage(emp)); // Navigoidaan uudelle sivulle
        }
    }
}