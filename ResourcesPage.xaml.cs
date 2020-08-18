using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Hydria_Calculator
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ResourcesPage : ContentPage
    {
        public ICommand TapCommand => new Command<string>(OpenBrowser);
        public ResourcesPage()
        {
            InitializeComponent();
            BindingContext = this; 
        }

        void OpenBrowser(string url)
        {
            Device.OpenUri(new Uri(url));
        }
    }
}