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

        public virtual bool FileWasPosted(HttpPostedFileBase file)
        {
            return file?.ContentLength != null && file.ContentLength > 0;
        }

        public abstract bool Delete(string path);

        public string GetFullPathToRepoFile(string filename)
        {
            var rawFileName = Path.GetFileName(filename);
            if (rawFileName != null) return Path.Combine(FileOpsSettings.FileRepo, rawFileName);
            throw new InvalidFileNameException(
                string.Format("The filename \"{0}\" could not be parsed by the filesystem.", filename));
        }

        public abstract bool FileExists(string path);

        public abstract Stream StreamFile(string path);

        protected string GetFullFileSavePath(string filename)
        {
            var absolutePath = Path.Combine(FileOpsSettings.FileRepo, filename);
            return absolutePath;
        }
    }
}