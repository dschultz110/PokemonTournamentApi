using PokemonTournament.Core.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<IPokeApiClient, PokeApiClient>(client =>
{
    var baseUrl = builder.Configuration["PokeApi:BaseUrl"]
        ?? throw new InvalidOperationException("Missing configuration value 'PokeApi:BaseUrl'.");
    client.BaseAddress = new Uri(baseUrl);
});

builder.Services.AddSingleton<IBattleResolver, BattleResolver>();
builder.Services.AddScoped<ITournamentService, TournamentService>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
         policy.WithOrigins("http://localhost:4200") // our Angular app runs on this port
               .AllowAnyHeader()
               .AllowAnyMethod());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseCors();

app.Run();
