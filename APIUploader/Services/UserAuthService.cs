using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using WebApplication1.Data;
using WebApplication1.Interfaces;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class UserAuthService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserAuthService(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool RegisterUser(UserRegister request)
        {

            if (this.userExists(request.Username))
            {
                throw new InvalidOperationException("Este usuario ya existe"); // Usuario ya existe
            }

            // Crear y almacenar el nuevo usuario
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Username = request.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = request.Role
            };
            _context.Users.Add(user);
            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return true;
        }
        public bool userExists(string username)
        {
            return (_context.Users.FirstOrDefault(u=>u.Username == username)!=null);
        }
        public string LoginUser(UserLogin request)
        {
            // Verificar que el usuario existe y que la contraseña es correcta

            if (!this.userExists(request.Username))
            {
                throw new InvalidOperationException("este usuario no existe"); // Usuario no existe
            }

            var user = _context.Users.FirstOrDefault(u => u.Username == request.Username);

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {
                throw new InvalidOperationException("La Contraseña es incorrecta"); // contraseña incorrecta
            }

            // Generar y retornar el token JWT
           return GenerateJwtToken(user);
        }



        private string GenerateJwtToken(User user)
        {

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("TuClaveSecretaMuySeguraMuchoMasQueSegura!!!!!");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


    }
}
