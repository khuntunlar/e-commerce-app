using System.Linq.Expressions;

namespace IdentityService.IntegrationTests.Support;

internal sealed class FakeAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
{
    public FakeAsyncEnumerable(IEnumerable<T> enumerable)
        : base(enumerable)
    {
    }

    public FakeAsyncEnumerable(Expression expression)
        : base(expression)
    {
    }

    IQueryProvider IQueryable.Provider => new FakeAsyncQueryProvider<T>(this);

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        => new FakeAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
}
