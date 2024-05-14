using DadsTapesApi.Models;


public interface ITapeServices
{
  Task<List<Tape>> Get();
  Task<Tape> Get(string id);
  Task<object> GetAllTestDocs();
  Task<Tape> Insert(Tape movie);
}