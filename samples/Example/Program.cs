using Backworker;
using Backworker.Database.Postgres;
using Example;
using Example.Backworker;
using Example.Backworker.Tasks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging((loggingBuilder) => loggingBuilder
    .SetMinimumLevel(LogLevel.Trace)
    .AddConsole(options =>
         {
             options.TimestampFormat = "HH:mm:ss ";
         })
);

builder.Services.UseBackworker(typeof(SayHelloTask).Assembly)
    .ConfigureBackworker(backworkerBuilder =>
        backworkerBuilder
            .AddPostgres()
            .WithGlobalConnectionString(Environment.GetEnvironmentVariable("POSTGRES_CONNECTION_STRING")));


WebApplication app = builder.Build();

app.Run();

