using BlogApp.Application.Features.Posts.Commands;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp.Application.Features.Posts.ValidationRules
{
    public class UpdatePostValidator : AbstractValidator<UpdatePostCommand>
    {
        public UpdatePostValidator()
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
