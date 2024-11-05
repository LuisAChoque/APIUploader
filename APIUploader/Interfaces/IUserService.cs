using WebApplication1.Models;

namespace WebApplication1.Interfaces
{
    public interface IUserService
    {
        bool RegisterUser(UserRegister request);
        string LoginUser(UserLogin request);
    }
}
