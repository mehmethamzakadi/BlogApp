using BlogApp.Domain.Common.Results;
using MediatR;

namespace BlogApp.Application.Features.Categories.Commands.Delete;

public sealed record DeleteCategoryCommand(int Id) : IRequest<IResult>;
