namespace BlogApp.Domain.DTOs;

public sealed record CreateAppUserDto(string UserName, string Email, string Password);
