using BlogApp.Domain.Common.Results;
using MediatR;

namespace BlogApp.Application.Features.Posts.Commands.Create;

public sealed record CreatePostCommand(string Title, string Body, string Summary, string Thumbnail, bool IsPublished, Guid CategoryId) : IRequest<IResult>;
