namespace API.Models;

public class Filme
{
    public int Id { get; set; }
    public string Titulo { get; set; }
    public string Diretor { get; set; }
    public int Ano { get; set; }
    public List<Avaliacao> Avaliacoes { get; set; } = new();
}