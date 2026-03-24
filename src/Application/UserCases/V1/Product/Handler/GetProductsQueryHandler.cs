using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using Application.Abstractions;
using AutoMapper;
using Contract.Abstractions.Message;
using Contract.Abstractions.Shared;
using MediatR;

namespace Application.UserCases.V1.Product.Handler;

public class GetProductsQueryHandler : IQueryHandler<Query.GetProductsQuery, List<Response.ProductResponse>>
{
    private readonly IRepositoryBase<Domain.Entities.Product, Guid> _productRepository;
    private readonly IMapper _mapper;

    public GetProductsQueryHandler(IRepositoryBase<Domain.Entities.Product, Guid> productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<Response.ProductResponse>>> Handle(Query.GetProductsQuery request, CancellationToken cancellationToken)
    {
        var products = _productRepository.FindAll().ToList();
        var result = _mapper.Map<List<Response.ProductResponse>>(products);
        return result;
    }
}
