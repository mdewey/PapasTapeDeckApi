using DeweyHomeMovieApi.Models;

namespace BookStoreApi.Services;

public interface IMovieServices
{
  Task<List<Movie>> Get();
  Task<Movie> Get(string id);
  Task<object> GetAllTestDocs();
  Task<Movie> Insert(Movie movie);
}