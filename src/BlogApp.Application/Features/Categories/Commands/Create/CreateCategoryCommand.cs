using BlogApp.Domain.Common.Results;
using MediatR;

namespace BlogApp.Application.Features.Categories.Commands.Create;

public sealed record CreateCategoryCommand(string Name) : IRequest<IResult>;
