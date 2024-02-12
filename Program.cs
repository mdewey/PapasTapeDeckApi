using Amazon.S3;
using BookStoreApi.Services;
using DeweyHomeMovieApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
{
  config.AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", optional: true);
});


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.Configure<DynamoDbSettings>(
    builder.Configuration.GetSection("DynamoDbSettings"));

builder.Services.AddSingleton<MovieService>();

builder.Services.AddCors(options =>
{
  options.AddPolicy(name: "video-sites",
                    policy =>
                    {
                      policy.WithOrigins("http://localhost:4200", "https://home.videos.markdewey.dev/")
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowAnyOrigin();

                    });
});

builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
builder.Services.AddAWSService<IAmazonS3>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();


app.UseCors("video-sites");

// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
