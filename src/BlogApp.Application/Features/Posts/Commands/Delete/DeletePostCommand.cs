using BlogApp.Domain.Common.Results;
using MediatR;

namespace BlogApp.Application.Features.Posts.Commands.Delete;

public sealed record DeletePostCommand(int Id) : IRequest<IResult>;
