using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace BLL.Services.Implementations
{
    public class FileService : IFileService
    {
        private readonly IHostEnvironment _env;

        public FileService(IHostEnvironment env)

        {
            _env = env;
        }

        public async Task<string> SaveFileAsync(IFormFile file, string folderPath)
        {
            if (file == null || file.Length == 0)
                return null;

            if (!IsImage(file))
                throw new ArgumentException("Only image files are allowed.");

            if (file.Length > 5 * 1024 * 1024)
                throw new ArgumentException("File size exceeds 5MB.");

            var fullFolderPath = Path.Combine(_env.ContentRootPath, folderPath);
            if (!Directory.Exists(fullFolderPath))
                Directory.CreateDirectory(fullFolderPath);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(fullFolderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Path.Combine(folderPath, fileName).Replace("\\", "/");
        }

        public async Task<List<string>> SaveFilesAsync(List<IFormFile> files, string folderPath)
        {
            var savedPaths = new List<string>();

            if (files == null || !files.Any())
                return savedPaths;

            foreach (var file in files)
            {
                if (file != null && file.Length > 0)
                {
                    var path = await SaveFileAsync(file, folderPath);
                    if (!string.IsNullOrEmpty(path))
                        savedPaths.Add(path);
                }
            }

            return savedPaths;
        }

        public bool DeleteFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return false;

            var fullPath = Path.Combine(_env.ContentRootPath, filePath);
            if (!File.Exists(fullPath))
                return false;

            File.Delete(fullPath);
            return true;
        }

        private bool IsImage(IFormFile file)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp" };
            var extension = Path.GetExtension(file.FileName).ToLower();
            return allowedExtensions.Contains(extension);
        }




    }
}