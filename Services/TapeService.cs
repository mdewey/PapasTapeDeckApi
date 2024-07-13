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

  public async Task<Tape> Insert(Tape tape)
  {
    var id = "tape-" + Guid.NewGuid().ToString();
    tape.Id = id;
    Console.WriteLine(tape);
    var request = new PutItemRequest
    {
      TableName = this._settings.TapeCollectionName,
      Item = tape.CreateAttributeList()
    };
    await _dynamoClient.PutItemAsync(request);
    return tape;
  }

  public async Task<Tape> Update(Tape tape){
    var request = new UpdateItemRequest
    {
      TableName = this._settings.TapeCollectionName,
      Key = new Dictionary<string, AttributeValue>()
            {
                { "id", new AttributeValue {
                      S = tape.Id
                  } }
            },
      ExpressionAttributeNames = new Dictionary<string, string>
            {
                { "#T", "title" },
                { "#F", "filePath" },
                { "#U", "url" },
                { "#L", "length" },
                { "#I", "imageUrl" },
                { "#V", "version" },
                { "#T", "tags" },
                { "#A", "audioTimeStamps" }
            },
      ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":t", new AttributeValue { S = tape.Title } },
                { ":f", new AttributeValue { S = tape.FilePath } },
                { ":u", new AttributeValue { S = tape.Url } },
                { ":l", new AttributeValue { S = tape.Length } },
                { ":i", new AttributeValue { S = tape.ImageUrl } },
                { ":v", new AttributeValue { N = tape.Version.ToString() } },
                { ":t", new AttributeValue { SS = tape.Tags?.ToList() } },
                { ":a", new AttributeValue { L = tape.AudioTimeStamps?.Select(x => new AttributeValue
                {
                  M = new Dictionary<string, AttributeValue>
                  {
                    { "description", new AttributeValue { S = x.Description } },
                    { "timeStamp", new AttributeValue { S = x.TimeStamp } }
                  }
                }).ToList() } }
            },
      UpdateExpression = "SET #T = :t, #F = :f, #U = :u, #L = :l, #I = :i, #V = :v, #T = :t, #A = :a",
      ReturnValues = "ALL_NEW"
    };
    var response = await _dynamoClient.UpdateItemAsync(request);
    return new Tape().fromAttributeList(response.Attributes);
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