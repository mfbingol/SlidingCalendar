using System.ComponentModel;
using Xamarin.Forms;
using SlidingCalendar.CustomControls;

namespace SlidingCalendar
{
  
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void SlidingCalendar_DayItem_Tapped(object sender, SlidingEventArgs e)
        {
            LabelSelectedDate.Text = e.SelectedDate.ToString("dd MMMM yyyy");
        }

        private void Button_Clicked(object sender, System.EventArgs e)
        {
            SlidingCalendar.Theme = (Themes)((Button)sender).CommandParameter;
        }

    }
}
