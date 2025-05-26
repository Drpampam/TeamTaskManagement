using Domain.Models;

public class RegisterDto
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class UserDto
{
    public string Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }

    public void MapFromUser(User user)
    {
        Id = user.Id;
        Username = user.Username;
        Email = user.Email;
    }

    public void MapToUser(User user)
    {
        user.Id = Id;
        user.Username = Username;
        user.Email = Email;
    }
}
