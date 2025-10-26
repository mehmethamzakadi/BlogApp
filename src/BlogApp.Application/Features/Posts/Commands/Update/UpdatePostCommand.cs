using BlogApp.Domain.Common.Results;
using MediatR;

namespace BlogApp.Application.Features.Posts.Commands.Update;

public sealed record UpdatePostCommand(Guid Id, string Title, string Body, string Summary, string Thumbnail, bool IsPublished, Guid CategoryId) : IRequest<IResult>;
