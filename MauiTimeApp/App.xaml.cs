namespace MauiTimeApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            //MainPage = new MainPage();
            // MainPage = new TabbedEsimerkki();
            MainPage = new NavigationPage(new EmployeePage());

        }
    }
}
