using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CurrencyConverter
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            var mainViewModel = new MainViewModel();
            BindingContext = mainViewModel;
            ValuteListInit(mainViewModel);
        }

        public void ValuteListInit(MainViewModel mainViewModel)
        {
            DataTable currencies = mainViewModel.GetAllValutes();
            picker_source.Items.Add("RUB");
            picker_target.Items.Add("RUB");
            foreach (DataRow row in currencies.Rows)
            {
                picker_source.Items.Add(row["CharCode"].ToString());
                picker_target.Items.Add(row["CharCode"].ToString());
            }
            picker_source.SelectedItem = "USD";
            picker_target.SelectedItem = "RUB";
        }
    }
}
