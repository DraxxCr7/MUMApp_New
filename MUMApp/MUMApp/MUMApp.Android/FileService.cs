using Android.App;
using MUMApp.Droid;
using MUMApp.Models;
using System;
using System.IO;
using System.Linq;

[assembly: Xamarin.Forms.Dependency(typeof(FileService))]

namespace MUMApp.Droid
{
    public class FileService : IFileService
    {
        public string GetRootPath()
        {
            return Application.Context.GetExternalFilesDir(null).ToString();
        }

        public void CreateFile(string name)
        {
            var destination = Path.Combine(GetRootPath(), name);

            if (!File.Exists(destination))
            {
                File.Create(destination).Close();

            }
            else if (File.Exists(destination))
            {
                return;
            }
        }

        public void WriteFile(string name, string message)
        {
            var destination = Path.Combine(GetRootPath(), name);

            if (File.Exists(destination))
            {
                File.AppendAllText(destination, message + System.Environment.NewLine);
            }
            else if (!File.Exists(destination))
            {
                return;
            }
        }

        public void ClearFile(string name)
        {
            var destination = Path.Combine(GetRootPath(), name);

            if (File.Exists(destination))
            {
                File.WriteAllText(destination, String.Empty);
            }
            else if (!File.Exists(destination))
            {
                return;
            }
        }

        public void ModifyFile(string name, string message, int selection)
        {
            var destination = Path.Combine(GetRootPath(), name);

            if (File.Exists(destination))
            {
                var linesList = File.ReadAllLines(destination).ToList();
                linesList.RemoveAt(selection);
                File.WriteAllLines(destination, linesList.ToArray());
            }
            else if (!File.Exists(destination))
            {
                return;
            }
        }

        public void traecantidad()
        {
            string[] dirs = Directory.GetFiles(GetRootPath(), "*.txt");
            int cantidad = dirs.Length;
            Console.WriteLine(cantidad.ToString());
        }
    }
}