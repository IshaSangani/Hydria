using Firebase.Auth;
using Firebase.Database;
using Hydria_Calculator.Droid;
using Hydria_Calculator.Droid.Helpers;
using Java.Util;
using SQLite;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace Hydria_Calculator
{
    public partial class App : Application
    {
        public static int Score;
        public static int Items; 
        public static string DatabaseLocation = string.Empty;
        public static int? ODate;
        public static int? OMonth;
        public static int? OYear; 
        
        public static User ThisUser = new User(); 
        public App()
        {
            InitializeComponent();
 
        }

        public App(string databaseLocation)
        {
            InitializeComponent();
            MainPage = new NavigationPage(new NavMenuPage());
            DatabaseLocation = databaseLocation;
            if (!ODate.HasValue)
            {
                InitializeNewUser();
            }
        }

        protected override void OnStart()
        {
            if (!(ODate is null) && DateTime.Now.Day != ODate && DateTime.Now.Year != OYear && DateTime.Now.Month != OMonth)
            {                
                ResetDatabase();
                
            }                       
        }

        protected override void OnSleep()
        {
            
        }

        protected override void OnResume()
        {
            if (!(ODate is null) && DateTime.Now.Day != ODate && DateTime.Now.Year != OYear && DateTime.Now.Month != OMonth)
            {
                ResetDatabase();

            }
           
        }
        //User is only assigned an id if the app has never been opened before
        private void InitializeNewUser()
        {
            DatabaseReference eek = AppDataHelper.GetDatabase().GetReference("users").Push();
            ThisUser.Id = eek.Key;
            ThisUser.beginDate = DateTime.Now.Day;
            ThisUser.beginMonth = DateTime.Now.Month;
            ThisUser.beginYear = DateTime.Now.Year; 
        }
        //Wipes database if old date retrieved from onSaveInstanceState() in MainActivity.cs is less than today's date
        //IE, wipes db if the date that the user last closed the application was not today
        private void ResetDatabase()
        {
            using (SQLiteConnection conn = new SQLiteConnection(DatabaseLocation))
            {
                conn.CreateTable<Action>();
                var actions = conn.Table<Action>().ToList();
                for (int i = 0; i < actions.Count; i++)
                {
                    int rows = conn.Delete(actions[i]);
                }
            }
            Items = 0; 
        }        
    }
}
