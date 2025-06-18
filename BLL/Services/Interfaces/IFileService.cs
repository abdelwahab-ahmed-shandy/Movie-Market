using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IFileService
    {
        Task<string> SaveFileAsync(IFormFile file, string folderPath);
        
        //Multiple files
        Task<List<string>> SaveFilesAsync(List<IFormFile> files, string folderPath); 
        bool DeleteFile(string filePath);
    }
}
