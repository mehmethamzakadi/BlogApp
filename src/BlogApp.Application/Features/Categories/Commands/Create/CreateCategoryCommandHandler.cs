
using System;
using BlogApp.Application.Abstractions;
using BlogApp.Application.Features.Categories.Queries.GetById;
using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Repositories;
using MediatR;

namespace BlogApp.Application.Features.Categories.Commands.Create;

public sealed class CreateCategoryCommandHandler(
    ICategoryRepository categoryRepository, 
    ICacheService cache,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateCategoryCommand, IResult>
{
    public async Task<IResult> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        bool categoryExists = await categoryRepository.AnyAsync(x => x.Name.ToLower() == request.Name.ToLower(), cancellationToken: cancellationToken);
        if (categoryExists)
        {
            return new ErrorResult("Kategori bilgisi zaten mevcut!");
        }

        var category = await categoryRepository.AddAsync(new Category { Name = request.Name });
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await cache.Add(
            $"category-{category.Id}",
            new GetByIdCategoryResponse(Id: category.Id, Name: category.Name),
            DateTimeOffset.UtcNow.AddMonths(1),
            null);

        return new SuccessResult("Kategori bilgisi başarıyla eklendi.");
    }
}
