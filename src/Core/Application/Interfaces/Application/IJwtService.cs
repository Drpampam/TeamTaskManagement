using Domain.Models;

namespace Application.Interfaces.Application
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}
