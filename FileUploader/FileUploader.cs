using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FileUploader;

public class FileUploader(ILogger<FileUploader> logger, BlobServiceClient blobServiceClient)
{
    [Function("FileUploader")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        logger.LogInformation("FileUploader received request.");
        // Parse the request
        var file = req.Form.Files["file"];

        if (file == null || file.Length == 0)
        {
            return new BadRequestObjectResult("No file received.");
        }

        var containerClient = blobServiceClient.GetBlobContainerClient("publisher-files");
        var newBlobName = $"{Guid.NewGuid()}/{file.FileName}";
        var blobClient = containerClient.GetBlobClient(newBlobName);

        var fileStream = file.OpenReadStream();
        await blobClient.UploadAsync(fileStream, true);

        return new OkObjectResult($"File {newBlobName} uploaded successfully.");
    }
}