using BlogApp.Domain.Common.Results;
using MediatR;

namespace BlogApp.Application.Features.Categories.Commands.Update;

public sealed record UpdateCategoryCommand(Guid Id, string Name) : IRequest<IResult>;
