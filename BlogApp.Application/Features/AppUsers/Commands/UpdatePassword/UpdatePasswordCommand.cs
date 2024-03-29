using MediatR;

namespace BlogApp.Application.Features.AppUsers.Commands.UpdatePassword;

public sealed record UpdatePasswordCommand(string UserId, string ResetToken, string Password, string PasswordConfirm) : IRequest<UpdatePasswordResponse>;