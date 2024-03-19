
using Microsoft.AspNetCore.Hosting.Server;
using System.Drawing;
using System.IO;

namespace RazeonProject.Helpers
{
    public enum Folders { ViewFiles = 0 }

    public class HelperWwwroot
    {
        internal IWebHostEnvironment hostEnvironment;

        public HelperWwwroot(IWebHostEnvironment hostEnvironment)
        {
            this.hostEnvironment = hostEnvironment;
        }

        public async Task BuildTemporalFileWwwrootAsync(string sourceFilePath, string folder)
        {
            try
            {
                sourceFilePath = Path.Combine(this.hostEnvironment.ContentRootPath , "Views" ,sourceFilePath);
                string wwwrootPath = Path.Combine(this.hostEnvironment.WebRootPath, folder ,Path.GetFileName(sourceFilePath));
                using (Stream source = File.Open(sourceFilePath, FileMode.Open))
                {
                    using (Stream destination = File.Create(wwwrootPath))
                    {
                        await source.CopyToAsync(destination);
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        public void BuildTemporalImgWwwroot(byte[] file, string folder, string fileName)
        {
            try
            {
                string wwwrootPath = Path.Combine(hostEnvironment.WebRootPath, folder, fileName + ".jpg");
                using (MemoryStream ms = new MemoryStream(file))
                {
                    using (Image image = Image.FromStream(ms))
                    {
                        image.Save(wwwrootPath, System.Drawing.Imaging.ImageFormat.Jpeg);
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        public async Task DeleteFilesFromWwwrootAsync(string fileName, string folder)
        {
            try
            {
                string wwwrootPath = Path.Combine(hostEnvironment.WebRootPath, folder, fileName);
                if (File.Exists(wwwrootPath))
                {
                    await Task.Run(() => File.Delete(wwwrootPath));
                }
            }
            catch (Exception)
            {
            }
        }

        public async Task RestoreWwwrootFolderAsync(string folder)
        {
            try
            {
                string wwwrootPath = Path.Combine(hostEnvironment.WebRootPath, folder);
                if (Directory.Exists(wwwrootPath))
                {
                    await Task.Run(() =>
                    {
                        foreach (FileSystemInfo fsi in new DirectoryInfo(wwwrootPath).GetFileSystemInfos())
                        {
                            if (fsi is DirectoryInfo)
                                ((DirectoryInfo)fsi).Delete(true);
                            else
                                try
                                {
                                    fsi.Delete();
                                }catch (Exception)
                                {
                                }
                        }
                    });
                }
            }
            catch (Exception)
            {
            }
        }

        public string GetFolderPathWwwroot(Folders folder)
        {
            string carpeta = "";

            if (folder == Folders.ViewFiles)
            {
                carpeta = "ViewFiles";
            }            

            return carpeta;
        }
    }
}
