using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MUMApp.Models;

namespace MUMApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Inicio : ContentPage
    {
        public Inicio()
        {
            InitializeComponent();
            ClaseCompartida.resetstyles();
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            switch (lblOpcion.Text)
            {
                case "1":
                    await Navigation.PushAsync(new DTarimas());
                    break;
            }
        }
    }
}