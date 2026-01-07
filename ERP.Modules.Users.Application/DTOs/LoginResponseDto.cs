namespace ERP.Modules.Users.Application.DTOs;

public class LoginResponseDto
{
    public string AccessToken { get; set; }
    public string TokenType { get; set; } = "Bearer";
    public int ExpiresIn { get; set; }
    public UserDto User { get; set; }
}
