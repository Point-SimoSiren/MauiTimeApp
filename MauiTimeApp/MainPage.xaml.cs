namespace MauiTimeApp
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
        }

        private void nappi_Clicked(object sender, EventArgs e)
        {
            tervehdys.Text = "Moikka!";
            nappi.BackgroundColor = Colors.YellowGreen;
            sivu.Background = Colors.LightBlue;
            nappi.RotateTo(3080, 3000);
            nappi.Rotation = 0;
            nappi.TranslateTo(0, 900, 3000);    // Move image left
            


        }
    }

}
