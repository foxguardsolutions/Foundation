using System.IO;
using System.Web;

using FGS.Pump.Settings.Config.FileOpsSettings;

namespace FGS.Pump.MVC.Support
{
    public abstract class FileOperations
    {
        protected IFileOpsSettings FileOpsSettings { get; }

        protected FileOperations(IFileOpsSettings fileOpsSettings)
        {
            FileOpsSettings = fileOpsSettings;
        }

        public abstract string SaveUploadedFile(string httpFileName, Stream inputStream);

        public abstract bool Delete(string path);

        public abstract bool FileExists(string path);

        public abstract Stream StreamFile(string path);

        protected string GetFullFileSavePath(string filename)
        {
            var absolutePath = Path.Combine(FileOpsSettings.FileRepo, filename);
            return absolutePath;
        }
    }
}