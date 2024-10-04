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
        emp_lataus.Text = "Ladataan ty�ntekij�it�...";
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

                // dataa -niminen observableCollection on alustettukin jo ylh��ll� p��tasolla ett� hakutoiminto,
                // p��see siihen k�siksi.
                if (employees != null)
                ocEmployees = new ObservableCollection<Employee>(employees);

                // Asetetaan datat n�kyviin xaml tiedostossa olevalle listalle
                employeeList.ItemsSource = ocEmployees;

                // Tyhjennet��n latausilmoitus label
                emp_lataus.Text = "";
            
        }

        catch (Exception e)
        {
            await DisplayAlert("Virhe", e.Message.ToString(), "SELV�!");

        }
    }

    // Hakutoiminto
    private void OnSearchBarTextChanged(object sender, EventArgs args)
    {
        // SearchBar searchBar = (SearchBar)sender; alla sama tietotyyppi "cast" eri syntaksilla
        SearchBar searchBar = sender as SearchBar;

        string searchText = searchBar.Text;

        // Ty�ntekij�listaukseen valitaan nyt vain ne joiden etu- tai sukunimeen sis�ltyy annettu hakutermi
        // "var ocemployees" on tiedoston p��tasolla alustettu muuttuja, johon sijoitettiin alussa koko lista ty�ntekij�ist�.
        // Nyt siihen sijoitetaan vain hakuehdon t�ytt�v�t ty�ntekij�t
        employeeList.ItemsSource = ocEmployees.Where(emp => emp.LastName.ToLower().Contains(searchText.ToLower())
        || emp.FirstName.ToLower().Contains(searchText.ToLower()));

    }

    async private void navibutton_Clicked(object sender, EventArgs e)
    {

        Employee emp = (Employee)employeeList.SelectedItem;

        if (emp == null)
        {
            await DisplayAlert("Valinta puuttuu", "Valitse ty�ntekij�.", "OK"); // (otsikko, teksti, kuittausnapin teksti)
            return;
        }
        else
        {

            //int id = emp.IdEmployee;
            await Navigation.PushAsync(new WaPage(emp)); // Navigoidaan uudelle sivulle
        }
    }
}