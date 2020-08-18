using SQLite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Core;
using Xamarin.Forms.Xaml;

namespace Hydria_Calculator
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TrackingPage : ContentPage
    {        
        static Image i = new Image(); 
        static Label label;
        static Label invitation;
        static Label assessment; 
        public TrackingPage()
        {
            InitializeComponent();
            label = new Label
            {               
                TextColor = Color.DodgerBlue,
                FontAttributes = FontAttributes.Bold,
                FontSize = Device.GetNamedSize(NamedSize.Header, typeof(Label)),
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Start
            };
            invitation = new Label
            {
                TextColor = Color.DodgerBlue,
                HorizontalTextAlignment = TextAlignment.Center,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                HorizontalOptions = LayoutOptions.Center
            };
            assessment = new Label
            {
                TextColor = Color.DodgerBlue,
                HorizontalTextAlignment = TextAlignment.Center,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Start
            };
            UpdateContents();
            funHeaderStackLayout.Children.Add(invitation);
            funFooterStackLayout.Children.Add(label);
            funFooterStackLayout.Children.Add(assessment);            
            funFooterStackLayout.Children.Add(i);
        }
        //Displays the ocean image, assessment, and invitation based on score
        public void UpdateContents()
        {
            //Displays table with actions
            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                conn.CreateTable<Action>();
                var actions = conn.Table<Action>().ToList();
                actionsListView.ItemsSource = actions;

            }
            //Displays invitation, assessment, label, image
            label.Text = "" + App.Score; 
            i.IsVisible = true;
            i.IsAnimationPlaying = true;
            i.VerticalOptions = LayoutOptions.StartAndExpand;            
            if (App.Score >= 15)
            {
                i.Source = "clean_ocean_animation.gif";
                invitation.Text = "Wow! Your ocean is really clean.\n";
            }
            else if (12 <= App.Score && App.Score < 15)
            {
                assessment.Text = "You're getting toward a cleaner ocean! Keep doing more actions.";
                invitation.Text = "Add actions to get points and see your ocean get cleaner.\n";
                i.Source = "dirty_ocean_animation.gif";
            }
            else if (9 <= App.Score && App.Score < 12)
            {
                assessment.Text = "You're getting toward a cleaner ocean! Keep doing more actions.";
                invitation.Text = "Add actions to get points and see your ocean get cleaner.\n";
                i.Source = "dirtier_ocean_animation.gif";
            }
            else if (App.Score >=6 && App.Score < 9)
            {
                assessment.Text = "You could be taking more steps to get us to a cleaner ocean. Check out the Info tab for tips and resources.";
                invitation.Text = "Add actions to get points and see your ocean get cleaner.\n";
                i.Source = "yucky_ocean_animation.gif";
            }
            else if (App.Score < 6)
            {
                assessment.Text = "Reduce, reuse and recycle to pollute the ocean less. Check out the Info tab for tips and resources.";
                invitation.Text = "Add actions to get points and see your ocean get cleaner.\n";
                i.Source = "disgusting_ocean_animation.gif";
            }
        }
        
        protected override void OnAppearing()
        {
            base.OnAppearing();
            UpdateContents();
        }        
        async void addBtn_Clicked(object sender, EventArgs e)
        {
            string action = await DisplayActionSheet("In what way did you reduce your plastic usage?", "Cancel", null, "Reduce", "Reuse", "Recycle");
            string note = ""; 
            if (action != "Cancel")
            {
                note = await DisplayPromptAsync("Details", "Enter more info about the action - for example, \"used a glass water bottle\"");
            }
            
            if (action != "Cancel" && action != null)
            {
                AddToDatabase(action, note);
                await Navigation.PopAsync();
            }
            else
            {
                await Navigation.PopAsync();
            }            
        }
        //Creates appropriate action, updates score, adds to database
        private void AddToDatabase(string action, string note)
        {
            Action a = new Action()
            {
                ActionType = action,
                Note = note
            };
            if (action=="Recycle")
            {
                a.Points = 1;
            }
            if (action=="Reuse")
            {
                a.Points = 2;
            }
            if (action=="Reduce")
            {
                a.Points = 3;
            }
            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                conn.CreateTable<Action>();
                int rows = conn.Insert(a);
                if (rows > 0)
                {
                    App.Items++;
                    App.Score += a.Points; 
                    DisplayAlert("Success", "Your action has been saved.", "OK");
                   
                }
                else
                {
                    DisplayAlert("Failure", "Your action had an issue being saved.", "OK");
                    
                }
            }
            UpdateContents();
        }
        private void actionsListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var selectedAction = actionsListView.SelectedItem as Action;

            if (selectedAction != null)
            {
                Navigation.PushAsync(new ActionDetailPage(selectedAction));
            }
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

    }
}