using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Hydria_Calculator
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActionDetailPage : ContentPage
    {
        Action selectedAction; 
        public ActionDetailPage(Action selectedAction)
        {
            InitializeComponent();
            this.selectedAction = selectedAction;
        }

        async void detailsBtn_Clicked(object sender, EventArgs e)
        {
            string note = await DisplayPromptAsync("Details", "Enter more info about the action - for example, \"used a glass water bottle\"");
            selectedAction.Note = note;
            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                conn.CreateTable<Action>();
                int rows = conn.Update(selectedAction);
                CountRows(rows);
            }
        }

        void deleteBtn_Clicked(object sender, EventArgs e)
        {
            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                conn.CreateTable<Action>();
                int rows = conn.Delete(selectedAction);
                CountRows(rows);
                App.Items--; 
            }
        }
        async void actionTypeBtn_Clicked(object sender, EventArgs e)
        {
            string action = await DisplayActionSheet("Which action did you perform?", "Cancel", null, "Reduce", "Reuse", "Recycle");
            selectedAction.ActionType = action;
            if (action=="Reduce")
            {
                selectedAction.Points = 3;
            }
            else if (action=="Reuse")
            {
                selectedAction.Points = 2;
            }
            else if (action=="Recycle")
            {
                selectedAction.Points = 1;
            }
            
            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                conn.CreateTable<Action>();
                int rows = conn.Update(selectedAction);
                CountRows(rows);
            }
        }
        //Counts the row number of changed db item and display approp. alert depending on
        //if item was saved or not
        public void CountRows(int rows)
        {
            if (rows > 0)
            {
                DisplayAlert("Success", "Action successfully updated.", "OK");
                Navigation.PopAsync();
            }
            else
            {
                DisplayAlert("Failure", "Action update failed.", "OK");
                Navigation.PopAsync();
            }
        }
    }
}