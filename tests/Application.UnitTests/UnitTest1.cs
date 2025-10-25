using BlogApp.Application.Abstractions;
using BlogApp.Application.Features.Categories.Commands.Create;
using BlogApp.Application.Features.Posts.Commands.Create;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Moq;
using IResult = BlogApp.Domain.Common.Results.IResult;

namespace Application.UnitTests;

public class CreateCategoryCommandHandlerTests
{
    [Test]
    public async Task Handle_ShouldReturnError_WhenCategoryAlreadyExists()
    {
        var repositoryMock = new Mock<ICategoryRepository>();
        repositoryMock
            .Setup(repo => repo.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Category, bool>>>(), false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var cacheMock = new Mock<ICacheService>();
        var unitOfWorkMock = new Mock<BlogApp.Domain.Common.IUnitOfWork>();
        var httpContextAccessorMock = new Mock<IHttpContextAccessor>();

        var handler = new CreateCategoryCommandHandler(repositoryMock.Object, cacheMock.Object, unitOfWorkMock.Object, httpContextAccessorMock.Object);
        IResult result = await handler.Handle(new CreateCategoryCommand("Test"), CancellationToken.None);

        Assert.That(result.Success, Is.False);
        cacheMock.Verify(cache => cache.Add(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<DateTimeOffset?>(), It.IsAny<TimeSpan?>()), Times.Never);
    }

    [Test]
    public async Task Handle_ShouldCacheCategory_WhenCreatedSuccessfully()
    {
        var repositoryMock = new Mock<ICategoryRepository>();
        repositoryMock
            .Setup(repo => repo.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Category, bool>>>(), false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        repositoryMock
            .Setup(repo => repo.AddAsync(It.IsAny<Category>()))
            .ReturnsAsync(new Category { Id = 10, Name = "Test" });

        var cacheMock = new Mock<ICacheService>();
        cacheMock
            .Setup(cache => cache.Add(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<DateTimeOffset?>(), It.IsAny<TimeSpan?>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        var unitOfWorkMock = new Mock<BlogApp.Domain.Common.IUnitOfWork>();
        unitOfWorkMock
            .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var httpContextAccessorMock = new Mock<IHttpContextAccessor>();

        var handler = new CreateCategoryCommandHandler(repositoryMock.Object, cacheMock.Object, unitOfWorkMock.Object, httpContextAccessorMock.Object);
        IResult result = await handler.Handle(new CreateCategoryCommand("Test"), CancellationToken.None);

        Assert.That(result.Success, Is.True);
        cacheMock.Verify(cache => cache.Add("category-10", It.IsAny<object>(), It.IsAny<DateTimeOffset?>(), It.IsAny<TimeSpan?>()), Times.Once);
    }
}

public class CreatePostCommandHandlerTests
{
    [Test]
    public async Task Handle_ShouldPersistPost_WithProvidedCategoryId()
    {
        var repositoryMock = new Mock<IPostRepository>();
        repositoryMock
            .Setup(repo => repo.AddAsync(It.IsAny<Post>()))
            .ReturnsAsync((Post post) => post);

        var unitOfWorkMock = new Mock<BlogApp.Domain.Common.IUnitOfWork>();
        unitOfWorkMock
            .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var httpContextAccessorMock = new Mock<IHttpContextAccessor>();

        var handler = new CreatePostCommandHandler(repositoryMock.Object, unitOfWorkMock.Object, httpContextAccessorMock.Object);
        var command = new CreatePostCommand("Title", "Body", "Summary", "thumb", true, 5);

        IResult result = await handler.Handle(command, CancellationToken.None);

        Assert.That(result.Success, Is.True);
        repositoryMock.Verify(repo => repo.AddAsync(It.Is<Post>(post => post.CategoryId == 5 && post.Title == "Title")), Times.Once);
        unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
