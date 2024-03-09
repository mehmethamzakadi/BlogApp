﻿using AutoMapper;
using BlogApp.Domain.Common.Paging;
using BlogApp.Domain.Common.Requests;
using BlogApp.Domain.Common.Responses;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Application.Features.Posts.Queries.GetList
{
    public class GetListPostQuery : IRequest<GetListResponse<GetListPostResponse>>
    {
        public PageRequest PageRequest { get; set; }
        public class GetListPostQueryHandler : IRequestHandler<GetListPostQuery, GetListResponse<GetListPostResponse>>
        {
            private readonly IPostRepository _postRepository;
            private readonly IMapper _mapper;

            public GetListPostQueryHandler(IPostRepository postRepository, IMapper mapper)
            {
                _postRepository = postRepository;
                _mapper = mapper;

            }

            public async Task<GetListResponse<GetListPostResponse>> Handle(GetListPostQuery request, CancellationToken cancellationToken)
            {
                Paginate<Post> posts = await _postRepository.GetListAsync(
                    index: request.PageRequest.PageIndex,
                    include: p => p.Include(p => p.Category),
                    size: request.PageRequest.PageSize,
                    cancellationToken: cancellationToken
               );

                GetListResponse<GetListPostResponse> response = _mapper.Map<GetListResponse<GetListPostResponse>>(posts);
                return response;
            }
        }
    }
}
