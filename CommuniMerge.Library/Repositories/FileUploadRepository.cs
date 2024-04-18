using CommuniMerge.Library.Enums;
using CommuniMerge.Library.Loggers.Interfaces;
using CommuniMerge.Library.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommuniMerge.Library.Repositories
{
    public class FileUploadRepository : IFileUploadRepository
    {
        private readonly ICustomLogger logger;

        public FileUploadRepository(ICustomLogger logger)
        {
            this.logger = logger;
        }
        public async Task<FileType> GetFileType(IFormFile formFile)
        {
            var images = new[] { "image/jpeg", "image/png", "image/jpg" };
            var videos = new[] { "video/mp4", "video/webm", "video/ogg" };

            FileType fileType = FileType.Image;

            if (images.Contains(formFile.ContentType))
            {
                fileType = FileType.Image;
            }
            else if (videos.Contains(formFile.ContentType))
            {
                fileType = FileType.Video;
            }
            return fileType;
        }

        public async Task<bool> IsValidFileType(IFormFile formFile)
        {
            var allowedContentTypes = new[]
            {
                "image/jpeg", "image/png", "image/jpg",
                "video/mp4", "video/webm", "video/ogg", "video/mkv", "video/x-matroska"
            };

            return allowedContentTypes.Contains(formFile.ContentType);
        }
        public async Task<string?> UploadFile(IFormFile file, FileType fileType)
        {

            try
            {


                var path = "";

                if(fileType == FileType.Image)
                {
                    path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "..\\CommuniMerge\\wwwroot\\img"));
                }
                else if(fileType == FileType.Video)
                {
                    path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "..\\CommuniMerge\\wwwroot\\vid"));
                }
                else
                {
                    throw new ArgumentException("File type not supported.");
                }

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + file.FileName;

                string filePath = Path.Combine(path, uniqueFileName);

                using(var fileStram = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStram);
                }


                return uniqueFileName;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, GetType().Name, nameof(UploadFile));
                return null;
            }
        }





    }
}
