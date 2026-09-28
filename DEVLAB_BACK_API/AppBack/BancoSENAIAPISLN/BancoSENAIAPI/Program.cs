using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using BancoSENAIAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseMySql(connectionString,
    new MySqlServerVersion(new Version(7 , 0, 0)))
);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Banco SENAI - Sistema Financeiro",
        Version = "v1",
        Description = "API Gestão Financeira e Integração Clientes."
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirTudo",
        policy => policy.AllowAnyOrigin() // Permite a 'origin null' do seu arquivo local
                        .AllowAnyMethod() // Permite os verbos GET, POST, PUT, DELETE [2]
                        .AllowAnyHeader()); // Permite o envio de JSON no corpo da mensagem [3]
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("PermitirTudo");

app.UseAuthorization();

app.MapControllers();

app.Run();
