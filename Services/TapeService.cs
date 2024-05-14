using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using DadsTapesApi.Models;
using Microsoft.Extensions.Options;

namespace DadsTapesApi.Services;

public class TapeService : ITapeServices
{
  private readonly DynamoDbSettings _settings;

  private readonly AmazonDynamoDBClient _dynamoClient;

  public TapeService(
    IOptions<DynamoDbSettings> settings)
  {
    _settings = settings.Value;
    AmazonDynamoDBConfig clientConfig = new AmazonDynamoDBConfig();
    clientConfig.RegionEndpoint = RegionEndpoint.USEast1;
    _dynamoClient = new AmazonDynamoDBClient(clientConfig);

  }
  public async Task<List<Tape>> Get()
  {

    var request = new ScanRequest
    {
      TableName = _settings.TapeCollectionName,
    };

    var response = await _dynamoClient.ScanAsync(request);
    return response.Items.Select(s => new Tape().fromAttributeList(s)).ToList();
  }

  public async Task<Tape> Get(string id)
  {
    var request = new GetItemRequest
    {
      TableName = _settings.TapeCollectionName,
      Key = new Dictionary<string, AttributeValue>()
            {
                { "id", new AttributeValue {
                      S = id
                  } }
            }

    };

    var response = await _dynamoClient.GetItemAsync(request);
    var attributeList = response.Item;
    return new Tape().fromAttributeList(attributeList);
  }

  public async Task<Tape> Insert(Tape movie)
  {
    var id = "movie-" + Guid.NewGuid().ToString();
    movie.Id = id;
    var request = new PutItemRequest
    {
      TableName = this._settings.TapeCollectionName,
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
    rv.Add(new Tape().fromAttributeList(attributeList));
    return rv;
  }
}