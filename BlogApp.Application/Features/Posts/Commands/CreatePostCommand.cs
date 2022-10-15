﻿using BlogApp.Application.Features.Posts.ValidationRules;
using BlogApp.Application.Interfaces.Persistence;
using BlogApp.Application.Utilities.Results;
using BlogApp.Domain.Entities;
using MediatR;

namespace BlogApp.Application.Features.Posts.Commands
{
    public class CreatePostCommand : IRequest<IResult>
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public string Summary { get; set; }
        public string Thumbnail { get; set; }
        public bool IsPublished { get; set; }
        public virtual List<int> CategoriIds { get; set; }

        public class CreatePostCommandHandler : IRequestHandler<CreatePostCommand, IResult>
        {
            private readonly IUnitOfWork _unitOfWork;

            public CreatePostCommandHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<IResult> Handle(CreatePostCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var post = await _unitOfWork.PostRepository.AddAsync(new Post { Title = request.Title, Body = request.Body, Summary = request.Summary, Thumbnail = request.Thumbnail, IsPublished = false });

                    foreach (var id in request.CategoriIds)
                    {
                        await _unitOfWork.PostCategoryRepository.AddAsync(new PostCategory { CategoryId = id, PostId = post.Id });
                    }

                    await _unitOfWork.SaveAsync();
                    return new SuccessResult("Post bilgsi başarıyla eklendi.");
                }
                catch (Exception)
                {
                    return new ErrorResult("Post bilgsi eklerken hata oluştu!");
                }
            }
        }
    }
}
