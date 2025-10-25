using BlogApp.Domain.Common.Results;
using MediatR;

namespace BlogApp.Application.Features.Posts.Queries.GetById;

public sealed record GetByIdPostQuery(int Id, bool IncludeUnpublished) : IRequest<IDataResult<GetByIdPostResponse>>;
