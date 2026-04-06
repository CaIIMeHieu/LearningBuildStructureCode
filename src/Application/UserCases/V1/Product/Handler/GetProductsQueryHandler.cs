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
using Domain.Abstractions;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace Application.UserCases.V1.Product.Handler;

public class GetProductsQueryHandler : IQueryHandler<Query.GetProductsQuery, List<Response.ProductResponse>>
{
    private readonly IRepositoryBase<Domain.Entities.Product, Guid> _productRepository;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _memCache;

    public GetProductsQueryHandler(IRepositoryBase<Domain.Entities.Product, Guid> productRepository, IMapper mapper, IMemoryCache memCache)
    {
        _productRepository = productRepository;
        _mapper = mapper;
        _memCache = memCache;
    }

    //mem cache
    public async Task<Result<List<Response.ProductResponse>>> Handle(Query.GetProductsQuery request, CancellationToken cancellationToken)
    {
        string key = "products_all";

        if( _memCache.TryGetValue(key, out List<Response.ProductResponse>? cached ))
        {
            return cached;
        }

        var products = _productRepository.FindAll().ToList();
        var result = _mapper.Map<List<Response.ProductResponse>>(products);
        _memCache.Set(key, result, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
            SlidingExpiration = TimeSpan.FromMinutes(2)
            // AbsoluteExpiration: Hết hạn sau 5 phút dù có được dùng hay không
            // SlidingExpiration: Reset timer nếu được access trong vòng 2 phút
        });
        return result;
    }


}
