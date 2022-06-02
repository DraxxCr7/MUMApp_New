namespace MUMApp.Models
{
    public class ClaseCompartida
    {
        public static string conn = "Data Source=10.0.0.50;Initial Catalog=voficina;User ID='sa';Password='Muma2022';Integrated Security=false;";
        public static string archivo;

        public static void resetstyles()
        {
            App.Current.Resources["background_color"] = App.Current.Resources["background_color"];
            App.Current.Resources["dark_color"] = App.Current.Resources["predefined_dark_color"];
            App.Current.Resources["light_color"] = App.Current.Resources["predefined_light_color"];
            App.Current.Resources["light_text_color"] = App.Current.Resources["predefined_light_text_color"];
            App.Current.Resources["dark_text_color"] = App.Current.Resources["predefined_dark_text_color"];
            App.Current.Resources["placeholder_color"] = App.Current.Resources["predefined_placeholder_color"];
            App.Current.Resources["table_border_color"] = App.Current.Resources["predefined_table_border_color"];
        }
    }
}
