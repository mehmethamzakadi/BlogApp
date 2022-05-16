using AutoMapper;
using BlogApp.Application.DTOs.Category;
using BlogApp.Application.Features.Category.Queries;
using BlogApp.Application.Interfaces.Persistence;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp.Application.Features.Category.Handlers
{
    internal class GetCategoryByIdQueryHandler : IRequestHandler<GetByIdCategoryQuery, RsCategory>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetCategoryByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<RsCategory> Handle(GetByIdCategoryQuery request, CancellationToken cancellationToken)
        {
            var category = await _unitOfWork.CategoryRepository.GetByIdAsync(request.Id);
            return _mapper.Map<RsCategory>(category);
        }
    }
}
