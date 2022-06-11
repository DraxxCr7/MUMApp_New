using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MUMApp.Models;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MUMApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Inicio : ContentPage
    {
        public static ObservableCollection<DataModel> MySource = new ObservableCollection<DataModel> { };
        public int Opcion;
        public Inicio()
        {
            InitializeComponent();
            ClaseCompartida.resetstyles();
            txtEmpresa.Text = " - Empresa: " + ClaseCompartida.empresa;
            txtSucursal.Text = " - Sucursal: " +  ClaseCompartida.sucursal;
            txtUser.Text = " - Usuario: " + ClaseCompartida.usuario;

            MySource.Clear();
            MySource.Add(new DataModel { Title = "CREAR TARIMAS", Opcion = 1, Imagen = "icon_op1.png" });
            MySource.Add(new DataModel { Title = "UBICAR TARIMAS", Opcion = 2, Imagen = "icon_op2.png" });

            list.ItemsSource = null;
            list.ItemsSource = MySource;
            list.BindingContext = this;
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            var stack = sender as Frame;

            stack.Scale = 1;
            await Task.Delay(200);
            stack.Scale = 0.95;
        }

        private async void btn_end_session_Clicked(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Pregunta", "¿Desea cerrar sesion?", "Si", "No");
            if (answer)
            {
                ClaseCompartida.empresa = "";
                ClaseCompartida.sucursal = "";
                ClaseCompartida.usuario = "";
                await Navigation.PushAsync(new Login());
            }
            else
            {
                return;
            }
        }

        public class DataModel
        {
            public string Title { get; set; }
            public int Opcion { get; set; }
            public string Imagen { get; set; }
        }

        public ICommand Testcommand => new Command<DataModel>(async (p) => await Test(p));

        public async Task Test(DataModel param)
        {
            Opcion = param.Opcion;

            switch (Opcion)
            {
                case 1:
                    await Navigation.PushAsync(new DTarimas());
                    break;

                case 2:
                    await Navigation.PushAsync(new Ubicar_Tarimas());
                    break;
            }
        }
    }
}