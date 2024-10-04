namespace MauiTimeApp;

public partial class TabbedEsimerkki : TabbedPage
{
	public TabbedEsimerkki()
	{
		InitializeComponent();

		olohuone_slider.Minimum = 0;
        olohuone_slider.Maximum = 100;


    }

    private void kytkin_Toggled(object sender, ToggledEventArgs e)
    {
		if (kytkin.IsToggled == true)
		{
			h‰lytys_txt.Text = "P‰‰ll‰";

		}
		else
		{
            h‰lytys_txt.Text = "Pois";
        }
    }
}
