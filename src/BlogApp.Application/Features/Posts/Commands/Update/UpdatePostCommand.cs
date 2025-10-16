using BlogApp.Domain.Common.Results;
using MediatR;

namespace BlogApp.Application.Features.Posts.Commands.Update;

public sealed record UpdatePostCommand(int Id, string Title, string Body, string Summary, string Thumbnail, bool IsPublished, int CategoryId) : IRequest<IResult>;
