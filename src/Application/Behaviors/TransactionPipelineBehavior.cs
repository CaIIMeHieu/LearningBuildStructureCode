using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Application.Abstractions;
using Contract.Abstractions.Message;
using Domain.Abstractions.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistance.DBContext;

namespace Application.Behaviors;

public sealed class TransactionPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
where TRequest : notnull
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ApplicationDbContext _context;
    private readonly IPublisher _publisher;

    public TransactionPipelineBehavior(IUnitOfWork unitOfWork, ApplicationDbContext context, IPublisher publisher)
    {
        _unitOfWork = unitOfWork;
        _context = context;
        _publisher = publisher;
    }

    public async Task<TResponse> Handle(TRequest request,
        RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!IsCommand()) // In case TRequest is QueryRequest just ignore
            return await next();

        #region ============== SQL-SERVER-STRATEGY-1 ============== 

        //// Use of an EF Core resiliency strategy when using multiple DbContexts within an explicit BeginTransaction():
        //// https://learn.microsoft.com/ef/core/miscellaneous/connection-resiliency
        //var strategy = _context.Database.CreateExecutionStrategy();
        //return await strategy.ExecuteAsync(async () =>
        //{
        //    await using var transaction = await _context.Database.BeginTransactionAsync();
        //    {
        //        var response = await next();
        //        await _context.SaveChangesAsync();
        //        await transaction.CommitAsync();
        //        return response;
        //    }
        //});
        #endregion ============== SQL-SERVER-STRATEGY-1 ==============

        #region ============== SQL-SERVER-STRATEGY-2 ==============

        //IMPORTANT: passing "TransactionScopeAsyncFlowOption.Enabled" to the TransactionScope constructor. This is necessary to be able to use it with async/await.
        using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {            
            var response = await next();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            // dispatch event here            
            var aggregateRoots = _context.ChangeTracker.Entries()
                .Select(e => e.Entity)
                .OfType<AggregateRoot>()
                .ToList();
            var domainEvents = aggregateRoots
                .SelectMany(e => e.DomainEvents)
                .ToList();
            foreach (var domainEvent in domainEvents)
            {
                await _publisher.Publish(domainEvent, cancellationToken);
            }
            aggregateRoots.ForEach(e => e.ClearDomainEvent());
            transaction.Complete();            
            return response;
        }
        #endregion ============== SQL-SERVER-STRATEGY-2 ==============

    }

    private bool IsCommand()
        => typeof(ICommand).IsAssignableFrom(typeof(TRequest));
}
