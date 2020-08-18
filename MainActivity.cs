using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Firebase.Database;
using Hydria_Calculator.Droid.Helpers;
using Java.Util;
using System;
using Xamarin.Essentials;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;

[assembly: Application(Icon = "@drawable/Hydria_circle_logo")]


namespace Hydria_Calculator.Droid
{
    [Activity(Label = "Hydria", Icon = "@drawable/Hydria_circle_logo", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            //Creates Database reference, pushes it out
           
            //Creates, loads app w/ database
            string dbName = "actions_db.sqlite";
            string folderPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            string fullPath = System.IO.Path.Combine(folderPath, dbName);

            //retrieves saved date and month from bundle from last time app closed (see OnSavedInstanceState method)
            int? old = 2;
                //Hee hee mold
            int? mold = 6;
            int? yold = 2002; 
            //statements inside nested if statements only execute if the app has been run before
            if (!(savedInstanceState is null))
            {
                if (!((int?)savedInstanceState.GetInt("date") is null)) 
                {
                    old = savedInstanceState.GetInt("date");
                }
                if (!((int?)savedInstanceState.GetInt("month") is null))
                {
                    mold = savedInstanceState.GetInt("month");
                }
                if (!((int?)savedInstanceState.GetInt("year") is null))
                {
                    yold = savedInstanceState.GetInt("year");
                }
                if (old != DateTime.Now.DayOfYear && mold != DateTime.Now.Month && yold != DateTime.Now.Year)  
                {
                    StoreTodaysData(); 
                }
                if (mold != DateTime.Now.Month && yold != DateTime.Now.Year)  
                {
                    SimplifyPrevMonth();                     
                }
                if (yold != DateTime.Now.Year)
                {
                    SimplifyPrevYear();
                }
                
                if (!(savedInstanceState.GetString("userID") is null))
                {
                    App.ThisUser.Id = savedInstanceState.GetString("userID");
                }
            }
            //loads application 
            App A = new App(fullPath);            
            LoadApplication(A);
            //ODate set to retrieved int value from old bundle (recorded date last destroyed)
            //Same for OMonth
            App.ODate = old;
            App.OMonth = mold;
            App.OYear = yold;  
            
        }        
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        //Saves the date when the app screen is exited - used in ResetDatabase() method in App.cs
        [Android.Runtime.Register("onSaveInstanceState", "(Landroid/os/Bundle;)V", "GetOnSaveInstanceState_Landroid_os_Bundle_Handler")]
        protected override void OnSaveInstanceState(Bundle outState)
        {
            outState.PutInt("date", DateTime.Now.DayOfYear);            
            outState.PutInt("month", DateTime.Now.Month);
            outState.PutInt("year", DateTime.Now.Year);
            outState.PutString("userID", App.ThisUser.Id);
            base.OnSaveInstanceState(outState);
            StoreTodaysData(); 
        }

        public static void StoreTodaysData()
        {
            HashMap mo = new HashMap();
            HashMap dy = new HashMap();
            HashMap metrics = new HashMap();          
           
            metrics.Put("items", App.Items);
            metrics.Put("score", App.Score);
            dy.Put(DateTime.Now.Day + "", metrics);
            mo.Put(DateTime.Now.Month + "", dy);
           
            if (!(App.OYear.Equals(DateTime.Now.Year)))
            {
                DatabaseReference dr = AppDataHelper.GetDatabase().GetReference("users").Child(App.ThisUser.Id).Child(DateTime.Now.Year + "");
                dr.SetValue(mo);
               
            }
            else if (!(App.OMonth.Equals(DateTime.Now.Month)))
            {
                DatabaseReference dr = AppDataHelper.GetDatabase().GetReference("users").Child(App.ThisUser.Id).Child(DateTime.Now.Year + "").Child(DateTime.Now.Month + "");
                dr.SetValue(dy);               
               
            }
            else if (!(App.ODate.Equals(DateTime.Now.Day)))
            {
                DatabaseReference dr = AppDataHelper.GetDatabase().GetReference("users").Child(App.ThisUser.Id).Child(DateTime.Now.Year + "").Child(DateTime.Now.Month + "").Child(DateTime.Now.Day + "");
                dr.SetValue(metrics);   
                
            }
            
        }

        public static void SimplifyPrevMonth()
        {            
            int pm = DateTime.Now.Month - 1;            
            int py = DateTime.Now.Year; 
            if (DateTime.Now.Month ==1)
            {
                py--;
            }           

            int monthlyTotalItems = 0;
            int monthlyAvgScore = 0;
            int daysInMonth = 31;
            //changes daysInMonth depending on the month that pm was            
            if (pm == 2)
            {
                daysInMonth = 28; 
                if (py%4==0)
                {
                    daysInMonth = 29; 
                }
            }
            int[] monthsWith30Days = { 4, 6, 9, 11 };
            foreach (int i in monthsWith30Days)
            {
                if (pm==i)
                {
                    daysInMonth = 30; 
                }
            }
            string prevMonth = "" + pm;
            string prevYear = py + "";
            for (int date = 1; date <= daysInMonth; date++)
            {
                DatabaseReference items = AppDataHelper.GetDatabase().GetReference("users/" + App.ThisUser.Id + "/" + prevYear + "/" + prevMonth + "/" + date + "/items/");
                monthlyTotalItems += (int) items.RemoveValue();
                DatabaseReference scores = AppDataHelper.GetDatabase().GetReference("users/" + App.ThisUser.Id + "/" + prevYear + "/" + prevMonth + "/" + date + "/score/");
                monthlyAvgScore += (int)scores.RemoveValue();
            }
            monthlyAvgScore /= daysInMonth; 
            DatabaseReference month = AppDataHelper.GetDatabase().GetReference("users/" + App.ThisUser.Id + "/year/" + prevYear + "/" + prevMonth);
            HashMap h = new HashMap();
            h.Put("monthlyAvgScore", monthlyAvgScore);
            h.Put("monthlyTotalItems", monthlyTotalItems);
            month.SetValue(h); 
        }
        //Do we want years to be further ahead? 
        public static void SimplifyPrevYear()
        {
            int py = DateTime.Now.Year - 1;
            string prevYear = "" + py;             
            int yearlyTotalItems = 0;
            int yearlyAvgScore = 0;
            for (int month = 1; month <= 12; month++)
            {
                DatabaseReference items = AppDataHelper.GetDatabase().GetReference("users/" + App.ThisUser.Id + "/year/" + prevYear + "/month/" + month + "/monthlyAvgScore");
                yearlyTotalItems += (int)items.RemoveValue();
                DatabaseReference scores = AppDataHelper.GetDatabase().GetReference("users/" + App.ThisUser.Id + "/year/" + prevYear + "/month/" + month + "/monthlyAvgScore");
                yearlyAvgScore += (int)scores.RemoveValue();
            }
            //HOW TO WEIGHT BASED ON NUMBER OF DAYS LOGGED? 
            yearlyAvgScore /= 12;
            DatabaseReference year = AppDataHelper.GetDatabase().GetReference("users/" + App.ThisUser.Id + "/year/" + prevYear);
            HashMap h = new HashMap();
            h.Put("yearlyAvgScore", yearlyAvgScore);
            h.Put("yearlyTotalItems", yearlyTotalItems);
            year.SetValue(h);
        }
    }
}