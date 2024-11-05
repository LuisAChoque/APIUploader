using WebApplication1.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
namespace WebApplication1.Services
{
    public class FileStorageDataService : IFileStorageDataService
    {
        private readonly ApplicationDbContext _context;
        public FileStorageDataService(ApplicationDbContext context)
        {
            _context = context;
        }
        public IEnumerable<UserStat> GetUserDailyStats()
        {
            // Fecha actual
            DateTime today = DateTime.Today;

            var storageUsageToday = _context.Archivos
                                .Where(file => file.FechaSubida == today) // Filtrar archivos subidos hoy
                                .GroupBy(file => file.UsuarioId) // Agrupar por usuario
                                .Select(g => new UserStat
                                {
                                    Username = _context.Users
                                        .Where(user => user.Id == g.Key)
                                        .Select(user => user.Username)
                                        .FirstOrDefault(),
                                    StorageUsed = g.Sum(file => file.TamañoEnBytes) // Sumar el tamaño de archivos subidos hoy
                                })
                                .Where(result => result.Username != null) // Excluir usuarios no encontrados (precaución)
                                .ToList();
            return storageUsageToday;
        }

    }
}
