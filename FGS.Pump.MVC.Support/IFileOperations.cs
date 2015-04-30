using System.Web;

namespace MVCSupport
{
    public interface IFileOperations
    {
        string SaveUploadedFile(HttpPostedFileBase uploadedFile);
        bool FileWasPosted(HttpPostedFileBase file);
        bool Delete(string path);
        string GetFullPathToRepoFile(string filename);
        bool FileExists(string path);
    }
}