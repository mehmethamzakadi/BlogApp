﻿using FluentValidation;

namespace BlogApp.Application.Features.Posts.Commands.Create
{
    public sealed class CreatePostValidator : AbstractValidator<CreatePostCommand>
    {
        public CreatePostValidator()
        {
            RuleFor(c => c.Title)
                .NotEmpty().WithMessage("Başlık bilgisi boş olmamalıdır!")
                .MaximumLength(100).WithMessage("Başlık bilgisi 100 karakterden uzun olmamalıdır!");

            RuleFor(c => c.Body)
                .NotEmpty().WithMessage("İçerik bilgisi boş olmamalıdır!");

            RuleFor(c => c.Thumbnail)
                .NotEmpty().WithMessage("Küçük resim bilgisi boş olmamalıdır!");

            RuleFor(c => c.Summary)
               .NotEmpty().WithMessage("Özet bilgisi boş olmamalıdır!")
               .MaximumLength(400).WithMessage("Özet bilgisi 400 karakterden fazla olmamalıdır!");
        }
    }
}
