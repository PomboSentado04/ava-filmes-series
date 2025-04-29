using Microsoft.EntityFrameworkCore;
using API.Models;

public class AppDataContext : DbContext
{
    // Construtor necessário para injeção de dependência
    public AppDataContext(DbContextOptions<AppDataContext> options) : base(options) { }

    // Tabelas do banco de dados
    public DbSet<Filme> Filmes { get; set; }
    public DbSet<Avaliacao> Avaliacoes { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configuração do relacionamento Filme -> Avaliacao (1:N)
        modelBuilder.Entity<Filme>()
            .HasMany(f => f.Avaliacoes)     // Um Filme tem muitas Avaliacoes
            .WithOne(a => a.Filme)          // Cada Avaliacao pertence a um Filme
            .HasForeignKey(a => a.FilmeId)  // Chave estrangeira em Avaliacao
            .OnDelete(DeleteBehavior.Cascade); // Se um Filme for deletado, suas avaliações também serão

        // Configuração do relacionamento Usuario -> Avaliacao (1:N)
        modelBuilder.Entity<Usuario>()
            .HasMany(u => u.Avaliacoes)     // Um Usuario tem muitas Avaliacoes
            .WithOne(a => a.Usuario)        // Cada Avaliacao pertence a um Usuario
            .HasForeignKey(a => a.UsuarioId) // Chave estrangeira em Avaliacao
            .OnDelete(DeleteBehavior.Cascade); // Opcional: define o comportamento de deleção
    }
}