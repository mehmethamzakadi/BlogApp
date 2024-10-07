using MediatR;

namespace BlogApp.Application.Features.Auths.UpdatePassword;

public sealed record UpdatePasswordCommand(string UserId, string ResetToken, string Password, string PasswordConfirm) : IRequest<UpdatePasswordResponse>;