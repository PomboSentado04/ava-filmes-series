namespace API.Models;
public class Avaliacao
{
    public int Id { get; set; }
    public int Nota { get; set; } // 1 a 5
    public string Comentario { get; set; }
    public DateTime DataAvaliacao { get; set; } = DateTime.Now;

    // Relação N:1 com Filme
    public int FilmeId { get; set; }
    public Filme Filme { get; set; }

    // Relação N:1 com Usuario
    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; }
}