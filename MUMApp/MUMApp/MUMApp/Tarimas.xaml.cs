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

    public partial class Tarimas : ContentPage
    {
        SqlConnection cn = new SqlConnection(Models.ClaseCompartida.conn);
        DataTable dt = new DataTable();
        DataTable dt2 = new DataTable();

        public static ObservableCollection<Table> info = new ObservableCollection<Table> { };
        public int salida, barcodeCntChar = 16, cuentaCajas = 0, ID;
        public string idEmpresa, idSucursal, status, barcode, nombre, kilos, tarima, ubicacion, fechasal;
        public bool found, RevSucursal, EliminarFilas = false;
        public decimal CantidadKG, KILOS;
        public string BARCODE, NOMBRE;

        public Tarimas()
        {
            InitializeComponent();
            txt_barcode.CursorPosition = 1;
            lblPedido.Text = "Pedido: " + Models.ClaseCompartida.archivo.ToString().Replace(".txt", "");
            limpiavalores();
            info.Clear();
            ClaseCompartida.resetstyles();
        }

        public void txt_barcode_Unfocused(object sender, EventArgs e)
        {
            txt_barcode.CursorPosition = 1;
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            Navigation.PushAsync(new Inicio());
        }

        private void txt_barcode_Completed(object sender, EventArgs e)
        {
            try
            {
                if (txt_barcode.Text.Length == barcodeCntChar)
                {
                    dt.Clear();
                    cn.Open();
                    SqlCommand cm = new SqlCommand("sp_App_agrega_prod_tarima", cn);
                    cm.CommandType = CommandType.StoredProcedure;
                    cm.Parameters.AddWithValue("@Empresa", SqlDbType.VarChar).Value = idEmpresa;
                    cm.Parameters.AddWithValue("@Sucursal", SqlDbType.VarChar).Value = idSucursal;
                    cm.Parameters.AddWithValue("@Barcode", SqlDbType.VarChar).Value = txt_barcode.Text;
                    cm.Parameters.AddWithValue("@RevSucursal", SqlDbType.Bit).Value = RevSucursal;
                    cm.Parameters.AddWithValue("@Opcion", SqlDbType.Int).Value = 1;

                    SqlDataAdapter da = new SqlDataAdapter(cm);
                    cn.Close();

                    da.Fill(dt);

                    // SI SE ENCONTRO 1 REGISTRO...
                    if (dt.Rows.Count == 1)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            limpiavalores();
                            status = dt.Rows[i]["estatus"].ToString().Trim();
                            nombre = dt.Rows[i]["nombre"].ToString().Trim();
                            barcode = dt.Rows[i]["codbarras"].ToString().Trim();
                            kilos = dt.Rows[i]["entrada"].ToString().Trim();
                            tarima = dt.Rows[i]["id_cliente"].ToString().Trim();
                            ubicacion = dt.Rows[i]["ubicacion"].ToString().Trim();
                            salida = Convert.ToInt32(dt.Rows[i]["salida"]);

                            Contiene(txt_barcode.Text);

                            // SI AUN NO HA SIDO ENVIADO
                            if (ubicacion != "ENVIADA")
                            {
                                // SI ESTA ACTIVO
                                if (status == "ACTIVO")
                                {
                                    // SI LA SALIDA NO SE ENCUENTRE EN OTRA TARIMA
                                    if (tarima == String.Empty)
                                    {
                                        // SI EL CODIGO NO HA SIDO REGISTRADO CON ANTERIORIDAD...
                                        if (found == false)
                                        {
                                            Decimal kg = Decimal.Parse(kilos);
                                            totales(true, cuentaCajas, Decimal.Round(kg, 2));
                                            info.Add(new Table() { barcode = barcode, nombre = nombre, kilos = Decimal.Round(kg, 2), id = cuentaCajas });
                                            
                                            List<Table> SortedList = info.OrderByDescending(x => x.id).ToList(); // Using System.Linq;
                                            list.ItemsSource = null;
                                            list.ItemsSource = SortedList;
                                            list.BindingContext = this;

                                            DependencyService.Get<IFileService>().WriteFile(Models.ClaseCompartida.archivo, barcode);
                                            break;
                                        }
                                        // SI YA SE REGISTRO EL CODIGO...
                                        else if (found == true)
                                        {
                                            DisplayAlert("Error", "Ya se ha añadido este codigo.", "OK");
                                            break;
                                        }
                                    }
                                    // SI SE ENCUENTRA EN OTRA TARIMA...
                                    else if (tarima != String.Empty)
                                    {
                                        DisplayAlert("Error", "El registro no se encuentra disponible.\n\nSe encuentra en la tarima: " + tarima + ".\n", "OK");
                                        break;
                                    }
                                }
                                // SI ESTA INACTIVO...
                                else if (status == "INACTIVO")
                                {
                                    DisplayAlert("Error", "Codigo inactivo.", "OK");
                                    break;
                                }
                            }
                            // SI EL PRODUCTO YA SE ENVIO
                            else
                            {
                                DisplayAlert("Error", "Este producto ya fue enviado.\n\nSe registro en la tarima: " + tarima + ".\n", "OK");
                                break;
                            }
                        }
                    }
                    // SI NO HAY RESULTADOS
                    else if (dt.Rows.Count == 0)
                    {
                        dt2.Clear();
                        cn.Open();
                        SqlCommand cmd = new SqlCommand("sp_App_agrega_prod_tarima", cn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Empresa", SqlDbType.VarChar).Value = idEmpresa;
                        cmd.Parameters.AddWithValue("@Sucursal", SqlDbType.VarChar).Value = idSucursal;
                        cmd.Parameters.AddWithValue("@Barcode", SqlDbType.VarChar).Value = txt_barcode.Text;
                        cmd.Parameters.AddWithValue("@RevSucursal", SqlDbType.Bit).Value = RevSucursal;
                        cmd.Parameters.AddWithValue("@Opcion", SqlDbType.Int).Value = 2;

                        SqlDataAdapter da2 = new SqlDataAdapter(cmd);
                        cn.Close();

                        da2.Fill(dt2);

                        // SI SE ENCONTRO 1 REGISTRO...
                        if (dt2.Rows.Count == 1)
                        {
                            for (int i = 0; i < dt2.Rows.Count; i++)
                            {
                                fechasal = "";
                                fechasal = dt2.Rows[i]["fechasal"].ToString().Trim();

                            }
                            DisplayAlert("Error", "La etiqueta tiene la siguiente fecha de salida: \n\n" + fechasal + ".\nSe encuentra en la tarima:\n\n", "OK");
                        }
                        else if (dt2.Rows.Count == 0)
                        {
                            DisplayAlert("Error", "No se ha bajado de la bascula.", "OK");
                        }
                    }
                    // SI SE ENCONTRARON MAS DE 1 REGISTRO
                    else if (dt.Rows.Count > 1)
                    {
                        DisplayAlert("Error", "Se encontro mas de una caja con el mismo codigo.", "OK");
                        #region comentarios
                        //for (int i = 0; i < dt.Rows.Count; i++)
                        //{
                        //    limpiavalores();
                        //    status = dt.Rows[i]["estatus"].ToString().Trim();
                        //    tarima = dt.Rows[i]["id_cliente"].ToString().Trim();
                        //    ubicacion = dt.Rows[i]["ubicacion"].ToString().Trim();

                        //    if (ubicacion != "ENVIADA")
                        //    {
                        //        if (status == "ACTIVO") // SI ESTA ACTIVO...
                        //        {
                        //            if (tarima != string.Empty) // SI SE ENCUENTRA EN OTRA TARIMA...
                        //            {
                        //                DisplayAlert("Error", "El registro no se encuentra disponible.\n\nSe encuentra en la tarima: " + tarima + ".\n", "OK");
                        //                break;
                        //            }
                        //        }
                        //        else if (status == "INACTIVO") // SI ESTA INACTIVO...
                        //        {
                        //            DisplayAlert("Error", "Codigo inactivo.", "OK");
                        //            break;
                        //        }
                        //    }
                        //    else // SI EL PRODUCTO YA SE ENVIO
                        //    {
                        //        DisplayAlert("Error", "Este producto ya fue enviado.\n\nSe registro en la tarima: " + tarima + ".\n", "OK");
                        //        break;
                        //    }
                        //}
                        #endregion
                    }
                }
                else
                {
                    DisplayAlert("Error", "El codigo de barras debe contener " + barcodeCntChar + " caracteres.", "OK");
                }

                txt_barcode.Text = String.Empty;
                txt_barcode.CursorPosition = 1;
            }
            catch (Exception f)
            {
                if (txt_barcode.Text == null) // SI NO SE INGRESO NADA EN EL CAMPO DE TEXTO
                {
                    DisplayAlert("Error", "Ingresa un codigo de barras.", "OK");
                    txt_barcode.Text = String.Empty;
                    txt_barcode.CursorPosition = 1;
                }
                else
                {
                    DisplayAlert("Error", f.Message.ToString(), "OK");
                    txt_barcode.Text = String.Empty;
                    txt_barcode.CursorPosition = 1;
                }
            }
        }

        private void sw_EliminarF_Toggled(object sender, ToggledEventArgs e)
        {
            if (sw_EliminarF.IsToggled)
            {
                App.Current.Resources["dark_color"] = Color.FromHex("BF0B29");
                App.Current.Resources["dark_text_color"] = Color.FromHex("BF0B29");
                App.Current.Resources["light_color"] = Color.FromHex("E20A2E");
                App.Current.Resources["placeholder_color"] = Color.FromHex("D45066");
                App.Current.Resources["table_border_color"] = Color.Red;
                EliminarFilas = true;

            }
            else
            {
                ClaseCompartida.resetstyles();
                EliminarFilas = false;
            }
        }

        public class Table
        {
            public string barcode { get; set; }
            public string nombre { get; set; }
            public decimal kilos { get; set; }
            public int id { get; set; }
        }

        public void limpiavalores()
        {
            status = string.Empty;
            barcode = string.Empty;
            nombre = string.Empty;
            kilos = string.Empty;
            tarima = string.Empty;
            ubicacion = string.Empty;
            salida = 0;
        }

        public bool Contiene(string item) // Recorre la lista en busca del elemento item (codigo de barras) para comprobar si ya se agrego o no en la lista
        {
            for (int y = 0; y < info.Count(); y++)
            {
                if (info[y].barcode.Contains(item))
                {
                    found = true;
                    break; // Si encuentra el codigo ingresado en la lista, termina el ciclo
                }
                else
                {
                    found = false;
                }
            }

            return found;
        }

        public void totales(bool accion, int cjs, decimal kg)
        {
            string cajas, kilos;

            cajas = Convert.ToInt32(lblCajas.Text).ToString();
            kilos = Convert.ToDecimal(lblKilos.Text).ToString();

            if (accion == true) // SI SE AGREGA UN PRODUCTO A LA TABLA...
            {
                cajas = (Convert.ToInt32(cajas) + 1).ToString();
                kilos = (Convert.ToDecimal(kilos) + kg).ToString();
                lblCajas.Text = cajas.ToString();
                lblKilos.Text = kilos.ToString();
                CantidadKG = Convert.ToDecimal(kilos);
                cuentaCajas = Convert.ToInt32(cajas);
            }
            else if (accion == false) // SI SE ELIMINA UN PRODUCTO DE LA TABLA...
            {
                cajas = (Convert.ToInt32(cajas) - 1).ToString();
                kilos = (Convert.ToDecimal(kilos) - kg).ToString();
                lblCajas.Text = cajas;
                lblKilos.Text = kilos;
                CantidadKG = Convert.ToDecimal(kilos);
                cuentaCajas = Convert.ToInt32(cajas);
            }
        }

        public ICommand Testcommand => new Command<Table>(async (p) => await Test(p));

        public async Task Test(Table param)
        {
            BARCODE = param.barcode.ToString();
            NOMBRE = param.nombre.ToString();
            KILOS = param.kilos;
            ID = param.id;

            if (EliminarFilas == true)
            {
                info.RemoveAt(ID - 1);

                var newid = 1;
                foreach (var item in info)
                {
                    item.id = newid;
                    newid++;
                }

                List<Table> SortedList = info.OrderByDescending(x => x.id).ToList(); // Using System.Linq;
                list.ItemsSource = null;
                list.ItemsSource = SortedList;
                list.BindingContext = this;

                Decimal kg = KILOS;
                totales(false, cuentaCajas, Decimal.Round(kg, 2));

                sw_EliminarF.IsToggled = false;
            }
        }
    }
}