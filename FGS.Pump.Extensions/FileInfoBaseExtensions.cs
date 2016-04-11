using System;
using System.IO.Abstractions;
using System.Threading.Tasks;

namespace FGS.Pump.Extensions
{
    public static class FileInfoBaseExtensions
    {
        public static async Task UseTemporaryFileAsync(this FileInfoBase temporaryFile, Func<FileInfoBase, Task> fileAction)
        {
            try
            {
                await fileAction(temporaryFile);
            }
            finally
            {
                temporaryFile.Delete();
            }
        }

        public static async Task<TReturnType> UseTemporaryFileAsync<TReturnType>(this FileInfoBase temporaryFile, Func<FileInfoBase, Task<TReturnType>> fileAction)
        {
            try
            {
                return await fileAction(temporaryFile);
            }
            finally
            {
                temporaryFile.Delete();
            }
        }

        public static void UseTemporaryFile(this FileInfoBase temporaryFile, Action<FileInfoBase> fileAction)
        {
            try
            {
                fileAction(temporaryFile);
            }
            finally
            {
                temporaryFile.Delete();
            }
        }

        public static TReturnType UseTemporaryFile<TReturnType>(this FileInfoBase temporaryFile, Func<FileInfoBase, TReturnType> fileAction)
        {
            try
            {
                return fileAction(temporaryFile);
            }
            finally
            {
                temporaryFile.Delete();
            }
        }
    }
}
