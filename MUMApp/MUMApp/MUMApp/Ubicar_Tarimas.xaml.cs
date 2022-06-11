using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using MUMApp.Models;

namespace MUMApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class Ubicar_Tarimas : ContentPage
    {
        SqlConnection cn = new SqlConnection(Models.ClaseCompartida.conn);
        DataTable dt = new DataTable();
        public string idEmpresa, idSucursal, idUsuario;
        
        public Ubicar_Tarimas()
        {
            InitializeComponent();
            txt_barcode.CursorPosition = 1;
            lblPedido.Text = "UBICAR TARIMAS";

            idEmpresa = ClaseCompartida.empresa;
            idSucursal = ClaseCompartida.sucursal;
            idUsuario = ClaseCompartida.usuario;
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Pregunta", "¿Desea regresar al menu principal?", "Si", "No");

            if (answer)
            {
                await Navigation.PushAsync(new Inicio());
            }
            else
            {
                return;
            }
        }
    }
}