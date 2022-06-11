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
        public string idEmpresa, idSucursal, idUsuario, status, barcode, nombre, kilos, tarima, ubicacion, fechasal, PedidoScanner;
        public bool found, RevSucursal, EliminarFilas = false, Clear;
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
            PedidoScanner = Models.ClaseCompartida.archivo.ToString().Replace(".txt", "");
            RevSucursal = true;

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

        private void txt_barcode_Completed(object sender, EventArgs e)
        {
            if (txt_barcode.Text.Trim() != string.Empty)
            {
                AgregaCodigo();
            }
        }

        private async void txt_barcode_TextChanged(object sender, TextChangedEventArgs e)
        {
           if (txt_barcode.Text != null | txt_barcode.Text != string.Empty | txt_barcode.Text != "")
            {
                try
                {
                    if (txt_barcode.Text.Length == barcodeCntChar)
                    {
                        AgregaCodigo();
                    }
                }
                catch (Exception f)
                {
                    await DisplayAlert("Error", f.Message.ToString(), "OK");
                }
                finally
                {
                    if (txt_barcode.Text.Length == barcodeCntChar)
                    {
                        limpiaBarcode();
                    }
                }
            }
        }

        public async void AgregaCodigo()
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

            try
            {
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

                                        //limpiaBarcode();
                                        //txt_barcode.Text = "" + string.Empty;
                                        break;
                                    }
                                    // SI YA SE REGISTRO EL CODIGO...
                                    else if (found == true)
                                    {
                                        await DisplayAlert("Error", "Ya se ha añadido este codigo.", "OK");
                                        //limpiaBarcode();
                                        //txt_barcode.Text = "" + string.Empty;
                                        break;
                                    }
                                }
                                // SI SE ENCUENTRA EN OTRA TARIMA...
                                else if (tarima != String.Empty)
                                {
                                    await DisplayAlert("Error", "El registro no se encuentra disponible.\nSe encuentra en la tarima: " + tarima + ".\n", "OK");

                                    //limpiaBarcode();
                                    txt_barcode.Text = "" + string.Empty;
                                    break;
                                }
                            }
                            // SI ESTA INACTIVO...
                            else if (status == "INACTIVO")
                            {
                                await DisplayAlert("Error", "Codigo inactivo.", "OK");
                                //limpiaBarcode();
                                txt_barcode.Text = "" + string.Empty;
                                break;
                            }
                        }
                        // SI EL PRODUCTO YA SE ENVIO
                        else
                        {
                            await DisplayAlert("Error", "Este producto ya fue enviado.\nSe registro en la tarima: " + tarima + ".\n", "OK");
                            //limpiaBarcode();
                            //txt_barcode.Text = "" + string.Empty;
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
                            tarima = dt2.Rows[i]["id_cliente"].ToString().Trim();
                        }
                        await DisplayAlert("Error", "La etiqueta tiene la siguiente fecha de salida: " + fechasal + ".\nSe encuentra en la tarima: " + tarima + ".\n", "OK");
                        //limpiaBarcode();
                        //txt_barcode.Text = "" + string.Empty;
                    }
                    else if (dt2.Rows.Count == 0)
                    {
                        await DisplayAlert("Error", "No se ha bajado de la bascula.", "OK");
                        //limpiaBarcode();
                        //txt_barcode.Text = "" + string.Empty;
                    }
                }
                // SI SE ENCONTRARON MAS DE 1 REGISTRO
                else if (dt.Rows.Count > 1)
                {
                    await DisplayAlert("Error", "Se encontro mas de una caja con el mismo codigo.", "OK");

                    //limpiaBarcode();
                    //txt_barcode.Text = "" + string.Empty;
                }
            }
            catch (Exception f)
            {
                await DisplayAlert("Error", f.Message.ToString(), "OK");
            }
        }
        
        public void txt_barcode_Unfocused(object sender, EventArgs e)
        {
            txt_barcode.CursorPosition = 1;
        }

        public async void limpiaBarcode()
        {
            txt_barcode.TextChanged -= txt_barcode_TextChanged;
            await Task.Delay(200);
            txt_barcode.IsEnabled = false;
            txt_barcode.Text = string.Empty;
            txt_barcode.IsEnabled = true;
            txt_barcode.CursorPosition = 1;
            txt_barcode.TextChanged += txt_barcode_TextChanged;
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

        private async void btn_save_Clicked(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Pregunta", "¿Desea crear la tarima?", "Si", "No");
            if (answer)
            {
                if (info.Count > 0)
                {
                    DataTable DatosT = new DataTable();
                    DatosT.Clear();
                    DatosT.Columns.Add("ID", typeof(int));
                    DatosT.Columns.Add("Barcode", typeof(string));
                    DatosT.Columns.Add("Kilos", typeof(string));

                    for (int i = 0; i < info.Count(); i++)
                    {
                        DatosT.Rows.Add(i, info[i].barcode.ToString(), info[i].kilos.ToString());
                    }
                    Console.WriteLine(DatosT.Rows.Count.ToString());

                    cn.Open();
                    SqlCommand cmd = new SqlCommand("sp_App_Tarimas", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    //DisplayAlert("Datos Enviados SP", idEmpresa + "\n" + idSucursal + "\n" + PedidoScanner + "\n" + cuentaCajas + "\n" + CantidadKG , "OK");
                    cmd.Parameters.AddWithValue("@id_empresa", SqlDbType.VarChar).Value = idEmpresa;
                    cmd.Parameters.AddWithValue("@id_sucursal", SqlDbType.VarChar).Value = idSucursal;
                    cmd.Parameters.AddWithValue("@ID_Usuario", SqlDbType.VarChar).Value = idUsuario;
                    cmd.Parameters.AddWithValue("@Pedidoscanner", SqlDbType.VarChar).Value = PedidoScanner;
                    cmd.Parameters.AddWithValue("@Contiene", SqlDbType.VarChar).Value = cuentaCajas;
                    cmd.Parameters.AddWithValue("@Cantidad", SqlDbType.VarChar).Value = CantidadKG;
                    cmd.Parameters.Add("@FolioFinal", SqlDbType.Char).Direction = ParameterDirection.ReturnValue;

                    SqlParameter param = cmd.Parameters.AddWithValue("@Datos", DatosT);
                    param.SqlDbType = SqlDbType.Structured;
                    param.TypeName = "dbo.TarimaTable";

                    SqlDataAdapter da2 = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    da2.Fill(ds);

                    string folio = cmd.Parameters["@FolioFinal"].Value.ToString();

                    cn.Close();

                    try
                    {
                        String Message = "", Tarima = "", Fsalida = "", KGsalida = "", ubicacion = "", code = "";
                        DateTime FSalida2;

                        if (ds.Tables.Count == 0 && folio.Trim() != string.Empty) // Si no encontro problemas...
                        {
                            await DisplayAlert("Operacion Completada!", "SE CREO LA TARIMA: " + folio, "OK");
                            await Task.Delay(100);
                            await Navigation.PushAsync(new DTarimas());
                        }
                        else
                        {
                            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                            {
                                code = ds.Tables[0].Rows[i]["codbarras"].ToString();
                                KGsalida = ds.Tables[0].Rows[i]["salida"].ToString();
                                FSalida2 = (DateTime)ds.Tables[0].Rows[i]["fechasal"];
                                Fsalida = FSalida2.ToString("yyyy-MM-dd");
                                Tarima = ds.Tables[0].Rows[i]["id_cliente"].ToString();
                                ubicacion = ds.Tables[0].Rows[i]["ubicacion"].ToString();

                                if (Tarima != "")
                                {
                                    Message = Message + " - Codigo: " + code + "\n   Se encuentra en la tarima: " + Tarima + "\n\n";
                                }
                                else if (Fsalida != "" & KGsalida != "" & ubicacion != "")
                                {
                                    Message = Message + " - Codigo: " + code + "\n   Tiene la siguiente ubicacion: \n   " + ubicacion.ToString() + "\n\n";
                                }
                                else if (Fsalida != "")
                                {
                                    Message = Message + " - Codigo: " + code + "\n   Tiene la siguiente fecha de salida: \n   " + Fsalida.ToString() + "\n\n";
                                }
                                else if (KGsalida != "")
                                {
                                    Message = Message + " - Codigo: " + code + "\n   Su salida fue de: " + KGsalida + "\n\n";
                                }
                                else
                                {
                                    Message = Message + "La tarima no se pudo crear.";
                                }


                                //if (code == info[i].barcode.ToString())
                                //{
                                //    for (int a = 0; a < info.Count() - 1; a++)
                                //    {
                                //        Filas.BorderColor = Color.Red;
                                //    }

                                //    List<Table> SortedList = info.OrderByDescending(x => x.id).ToList(); // Using System.Linq;
                                //    list.ItemsSource = null;
                                //    list.ItemsSource = SortedList;
                                //    list.BindingContext = this;
                                //}
                            }
                            await DisplayAlert("Los siguientes codigos ya no se encuentran disponibles:", Message, "OK");
                        }
                    }
                    catch (Exception f)
                    {
                        await DisplayAlert("Error", f.Message.ToString(), "OK");
                        throw;
                    }
                    
                }
                else
                {
                    await DisplayAlert("Error", "No se han agregado filas", "OK");
                }
            }
            else
            {
                return;
            }
        }

        public class Table
        {
            public string barcode { get; set; }
            public string nombre { get; set; }
            public decimal kilos { get; set; }
            public int id { get; set; }
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

        public ICommand Testcommand => new Command<Table>(async (p) => await Test(p));

        public async Task Test(Table param)
        {
            BARCODE = param.barcode.ToString();
            NOMBRE = param.nombre.ToString();
            KILOS = param.kilos;
            ID = param.id;

            if (EliminarFilas == true)
            {
                bool answer = await DisplayAlert("Pregunta", "¿Desea eliminar este registro?", "Si", "No");

                if (answer)
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
}