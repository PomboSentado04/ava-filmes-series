using Microsoft.EntityFrameworkCore;
using API.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Configuração do DbContext
builder.Services.AddDbContext<AppDataContext>(options =>
    options.UseSqlite("Data Source=filmes.db"));

// Configuração da API
builder.Services.AddControllers();

// Habilita o Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Ignora referências circulares
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

var app = builder.Build();

// Middlewares
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Endpoints para Filmes
app.MapGet("/filmes", async (AppDataContext context) =>
    await context.Filmes.ToListAsync());

app.MapGet("/filmes/{id}", async (int id, AppDataContext context) =>
    await context.Filmes.FindAsync(id) is Filme filme ? Results.Ok(filme) : Results.NotFound());

app.MapPost("/filmes", async (Filme filme, AppDataContext context) =>
{
    context.Filmes.Add(filme);
    await context.SaveChangesAsync();
    return Results.Created($"/filmes/{filme.Id}", filme);
});

app.MapPut("/filmes/{id}", async (int id, Filme filmeAtualizado, AppDataContext context) =>
{
    var filme = await context.Filmes.FindAsync(id);
    if (filme is null) return Results.NotFound();

    filme.Titulo = filmeAtualizado.Titulo;
    filme.Diretor = filmeAtualizado.Diretor;
    filme.Ano = filmeAtualizado.Ano;
    await context.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/filmes/{id}", async (int id, AppDataContext context) =>
{
    var filme = await context.Filmes.FindAsync(id);
    if (filme is null) return Results.NotFound();

    context.Filmes.Remove(filme);
    await context.SaveChangesAsync();
    return Results.NoContent();
});

// Endpoints para Usuários
app.MapGet("/usuarios", async (AppDataContext context) =>
    await context.Usuarios.ToListAsync());

app.MapGet("/usuarios/{id}", async (int id, AppDataContext context) =>
    await context.Usuarios.FindAsync(id) is Usuario usuario ? Results.Ok(usuario) : Results.NotFound());

app.MapPost("/usuarios", async (Usuario usuario, AppDataContext context) =>
{
    context.Usuarios.Add(usuario);
    await context.SaveChangesAsync();
    return Results.Created($"/usuarios/{usuario.Id}", usuario);
});

// Endpoints para Avaliações
app.MapGet("/avaliacoes", async (AppDataContext context) =>
{
    var avaliacoes = await context.Avaliacoes
        .Select(a => new 
        {
            a.Id,
            a.Nota,
            a.Comentario,
            Filme = new { a.Filme.Id, a.Filme.Titulo },
            Usuario = new { a.Usuario.Id, a.Usuario.Nome }
        })
        .ToListAsync();
    
    return Results.Ok(avaliacoes);
});

app.MapGet("/avaliacoes/{id}", async (int id, AppDataContext context) =>
{
    var avaliacao = await context.Avaliacoes
        .Include(a => a.Filme)
        .Include(a => a.Usuario)
        .FirstOrDefaultAsync(a => a.Id == id);
    
    return avaliacao is not null ? Results.Ok(avaliacao) : Results.NotFound();
});

app.MapPost("/avaliacoes", async (Avaliacao avaliacao, AppDataContext context) =>
{
    // Validação simplificada
    if (avaliacao.Nota < 1 || avaliacao.Nota > 5)
        return Results.BadRequest("Nota deve ser entre 1 e 5");

    context.Avaliacoes.Add(avaliacao);
    await context.SaveChangesAsync();
    return Results.Created($"/avaliacoes/{avaliacao.Id}", avaliacao);
});

app.MapDelete("/avaliacoes/{id}", async (int id, AppDataContext context) =>
{
    var avaliacao = await context.Avaliacoes.FindAsync(id);
    if (avaliacao is null) return Results.NotFound();

    context.Avaliacoes.Remove(avaliacao);
    await context.SaveChangesAsync();
    return Results.NoContent();
});

// Cria o banco de dados se não existir
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDataContext>();
    db.Database.EnsureCreated();
}

app.Run();