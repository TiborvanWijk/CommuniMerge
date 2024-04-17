using CommuniMerge.Library.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommuniMerge.Library.Repositories.Interfaces
{
    public interface IFileUploadRepository
    {
        Task<string?> UploadFile(IFormFile file, FileType fileType);
        Task<FileType> GetFileType(IFormFile formFile);
        Task<bool> IsValidFileType(IFormFile formFile);
    }
}
