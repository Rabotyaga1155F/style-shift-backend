using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading.Tasks;

namespace StyleShiftBackend.Services.YandexStorage
{
    public class YandexStorageService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;

        public YandexStorageService(IOptions<YandexStorageSettings> settings)
        {
            _bucketName = settings.Value.BucketName;

            _s3Client = new AmazonS3Client(settings.Value.AccessKey, settings.Value.SecretKey, new AmazonS3Config
            {
                ServiceURL = "https://storage.yandexcloud.net",
                ForcePathStyle = true
            });
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType)
        {
            var objectKey = $"products/{Guid.NewGuid()}_{fileName}";

            try
            {
                var putRequest = new PutObjectRequest
                {
                    BucketName = _bucketName,
                    Key = objectKey,
                    InputStream = fileStream,
                    ContentType = contentType,
                    CannedACL = S3CannedACL.PublicRead
                };

                await _s3Client.PutObjectAsync(putRequest);

                return $"https://{_bucketName}.storage.yandexcloud.net/{objectKey}";
            }
            catch (AmazonS3Exception ex)
            {
                throw new Exception($"Ошибка загрузки файла: {ex.Message}");
            }
        }

        public async Task<bool> DeleteFileAsync(string fileKey)
        {
            try
            {
                var deleteRequest = new DeleteObjectRequest
                {
                    BucketName = _bucketName,
                    Key = fileKey
                };

                var response = await _s3Client.DeleteObjectAsync(deleteRequest);
                return response.HttpStatusCode == System.Net.HttpStatusCode.NoContent;
            }
            catch (AmazonS3Exception ex)
            {
                throw new Exception($"Ошибка удаления файла: {ex.Message}");
            }
        }
    }
}
