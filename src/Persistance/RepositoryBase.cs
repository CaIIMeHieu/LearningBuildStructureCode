using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Application.Abstractions;
using Contract.Abstractions.Shared;
using Domain.Abstractions;
using Domain.Abstractions.Entities;
using Microsoft.EntityFrameworkCore;
using Persistance.DBContext;

namespace Persistance;

public class RepositoryBase<TEntity, TKey> : IRepositoryBase<TEntity, TKey>, IDisposable
        where TEntity : DomainEntity<TKey>
{

    private readonly ApplicationDbContext _context;

    public RepositoryBase(ApplicationDbContext context)
        => _context = context;

    public void Dispose()
        => _context?.Dispose();

    public IQueryable<TEntity> FindAll(Expression<Func<TEntity, bool>>? predicate = null,
        params Expression<Func<TEntity, object>>[] includeProperties)
    {
        IQueryable<TEntity> items = _context.Set<TEntity>().AsNoTracking(); // Importance Always include AsNoTracking for Query Side
        if (includeProperties != null)
            foreach (var includeProperty in includeProperties)
                items = items.Include(includeProperty);

        if (predicate is not null)
            items = items.Where(predicate);

        return items;
    }

    public async Task<TEntity> FindByIdAsync(TKey id, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includeProperties)
        => await FindAll(null, includeProperties).AsTracking().SingleOrDefaultAsync(x => x.Id.Equals(id), cancellationToken);

    public async Task<PageResultT<TEntity>> FindAllPagedAsync(
            PagedRequest request,
            Expression<Func<TEntity,bool>>? predicate = null,
            CancellationToken cancellationToken = default,
            params Expression<Func<TEntity, object>>[] includeProperties
        )
    {
        IQueryable<TEntity> query = FindAll(predicate, includeProperties);

        int totalCount = await query.CountAsync(cancellationToken);

        query = ApplySorting(query, request.SortOptions);

        // 4. Xử lý Phân trang (Pagination)
        // Công thức chuẩn: OFFSET (Index - 1) * Size FETCH NEXT Size ROWS ONLY
        var items = await query
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        // 5. Đóng gói kết quả
        return new PageResultT<TEntity>
        {
            Items = items,
            TotalCount = totalCount,
            PageIndex = request.PageIndex,
            PageSize = request.PageSize
        };
    }

    // ==========================================
    // HELPER: Hàm chuyển mảng SortOption thành LINQ OrderBy
    // ==========================================
    private IQueryable<TEntity> ApplySorting(IQueryable<TEntity> query, List<SortOption> sortOptions)
    {
        if (sortOptions == null || !sortOptions.Any())
            return query;

        var entityType = typeof(TEntity);
        var parameter = Expression.Parameter(entityType, "x");
        bool isFirstSort = true;

        foreach (var sort in sortOptions)
        {
            // Tìm thuộc tính trong Entity có tên khớp với chuỗi (Bỏ qua hoa/thường)
            var propertyInfo = entityType.GetProperty(sort.Field,
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            if (propertyInfo == null)
                continue; // Bỏ qua nếu Frontend gửi sai tên cột

            var propertyAccess = Expression.MakeMemberAccess(parameter, propertyInfo);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);

            // Xác định gọi OrderBy hay ThenBy
            string methodName = isFirstSort
                ? (sort.IsDescending ? "OrderByDescending" : "OrderBy")
                : (sort.IsDescending ? "ThenByDescending" : "ThenBy");

            var resultExpression = Expression.Call(
                typeof(Queryable),
                methodName,
                new Type[] { entityType, propertyInfo.PropertyType },
                query.Expression,
                Expression.Quote(orderByExpression));

            query = query.Provider.CreateQuery<TEntity>(resultExpression);
            isFirstSort = false;
        }

        return query;
    }
    public async Task<TEntity> FindSingleAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includeProperties)
        => await FindAll(null, includeProperties).AsTracking().SingleOrDefaultAsync(predicate, cancellationToken);

    public void Add(TEntity entity)
        => _context.Add(entity);

    public void Remove(TEntity entity)
        => _context.Set<TEntity>().Remove(entity);

    public void RemoveMultiple(List<TEntity> entities)
        => _context.Set<TEntity>().RemoveRange(entities);

    public void Update(TEntity entity)
        => _context.Set<TEntity>().Update(entity);
}
