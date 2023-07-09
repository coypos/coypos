var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(o => o.AddDefaultPolicy(
    policy =>
    {
        policy.WithOrigins(
            "https://smilginp.evolpe.net/",
            "http://localhost:8080/",
            "http://localhost:5016/",
            "https://localhost:7091/",
            "http://localhost:13966" // IIS
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    }));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// <> UseCors must be placed after UseRouting and before UseAuthorization. This is to ensure that CORS headers are included in the response for both authorized and unauthorized calls.
app.UseCors();
// </>

app.UseAuthorization();

app.MapControllers();

app.Run();