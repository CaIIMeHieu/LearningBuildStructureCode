using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Abstractions;
using AutoMapper;
using Contract.Abstractions.Message;
using Contract.Abstractions.Shared;
using static Application.UserCases.V1.Product.Response;

namespace Application.UserCases.V1.Product.Handler;

public class GetProductByIdQueryHandler : IQueryHandler<Query.GetProductByIdQuery, ProductResponse>
{
    private readonly IRepositoryBase<Domain.Entities.Product, Guid> _productRepository;
    private readonly IMapper _mapper;

    public GetProductByIdQueryHandler(IRepositoryBase<Domain.Entities.Product, Guid> productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }
    public async Task<Result<ProductResponse>> Handle(Query.GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.FindByIdAsync(request.Id, cancellationToken);
        var result = _mapper.Map<ProductResponse>(product);
        return result;
    }
}
