using WebApplication1.Models;

namespace WebApplication1.Interfaces
{
    public interface IFileStorageDataService
    {
        IEnumerable<UserStat> GetUserDailyStats();
    }
}
