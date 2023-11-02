using Backworker;
using Backworker.Database.Postgres;
using Example.Backworker;
using Example.Backworker.Tasks;

IHostBuilder builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices(services =>
{
    services.UseBackworker(typeof(BackworkerTaskFactory), typeof(SayHelloTask).Assembly)
        .ConfigureBackworker(builder =>
            builder
                .AddPostgres()
                .WithGlobalConnectionString(Environment.GetEnvironmentVariable("POSTGRES_CONNECTION_STRING")));
});

builder.ConfigureWebHostDefaults(conf => conf.UseUrls("http://*:3000"));

IHost app = builder.Build();

app.Run();