using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Interfaces;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FileStorageDataController : ControllerBase
    {
        private readonly IFileStorageDataService _fileStorageDataService;
        public FileStorageDataController( IFileStorageDataService fileStorageDataService)
        {
            _fileStorageDataService = fileStorageDataService;
        }
        [HttpGet("daily-stats")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetDailyUserStats()
        {
            var dailyStats = _fileStorageDataService.GetUserDailyStats();

            return Ok(dailyStats);

        }
    }
}
