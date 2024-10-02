using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using RunGroopWebApp.Helpers;
using RunGroopWebApp.Interfaces;

namespace RunGroopWebApp.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly Cloudinary _cloudinary;
        public PhotoService(IOptions<CloudinarySettings> config) {
            var acc = new Account
               (
                   config.Value.CloudName,
                   config.Value.ApiKey,
                   config.Value.ApiSecret
               );
            _cloudinary = new Cloudinary(acc);
        }
        public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
        {
            var uploadRes=new ImageUploadResult();
            if (file.Length > 0)
            {
                using var stream=file.OpenReadStream();
                var uploadParam = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation=new Transformation().Height(500).Width(500).Crop("fill").Gravity("face")
                };
                uploadRes= await _cloudinary.UploadAsync(uploadParam);
            }
            return uploadRes;
        }

        public async Task<DeletionResult>DeletePhotoAsync(string publicId)
        {
            var deleteParam=new DeletionParams(publicId);   
            var res=await _cloudinary.DestroyAsync(deleteParam);
            return res;


        }
    }
}
