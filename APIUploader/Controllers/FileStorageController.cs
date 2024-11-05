using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.InMemory.Storage.Internal;
using System.Security.Claims;
using WebApplication1.Data;
using WebApplication1.Interfaces;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FileStorageController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private const long LimiteEspacio = 5L * 1024 * 1024 * 1024; // 5 GB en bytes
        private readonly IFileStorageService _awsService;
        private readonly IFileStorageService _azureService;

        public FileStorageController(IFileStorageService awsService, IFileStorageService azureService, ApplicationDbContext context)
        {
            _awsService = awsService;
            _azureService = azureService;
            _context = context;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            // Verificar si el usuario está autenticado
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("User must be logged in to upload files.");
            }

            // Verificar si se proporcionó un archivo
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file provided.");
            }

            // Obtener el usuario desde la base de datos
            User? user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Verificar el espacio utilizado durante el mes
            var monthStorageUsed = _context.Archivos
                                    .Where(a => a.UsuarioId == userId &&
                                                a.FechaSubida.Month == DateTime.Now.Month &&
                                                a.FechaSubida.Year == DateTime.Now.Year)
                                    .Sum(a => a.TamañoEnBytes);

            if (monthStorageUsed + file.Length > LimiteEspacio)
            {
                return BadRequest("Storage limit exceeded.");
            }

            // Crear nuevo objeto Archivo
            var archivo = new Archivo
            {
                Id = file.FileName,
                TamañoEnBytes = file.Length,
                FechaSubida = DateTime.Today,
                UsuarioId = userId
            };

            string rutaArchivo = "";

            try
            {
                // Intentar subir a AWS S3
                rutaArchivo = await _awsService.UploadFileAsync(file, userId);
            }
            catch (Exception awsEx)
            {
                
                // Si falla, intentar con Azure Blob Storage
                try
                {
                    rutaArchivo = await _azureService.UploadFileAsync(file, userId);
                }
                catch (Exception azureEx)
                {
                    return StatusCode(500, azureEx.Message+" "+awsEx.Message);
                }
            }

            // Guardar en base de datos si la subida fue exitosa
            if (string.IsNullOrEmpty(rutaArchivo))
            {
                return StatusCode(500, "No se pudo subir el archivo.");
            }

            archivo.Path = rutaArchivo;
            user.StorageUsed += archivo.TamañoEnBytes;
            _context.Archivos.Add(archivo);
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "File uploaded successfully", Url = rutaArchivo });
        }


    }
}