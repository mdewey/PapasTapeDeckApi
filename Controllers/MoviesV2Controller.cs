using Amazon.S3;
using Amazon.S3.Model;
using BookStoreApi.Services;
using DeweyHomeMovieApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace DeweyHomeMovieApi
{
  [Route("api/v2/movies")]
  [ApiController]
  public class MoviesV2Controller : ControllerBase
  {
    private readonly MovieService _movieService;
    private readonly IAmazonS3 _s3Client;

    private IConfiguration Configuration;

    public MoviesV2Controller(IConfiguration _configuration, MovieService service, IAmazonS3 s3Client)
    {
      _s3Client = s3Client;
      _movieService = service;
      Configuration = _configuration;
    }
    // GET: api/Movies
    [HttpGet]
    public async Task<ActionResult> Get()
    {
      return Ok(await this._movieService.Get());
    }

    // GET: api/Movies/5
    [HttpGet("{id}")]
    public async Task<ActionResult> Get(string id)
    {
      var movie = await this._movieService.Get(id);
      if (movie == null)
      {
        return NotFound();
      }
      return Ok(movie);
    }

    // GET: api/Movies/string-id/video
    [HttpGet("{id}/video")]
    public async Task<ActionResult> GetVideo(string id)
    {
      var movie = await this._movieService.Get(id);

      var urlRequest = new GetPreSignedUrlRequest()
      {
        BucketName = Configuration["AWS:BUCKET"],
        Key = movie.AwsKey,
        Expires = DateTime.UtcNow.AddMinutes(10),
      };
      return Ok(new { id, VideoUrl = _s3Client.GetPreSignedURL(urlRequest) });
    }

    [HttpGet("{id}/image")]
    public async Task<ActionResult> GetImage(string id)
    {
      Console.WriteLine(id);
      var movie = await this._movieService.Get(id);
      var request = new GetObjectRequest { BucketName = Configuration["AWS:BUCKET"], Key = movie.AwsImageKey };
      var response = await this._s3Client.GetObjectAsync(request);
      return new FileStreamResult(response.ResponseStream, "image/jpeg") { FileDownloadName = movie.AwsImageKey };

    }


    // POST: api/Movies
    [HttpPost]
    public async Task<ActionResult> Post(Movie movie)
    {
      movie.Id = String.Empty;
      return Ok(await this._movieService.Insert(movie));
    }

  }
}
