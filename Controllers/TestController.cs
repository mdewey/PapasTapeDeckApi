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

    [HttpGet("site/get-env")]
    public ActionResult GetEnv()
    {
      return Ok(((IConfigurationRoot)_configuration).GetDebugView());
    }


   
  }
}
