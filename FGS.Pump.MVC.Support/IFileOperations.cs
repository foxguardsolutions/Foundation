using System.Web;

namespace FGS.Pump.MVC.Support
{
    public abstract class AFileOperations
    {
        public abstract string SaveUploadedFile(HttpPostedFileBase uploadedFile);

        public  bool FileWasPosted(HttpPostedFileBase file)
        {
            return file != null && file.ContentLength > 0;
        }

        public abstract bool Delete(string path);

        public abstract string GetFullPathToRepoFile(string filename);

        public abstract bool FileExists(string path);
    }
}