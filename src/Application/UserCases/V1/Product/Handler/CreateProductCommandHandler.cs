using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Abstractions;
using Application.Interface;
using Contract.Abstractions.Message;
using Contract.Abstractions.Shared;
using Domain.Abstractions;
using static Application.UserCases.V1.Product.Command;

namespace Application.UserCases.V1.Product.Handler;

public class CreateProductCommandHandler : ICommandHandler<CreateProductCommand>
{
    private readonly IRepositoryBase<Domain.Entities.Product, Guid> _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;

    public CreateProductCommandHandler(IRepositoryBase<Domain.Entities.Product, Guid> productRepository, IUnitOfWork unitOfWork, ICacheService cacheService)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<Result> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = Domain.Entities.Product.Create(request.Name, request.Price, request.Description);
        _productRepository.Add(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _cacheService.RemoveCacheResponseAsync("/api/v1/products");
        return Result.Success();
    }
}
