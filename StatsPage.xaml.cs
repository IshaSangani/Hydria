using Android.App;
using Firebase.Database;
using Hydria_Calculator.Droid.Helpers;
using Microcharts;
using Microcharts.Forms;
using SQLite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Core;
using Xamarin.Forms.Xaml;

namespace Hydria_Calculator.Droid
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StatsPage : ContentPage
    {
        static Chart ScoresChart = new BarChart();
        static Chart ItemsChart = new BarChart();
        
        static DataEventListener ItemDataListener;
        static DataEventListener ScoreDataListener;
        static List<Microcharts.ChartEntry> PastMonthItemData = new List<Microcharts.ChartEntry>();
        static List<Microcharts.ChartEntry> PastMonthPointData = new List<Microcharts.ChartEntry>();
        static List<Microcharts.ChartEntry> BlankList = new List<Microcharts.ChartEntry>();
        public StatsPage()
        {
            InitializeComponent();


            if (!App.OMonth.HasValue)
            {
                PullBlankChart();
            }
            else
            {
                //NEED TO CHANGE THIS BACK WHEN DONE TESTING
                PullPastMonthData();
                
            }
            ShowCharts();

        }

        //Creates charts with correct data and puts in stacklayout
        public void ShowCharts()
        {
            ItemsChart.BackgroundColor = SkiaSharp.SKColor.Parse("#ffffff");
            ScoresChart.BackgroundColor = SkiaSharp.SKColor.Parse("#ffffff");

            Label title2 = new Label
            {
                Text = "Daily Score During " + DateTime.Now.Month.ToString(),
                TextColor = Color.DodgerBlue,
                FontAttributes = FontAttributes.Bold,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                HorizontalOptions = LayoutOptions.Center
            };
            var chart2 = new ChartView
            {
                BackgroundColor = Color.FromHex("#ffffff"),
                Chart = ScoresChart,
                HeightRequest = 180

            };
            Label axis2 = new Label
            {
                Text = "Time",
                TextColor = Color.DodgerBlue,
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                HorizontalOptions = LayoutOptions.Center
            };
            Label title1 = new Label
            {
                Text = "Daily Number of Items During " + DateTime.Now.Month.ToString(),
                TextColor = Color.DodgerBlue,
                FontAttributes = FontAttributes.Bold,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                HorizontalOptions = LayoutOptions.Center

            };
            var chart1 = new ChartView
            {
                BackgroundColor = Color.FromHex("#ffffff"),
                Chart = ItemsChart, 
                HeightRequest = 180
            };
            Label axis1 = new Label
            {
                Text = "Time",
                TextColor = Color.DodgerBlue,
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                HorizontalOptions = LayoutOptions.Center
            };
            statsLayout.Children.Add(title2); 
            statsLayout.Children.Add(chart2);
            statsLayout.Children.Add(axis2); 
            statsLayout.Children.Add(title1);
            statsLayout.Children.Add(chart1);
            statsLayout.Children.Add(axis1);

        }
        //Initializes a blank chart
        public void PullBlankChart()
        {
            ItemsChart.Entries = BlankList;
            ScoresChart.Entries = BlankList;

            Label nothing = new Label
            {
                Text = "You don't have any previous data, so there's nothing to see here!",
                TextColor = Color.DodgerBlue
            };
            statsLayout.Children.Add(nothing); 
        }


        //Initializes a chart with the past month's data       
        public void PullPastMonthData()
        {
            int py = DateTime.Now.Year;
            if (DateTime.Now.Month == 12)
            {
                py--;
            }
            int pm = DateTime.Now.Month - 1;
            int daysInMonth = 31;
            int[] monthsWith30Days = { 4, 6, 9, 11 };
            foreach (int i in monthsWith30Days)
            {
                if (pm == i)
                {
                    daysInMonth = 30;
                }
            }
            if (pm == 2)
            {
                if (py % 4 == 0 && py % 100 != 0)
                {
                    daysInMonth = 29;
                }
                else
                {
                    daysInMonth = 28;
                }
            }
            string prevMonth = "" + pm;
            string prevYear = "" + py;

            ItemDataListener = new DataEventListener();
            ScoreDataListener = new DataEventListener();
            for (int date = 1; date <= daysInMonth; date++)
            {
                DatabaseReference itemsref = AppDataHelper.GetDatabase().GetReference("users").Child(App.ThisUser.Id).Child(prevYear).Child(prevMonth).Child(date + "").Child("items");
                DatabaseReference scoreref = AppDataHelper.GetDatabase().GetReference("users").Child(App.ThisUser.Id).Child(prevYear).Child(prevMonth).Child(date + "").Child("score");
                itemsref.AddValueEventListener(ItemDataListener);
                ItemDataListener.DataRetrieved += ItemDataListener_DataRetrieved;
                scoreref.AddValueEventListener(ScoreDataListener);
                ScoreDataListener.DataRetrieved += ScoreDataListener_DataRetrieved;
            }
            ItemsChart.Entries = PastMonthItemData;
            ScoresChart.Entries = PastMonthPointData;
        }

        private static void ItemDataListener_DataRetrieved(object sender, DataEventListener.DataEventArgs e)
        {

            PastMonthItemData.Add(new Microcharts.ChartEntry(e.dataPoint) { Label = "" + e.dataPoint, Color = SkiaSharp.SKColor.Parse("#1e90ff"), TextColor = SkiaSharp.SKColor.Parse("#1e90ff") });
        }
        private static void ScoreDataListener_DataRetrieved(object sender, DataEventListener.DataEventArgs e)
        {
            PastMonthPointData.Add(new Microcharts.ChartEntry(e.dataPoint) { Label = "" + e.dataPoint, Color = SkiaSharp.SKColor.Parse("#1e90ff"), TextColor = SkiaSharp.SKColor.Parse("#1e90ff") });
        }
        public static void DisplayPastYearData()
        {
            throw new NotImplementedException();
        }

        public static void DisplayLifetimeData()
        {
            throw new NotImplementedException();
        }
    }

}