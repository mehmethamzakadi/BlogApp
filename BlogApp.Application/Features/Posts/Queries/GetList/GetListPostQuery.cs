using BlogApp.Domain.Common.Requests;
using BlogApp.Domain.Common.Responses;
using MediatR;

namespace BlogApp.Application.Features.Posts.Queries.GetList;

public sealed record GetListPostQuery(PageRequest PageRequest) : IRequest<GetListResponse<GetListPostResponse>>;