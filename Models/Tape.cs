using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using Amazon.DynamoDBv2.Model;

namespace DadsTapesApi.Models;



public class AudioTimeStamp
{
  public string? Description { get; set; }
  public string? TimeStamp { get; set; }
}

public class Tape
{
  public string? Id { get; set; }

  public string? Title { get; set; }

  public string? AwsKey { get; set; }

  public string? AwsImageKey { get; set; }

  public string? FilePath { get; set; }

  public string? Url { get; set; }

  public string? Length { get; set; }

  public string? ImageUrl { get; set; }

  public int Version { get; set; } = 1;

  public List<String>? Tags { get; set; } = new List<String>();

  public List<AudioTimeStamp>? AudioTimeStamps { get; set; } = new List<AudioTimeStamp>();


  public Tape fromAttributeList(Dictionary<string, AttributeValue> attributeList)
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
        case "audioTimeStamps":
          AudioTimeStamps = value.L.Select(x => new AudioTimeStamp
          {
            Description = x.M["description"].S,
            TimeStamp = x.M["timeStamp"].S
          }).ToList();
          break;
      }
    }
    return this;
  }

  public Dictionary<string, AttributeValue> CreateAttributeList()
  {
    var data =  new Dictionary<string, AttributeValue>()
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
            };
    if(this.AwsImageKey != null){
      data.Add("awsImageKey", new AttributeValue
      {
        S = this.AwsImageKey
      });
    }

    if (this.Tags != null && this.Tags.Count > 0)
    {
      data.Add("tags", new AttributeValue
      {
        SS = this.Tags.ToList()
      });
    }
    
    if (this.AudioTimeStamps != null && this.AudioTimeStamps.Count > 0){
      data.Add("audioTimeStamps", new AttributeValue
      {
        L = this.AudioTimeStamps.Select(x => new AttributeValue
        {
          M = new Dictionary<string, AttributeValue>
          {
            { "description", new AttributeValue { S = x.Description } },
            { "timeStamp", new AttributeValue { S = x.TimeStamp } }
          }
        }).ToList()
      });
    }

    return data;

  }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}