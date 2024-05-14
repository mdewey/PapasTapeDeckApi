using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Mvc;
using DadsTapesApi.Services;

namespace DadsTapesApi
{
  [Route("api/[controller]")]
  [ApiController]
  public class TestController : ControllerBase
  {

    private readonly TapeService _tapeService;
    private readonly IAmazonS3 _s3Client;
    private readonly IConfiguration _configuration;

    public TestController(IConfiguration configuration, IAmazonS3 s3Client, TapeService serviceV2)
    {
      _s3Client = s3Client;
      _tapeService = serviceV2;
      _configuration = configuration;
    }

    [HttpGet("ping")]
    public ActionResult Ping()
    {
      return Ok("pong");
    }

    [HttpGet("dynamo/testdocs")]
    public async Task<ActionResult> GetDynamoTestDocs()
    {
      return Ok(await this._tapeService.GetAllTestDocs());
    }

    [HttpGet("site/get-env")]
    public ActionResult GetEnv()
    {
      return Ok(((IConfigurationRoot)_configuration).GetDebugView());
    }

    [HttpGet("aws/get-all")]
    public async Task<IActionResult> GetAllBucketAsync()
    {
      var data = await _s3Client.ListBucketsAsync();
      var buckets = data.Buckets.Select(b => { return b.BucketName; });
      return Ok(buckets);
    }

    [HttpGet("aws/get-configured-bucket")]
    public async Task<IActionResult> GetConfiguredBucketAsync()
    {
      var bucket = _configuration["AWS:BUCKET"];
      try
      {
        var data = await _s3Client.ListObjectsAsync(bucket);
        var buckets = data.S3Objects.Select(b => { return b.Key; });
        return Ok(new { bucket, buckets });
      }
      catch (Exception e)
      {
        return Ok(new { bucket, e.Message });
      }
    }

    public class S3ObjectDto
    {
      public string? Name { get; set; }
      public string? PresignedUrl { get; set; }
    }

    [HttpGet("aws/get-all-full")]
    public async Task<IActionResult> GetAllFilesAsync(string bucketName = "deweys-home-video-bucket-dev", string? prefix = "xyz")
    {
      var bucketExists = await _s3Client.DoesS3BucketExistAsync(bucketName);
      if (!bucketExists) return NotFound($"Bucket {bucketName} does not exist.");
      var request = new ListObjectsV2Request()
      {
        BucketName = bucketName,
        Prefix = prefix
      };
      var result = await _s3Client.ListObjectsV2Async(request);
      var s3Objects = result.S3Objects.Select(s =>
      {
        var urlRequest = new GetPreSignedUrlRequest()
        {
          BucketName = bucketName,
          Key = s.Key,
          Expires = DateTime.UtcNow.AddMinutes(1)
        };
        return new S3ObjectDto()
        {
          Name = s.Key.ToString(),
          PresignedUrl = _s3Client.GetPreSignedURL(urlRequest),
        };
      });
      return Ok(s3Objects);
    }

  }
}
