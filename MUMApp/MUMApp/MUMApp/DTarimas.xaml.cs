using MUMApp.Models;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Linq;

namespace MUMApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DTarimas : ContentPage
    {
        public string file;
        public DTarimas()
        {
            InitializeComponent();
            MuestraTeclado();
            ClaseCompartida.resetstyles();
        }

        public async void MuestraTeclado()
        {
            await Task.Delay(150);
            txt_tarima.Focus();
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            Navigation.PushAsync(new Inicio());
        }

        public void txt_tarima_Unfocused(object sender, EventArgs e)
        {
            txt_tarima.CursorPosition = 1;
        }

        private async void txt_tarima_Completed(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txt_tarima.Text))
            {
                await DisplayAlert("Informacion", "Ingresa un pedido", "OK");
                await Task.Delay(500);
                txt_tarima.Focus();
                txt_tarima.CursorPosition = 1;

            }
            else
            {
                file = txt_tarima.Text.ToString() + ".txt";
                Models.ClaseCompartida.archivo = file;
                DependencyService.Get<IFileService>().CreateFile(file);

                await Navigation.PushAsync(new Tarimas());
            }
        }
    }
}