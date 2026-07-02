using System.Collections;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;

namespace IdentityService.IntegrationTests.Support;

internal sealed class FakeDbSet<T> : DbSet<T>, IQueryable<T>, IAsyncEnumerable<T>
    where T : class
{
    private readonly List<T> _data;
    private readonly IQueryable<T> _query;

    public FakeDbSet(List<T> data)
    {
        _data = data;
        _query = new FakeAsyncEnumerable<T>(_data);
    }

    public override IEntityType EntityType => null!;

    public override EntityEntry<T> Add(T entity)
    {
        _data.Add(entity);
        return null!;
    }

    public override void AddRange(IEnumerable<T> entities)
    {
        _data.AddRange(entities);
    }

    public override ValueTask<EntityEntry<T>> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        _data.Add(entity);
        return ValueTask.FromResult<EntityEntry<T>>(null!);
    }

    public override Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        _data.AddRange(entities);
        return Task.CompletedTask;
    }

    public override IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        => new FakeAsyncEnumerator<T>(_data.GetEnumerator());

    Type IQueryable.ElementType => _query.ElementType;

    Expression IQueryable.Expression => _query.Expression;

    IQueryProvider IQueryable.Provider => _query.Provider;

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => _data.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _data.GetEnumerator();
}
