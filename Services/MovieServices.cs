using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using DeweyHomeMovieApi.Models;
using Microsoft.Extensions.Options;

namespace BookStoreApi.Services;

public class MovieService : IMovieServices
{
  private readonly DynamoDbSettings _settings;

  private readonly AmazonDynamoDBClient _dynamoClient;

  public MovieService(
    IOptions<DynamoDbSettings> settings)
  {
    _settings = settings.Value;
    AmazonDynamoDBConfig clientConfig = new AmazonDynamoDBConfig();
    clientConfig.RegionEndpoint = RegionEndpoint.USEast1;
    _dynamoClient = new AmazonDynamoDBClient(clientConfig);

  }
  public async Task<List<Movie>> Get()
  {

    var request = new ScanRequest
    {
      TableName = _settings.MovieCollectionName,
    };

    var response = await _dynamoClient.ScanAsync(request);
    return response.Items.Select(s => new Movie().fromAttributeList(s)).ToList();
  }

  public async Task<Movie> Get(string id)
  {
    var request = new GetItemRequest
    {
      TableName = _settings.MovieCollectionName,
      Key = new Dictionary<string, AttributeValue>()
            {
                { "id", new AttributeValue {
                      S = id
                  } }
            }

    };

    var response = await _dynamoClient.GetItemAsync(request);
    var attributeList = response.Item;
    return new Movie().fromAttributeList(attributeList);
  }

  public async Task<Movie> Insert(Movie movie)
  {
    var id = "movie-" + Guid.NewGuid().ToString();
    movie.Id = id;
    var request = new PutItemRequest
    {
      TableName = this._settings.MovieCollectionName,
      Item = movie.CreateAttributeList()
    };
    await _dynamoClient.PutItemAsync(request);
    return movie;
  }

  public async Task<object> GetAllTestDocs()
  {
    var request = new GetItemRequest
    {
      TableName = _settings.TestCollectionName,
      Key = new Dictionary<string, AttributeValue>()
            {
                { "_id", new AttributeValue {
                      S = "test-id"
                  } }
            },

    };
    var response = await _dynamoClient.GetItemAsync(request);

    // Check the response.
    var attributeList = response.Item; // attribute list in the response.
    var rv = new List<object>();
    rv.Add(new Movie().fromAttributeList(attributeList));
    return rv;
  }
}