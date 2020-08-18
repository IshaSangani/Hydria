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
    public partial class NavMenuPage : TabbedPage
    {
        public NavMenuPage()
        {
            InitializeComponent();
            
        }

        public NavMenuPage(Page page)
        {
            InitializeComponent();
             
        } 
    }
}