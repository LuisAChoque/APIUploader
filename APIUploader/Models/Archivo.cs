namespace WebApplication1.Models
{
    public class Archivo
    {
        public string? Id { get; set; }
        public string? UsuarioId { get; set; }

        public string? Path { get; set; }

        public long TamañoEnBytes { get; set; }
        public DateTime FechaSubida { get; set; }
    }
}
