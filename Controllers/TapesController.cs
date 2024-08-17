using Amazon.S3;
using Amazon.S3.Model;
using DadsTapesApi.Models;
using DadsTapesApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace DadsTapesApi
{
  [Route("api/v2/tapes")]
  [ApiController]
  public class TapesController : ControllerBase
  {
    private readonly TapeService _tapeService;
    private readonly IAmazonS3 _s3Client;

    private IConfiguration Configuration;

    public TapesController(IConfiguration _configuration, TapeService service, IAmazonS3 s3Client)
    {
      _s3Client = s3Client;
      _tapeService = service;
      Configuration = _configuration;
    }
    // GET: api/tapes
    [HttpGet]
    public async Task<ActionResult> Get()
    {
      return Ok(await this._tapeService.Get());
    }

    // GET: api/tapes/5
    [HttpGet("{id}")]
    public async Task<ActionResult> Get(string id)
    {
      var tape = await this._tapeService.Get(id);
      if (tape == null)
      {
        return NotFound();
      }
      return Ok(tape);
    }

    // GET: api/tapes/string-id/audio
    [HttpGet("{id}/audio")]
    public async Task<ActionResult> GetVideo(string id)
    {
      var tape = await this._tapeService.Get(id);
      
      var urlRequest = new GetPreSignedUrlRequest()
      {
        BucketName = Configuration["AWS:BUCKET"],
        Key = tape.AwsKey,
        Expires = DateTime.UtcNow.AddMinutes(10),
      };
      return Ok(new { id, AudioUrl = _s3Client.GetPreSignedURL(urlRequest) });
    }

    [HttpGet("{id}/image")]
    public async Task<ActionResult> GetImage(string id)
    {
      Console.WriteLine(id);
      var tape = await this._tapeService.Get(id);
      var request = new GetObjectRequest { BucketName = Configuration["AWS:BUCKET"], Key = tape.AwsImageKey };
      var response = await this._s3Client.GetObjectAsync(request);
      return new FileStreamResult(response.ResponseStream, "image/jpeg") { FileDownloadName = tape.AwsImageKey };
    }


    // POST: api/tapes
    [HttpPost]
    public async Task<ActionResult> Post(Tape tape)
    {
      tape.Id = String.Empty;
      return Ok(await this._tapeService.Insert(tape));
    }

    public class TagViewModel{
      public string Tag { get; set; }
    }

    [HttpPost("{id}/tag")]
    public async Task<ActionResult> AddTag(string id,[FromBody] TagViewModel vm){
      var tape = await this._tapeService.Get(id);
      if(tape == null ){
        return NotFound();
      }
      if (tape.Tags == null){
        tape.Tags = new List<String>();
      }
      tape.Tags.Add(vm.Tag);
      await this._tapeService.Update(tape);
      return Ok(new {id, vm, tape});
    }

  }
}
