namespace MUMApp.Models
{
    public interface IFileService
    {
        void CreateFile(string name);
        void WriteFile(string name, string message);
        void ModifyFile(string name, string message, int selection);
        void ClearFile(string name);
        void traecantidad();
    }
}
