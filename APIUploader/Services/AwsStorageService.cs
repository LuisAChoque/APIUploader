using Amazon.S3.Model;
using Amazon.S3;
using WebApplication1.Interfaces;
using Amazon.S3.Transfer;

namespace WebApplication1.Services
{
    public class AwsStorageService: IFileStorageService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName = "buckettesttec";

        public AwsStorageService(IAmazonS3 s3Client)
        {
            _s3Client = s3Client;
        }

        public async Task<string> UploadFileAsync(IFormFile file, string userId)
        {
            var key = $"{userId}/{Guid.NewGuid()}_{file.FileName}";
            /*var putRequest = new PutObjectRequest
            {
                InputStream = file.OpenReadStream(),
                Key = key,
                BucketName = _bucketName,
            };
            putRequest.Metadata.Add("Content-Type", file.ContentType);
            await _s3Client.PutObjectAsync(putRequest);
            return file.FileName;// key;*/

            var fileTransferUtility = new TransferUtility(_s3Client);
            var fileTransferUtilityRequest = new TransferUtilityUploadRequest
            {
                FilePath = file.FileName,
                Key = key,
                BucketName = _bucketName,
                CannedACL = S3CannedACL.PublicRead,
                StorageClass = S3StorageClass.StandardInfrequentAccess,
                InputStream = file.OpenReadStream()
            };
             await fileTransferUtility.UploadAsync(fileTransferUtilityRequest);
            return file.FileName;// key;

        }
    }
}
