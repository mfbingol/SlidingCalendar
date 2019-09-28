using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SlidingCalendar.CustomControls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SlidingCalendar : ContentView
    {
        private const string DATE_FORMAT = "dd MMMM yyyy";
        private const string TODAY = "Today";
        private const string BORDERWIDTH = "BorderWidth";
        private const string MAINBACKCOLOR = "MainBackColor";
        private const string SECONDBACKCOLOR = "SecondBackColor";

        public event EventHandler<SlidingEventArgs> DayItem_Tapped;
        public SlidingCalendar()
        {
            InitializeComponent();
        }

        private static BindableProperty ThemeProperty = BindableProperty.Create(
                                                 propertyName: nameof(Theme),
                                                 returnType: typeof(Themes),
                                                 declaringType: typeof(SlidingCalendar),
                                                 defaultValue: Themes.BlueTheme,
                                                 defaultBindingMode: BindingMode.TwoWay,
                                                 propertyChanged: ThemePropertyChanged);
        public Themes Theme
        {
            get { return (Themes)base.GetValue(ThemeProperty); }
            set { base.SetValue(ThemeProperty, value); }
        }

        private static void ThemePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (SlidingCalendar)bindable;

            switch ((Themes)newValue)
            {
                case Themes.BlueTheme:
                    control.Resources[MAINBACKCOLOR] = Color.FromHex("#0072b1");
                    control.Resources[SECONDBACKCOLOR] = Color.FromHex("#006db6");
                    break;
                case Themes.MetalicBlueTheme:
                    control.Resources[MAINBACKCOLOR] = Color.FromHex("#08457e");
                    control.Resources[SECONDBACKCOLOR] = Color.FromHex("#32527b");
                    break;
                case Themes.RedTheme:
                    control.Resources[MAINBACKCOLOR] = Color.FromHex("#ee223e");
                    control.Resources[SECONDBACKCOLOR] = Color.FromHex("#ff334e");
                    break;
                case Themes.GreenTheme:
                    control.Resources[MAINBACKCOLOR] = Color.FromHex("#40826d");
                    control.Resources[SECONDBACKCOLOR] = Color.FromHex("#01796f");
                    break;
                case Themes.DarkTheme:
                    control.Resources[MAINBACKCOLOR] = Color.FromHex("#141414");
                    control.Resources[SECONDBACKCOLOR] = Color.FromHex("#1b1b1b");
                    break;
                default:
                    break;
            }

        }

        private static BindableProperty BorderWidthProperty = BindableProperty.Create(
                                                       propertyName: nameof(BorderWidth),
                                                       returnType: typeof(Thickness),
                                                       declaringType: typeof(SlidingCalendar),
                                                       defaultValue: null,
                                                       defaultBindingMode: BindingMode.TwoWay,
                                                       propertyChanged: BorderWidthPropertyChanged);
        public Thickness BorderWidth
        {
            get { return (Thickness)base.GetValue(BorderWidthProperty); }
            set { base.SetValue(BorderWidthProperty, value); }
        }

        private static void BorderWidthPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (SlidingCalendar)bindable;
            control.Resources[BORDERWIDTH] = (Thickness)newValue;
        }

        private static BindableProperty MonthSizeProperty = BindableProperty.Create(
                                                        propertyName: nameof(MonthSize),
                                                        returnType: typeof(Size),
                                                        declaringType: typeof(SlidingCalendar),
                                                        defaultValue: null,
                                                        defaultBindingMode: BindingMode.TwoWay,
                                                        propertyChanged: MonthSizePropertyChanged);
        public Size MonthSize
        {
            get { return (Size)base.GetValue(MonthSizeProperty); }
            set { base.SetValue(MonthSizeProperty, value); }
        }

        private static void MonthSizePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (SlidingCalendar)bindable;
            BindDays(control, (Size)newValue);
        }

        private static void BindDays(SlidingCalendar control, Size size)
        {
            DateTime DateToday = DateTime.Today;
            var dateList = new List<DateTime>();
            DateTime beginDate = new DateTime(DateToday.Year, DateToday.Month, 1).AddMonths(-(int)size.Height);
            DateTime endDate = new DateTime(DateToday.Year, DateToday.Month, 1).AddMonths((int)size.Width).AddDays(-1);
            int totalDays = (int)(endDate - beginDate).TotalDays;
            DateTime tempDate = beginDate;

            int index = 0;
            for (int i = 0; i <= totalDays; i++)
            {
                if (!control.IsWeekendEnabled)
                {
                    if (tempDate.DayOfWeek == DayOfWeek.Saturday || tempDate.DayOfWeek == DayOfWeek.Sunday)
                    {
                        tempDate = tempDate.AddDays(1);
                        continue;
                    }
                }

                if (tempDate == DateToday)
                {
                    control.TodayIndex = index;
                }
                dateList.Add(tempDate);
                tempDate = tempDate.AddDays(1);
                index++;
            }

            control.BindingContext = dateList;
            control.Resources[TODAY] = DateTime.Today.ToString(DATE_FORMAT);

        }

        public bool IsWeekendEnabled { get; set; }

        private int TodayIndex { get; set; } = -1;

        private StackLayout prevStack = null;
        private void DayPart_Tapped(object sender, EventArgs e)
        {
            var stackLayout = (StackLayout)sender;

            if (prevStack != null)
                prevStack.BackgroundColor = Color.Transparent;
            stackLayout.BackgroundColor = Color.FromHex("#60eeeeee");
            prevStack = stackLayout;

            var date = (DateTime)((TapGestureRecognizer)(stackLayout).GestureRecognizers[0]).CommandParameter;
            Label_CaptionDate.Text = date.ToString(DATE_FORMAT);
            SlidingEventArgs se = new SlidingEventArgs(date);
            DayItem_Tapped?.Invoke(sender, se);
        }

        private async void TodayButton_Clicked(object sender, EventArgs e)
        {
            if (TodayIndex >= 0)
            {
                var today = DateTime.Today;
                if (TodayIndex > 1)
                    await ScrollView_SlidingContainer.ScrollToAsync(StackLayout_Container.Children[TodayIndex - 2], ScrollToPosition.Start, true);
                else
                    await ScrollView_SlidingContainer.ScrollToAsync(StackLayout_Container.Children[TodayIndex], ScrollToPosition.Start, true);

                if (prevStack != null)
                    prevStack.BackgroundColor = Color.Transparent;
                StackLayout_Container.Children[TodayIndex].BackgroundColor = Color.FromHex("#60eeeeee");
                prevStack = (StackLayout)StackLayout_Container.Children[TodayIndex];
                Label_CaptionDate.Text = today.ToString(DATE_FORMAT);
                SlidingEventArgs se = new SlidingEventArgs(today);
                DayItem_Tapped?.Invoke(sender, se);
            }
        }

    }

    public class SlidingEventArgs : EventArgs
    {
        public SlidingEventArgs(DateTime selectedDate)
        {
            SelectedDate = selectedDate;
        }
        public DateTime SelectedDate { get; set; }
    }

    public enum Themes
    {
        BlueTheme,
        MetalicBlueTheme,
        RedTheme,
        GreenTheme,
        DarkTheme
    }

}