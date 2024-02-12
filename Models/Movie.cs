using Amazon.DynamoDBv2.Model;

namespace DeweyHomeMovieApi.Models;



public class VideoTimeStamp
{
  public string? Description { get; set; }
  public string? TimeStamp { get; set; }
}

public class Movie
{
  public string? Id { get; set; }

  public string? Title { get; set; }

  public string? AwsKey { get; set; }

  public string? AwsImageKey { get; set; }

  public string? FilePath { get; set; }

  public string? Url { get; set; }

  public string? Length { get; set; }

  public string? ImageUrl { get; set; }

  public int Version { get; set; } = 7;

  public IEnumerable<String>? Tags { get; set; }

  public IEnumerable<VideoTimeStamp>? VideoTimeStamps { get; set; }


  public Movie fromAttributeList(Dictionary<string, AttributeValue> attributeList)
  {
    // TODO: for when I get board, refactor this
    foreach (KeyValuePair<string, AttributeValue> kvp in attributeList)
    {
      string attributeName = kvp.Key;
      AttributeValue value = kvp.Value;

      switch (attributeName)
      {
        case "id":
          Id = value.S;
          break;
        case "title":
          Title = value.S;
          break;
        case "awsKey":
          AwsKey = value.S;
          break;
        case "awsImageKey":
          AwsImageKey = value.S;
          break;
        case "filePath":
          FilePath = value.S;
          break;
        case "url":
          Url = value.S;
          break;
        case "length":
          Length = value.S;
          break;
        case "imageUrl":
          ImageUrl = value.S;
          break;
        case "version":
          Version = int.Parse(value.N);
          break;
        case "tags":
          Tags = value.SS;
          break;
        case "videoTimeStamps":
          VideoTimeStamps = value.L.Select(x => new VideoTimeStamp
          {
            Description = x.M["description"].S,
            TimeStamp = x.M["timeStamp"].S
          });
          break;
      }
    }
    return this;
  }

  public Dictionary<string, AttributeValue> CreateAttributeList()
  {
    return new Dictionary<string, AttributeValue>()
            {
                { "id", new AttributeValue {
                      S = this.Id
                  }},
                { "title", new AttributeValue {
                      S = this.Title
                  }},
                { "awsKey", new AttributeValue {
                      S = this.AwsKey
                  }},
                { "awsImageKey", new AttributeValue {
                      S = this.AwsImageKey
                  }},
                { "filePath", new AttributeValue {
                      S = this.FilePath
                  }},
                { "url", new AttributeValue {
                      S = this.Url
                  }},
                { "length", new AttributeValue {
                      S = this.Length
                  }},
                { "imageUrl", new AttributeValue {
                      S = this.ImageUrl
                  }},
                { "version", new AttributeValue {
                      N = this.Version.ToString()
                  }},
                { "tags", new AttributeValue {
                      SS = this.Tags?.ToList()
                  }},
                { "videoTimeStamps", new AttributeValue {
                      L = this.VideoTimeStamps?.Select(x => new AttributeValue
                      {
                        M = new Dictionary<string, AttributeValue>
                        {
                          { "description", new AttributeValue { S = x.Description } },
                          { "timeStamp", new AttributeValue { S = x.TimeStamp } }
                        }
                      }).ToList()
                  }},
            };




  }
}