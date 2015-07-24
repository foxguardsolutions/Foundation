using System;
using System.IO;
using System.Web;
using FGS.Pump.Settings.FileOpsSettings;

namespace FGS.Pump.MVC.Support
{
    public abstract class AFileOperations
    {
        protected IFileOpsSettings _fileOpsSettings;

        protected AFileOperations(IFileOpsSettings fileOpsSettings)
        {
            _fileOpsSettings = fileOpsSettings;
        }

        public abstract string SaveUploadedFile(HttpPostedFileBase uploadedFile);

        public  bool FileWasPosted(HttpPostedFileBase file)
        {
            return file != null && file.ContentLength > 0;
        }

        public abstract bool Delete(string path);

        public string GetFullPathToRepoFile(string filename)
        {
            var rawFileName = Path.GetFileName(filename);
            if (rawFileName != null) return Path.Combine(_fileOpsSettings.FileRepo, rawFileName);
            throw new InvalidFileNameException(
                string.Format("The filename \"{0}\" could not be parsed by the filesystem.", filename));
        }

        public abstract bool FileExists(string path);

        public abstract Stream StreamFile(string path);
    }

    public class InvalidFileNameException : Exception
    {
        public InvalidFileNameException(string message) : base(message)
        {
            
        }
    }
}