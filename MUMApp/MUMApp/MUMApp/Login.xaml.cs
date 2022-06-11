using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MUMApp.Models;
using System.Text;
using System.Security.Cryptography;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Collections;
using System.Threading.Tasks;

namespace MUMApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Login : ContentPage
    {
        SqlConnection cn = new SqlConnection(Models.ClaseCompartida.conn);
        DataTable dt = new DataTable();
        public csEncrypt Encrypt;
        public string passw, empresa, sucursal, usuario;

        public Login()
        {
            InitializeComponent();
            ClaseCompartida.resetstyles();
        }

        private void btn_Login_Clicked(object sender, EventArgs e)
        {
            IniciaProceso();
        }

        private void txt_user_Completed(object sender, EventArgs e)
        {
            IniciaProceso();
        }

        private void txt_password_Completed(object sender, EventArgs e)
        {
            IniciaProceso();
        }

        public async void IniciaProceso()
        {
            dt.Clear();

            var user = txt_user.Text;
            var pass_insert = txt_password.Text;

            if ((txt_user.Text == String.Empty | txt_password.Text == String.Empty) | (txt_user.Text == null | txt_password.Text == null))
            {
                await DisplayAlert("Error", "Ingresa usuario y contraseña", "OK");
            }
            else
            {
                cn.Open();
                SqlCommand cm = new SqlCommand("SELECT top 1 * FROM control..usuarios WHERE id_usuario = '" + user + "'", cn);
                cm.ExecuteNonQuery();
                SqlDataAdapter da = new SqlDataAdapter(cm);
                cn.Close();
                da.Fill(dt);

                // SI SE ENCONTRO 1 REGISTRO...
                if (dt.Rows.Count == 1)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        passw = dt.Rows[i]["contrasena"].ToString();
                        empresa = dt.Rows[i]["id_empresa"].ToString();
                        sucursal = dt.Rows[i]["id_sucursal"].ToString();
                        usuario = dt.Rows[i]["id_usuario"].ToString();

                        ClaseCompartida.empresa = empresa;
                        ClaseCompartida.sucursal = sucursal;
                        ClaseCompartida.usuario = usuario;

                        var pass_e = Crypt(passw);
                        var password = pass_e.TrimStart().Replace(user, "");

                        if (pass_insert == password)
                        {
                            await Task.Delay(150);
                            await Navigation.PushAsync(new Inicio());
                        }
                        else
                        {
                            await DisplayAlert("Error", "Usuario y/o Contraseña Incorrectos", "OK");
                        }
                    }
                }
                else
                {
                    await DisplayAlert("Error", "Usuario y/o Contraseña Incorrectos", "OK");
                }
            }
        }

        private string Crypt(string cadena, double Factor = 34567)
        {
            Encrypt = new csEncrypt();
            {
                var withBlock = Encrypt;
                withBlock.Texto = cadena;
                withBlock.Accion = 1.ToString();
                withBlock.Clave = Factor.ToString(); // 34567 'Segunda Parte de la Clave

            }

            return Encrypt.f_Decrypt();

        }
    }
}