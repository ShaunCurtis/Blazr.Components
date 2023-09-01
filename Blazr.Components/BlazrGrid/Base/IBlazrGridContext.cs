/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Components.BlazrGrid;

public interface IBlazrGridContext<TGridItem>
{
    public Guid ContextUid { get; }
    public event EventHandler<EventArgs>? StateChanged;
    public IEnumerable<TGridItem> Items { get; }
    public long TotalCount { get; }
    public ICollectionState ListState { get; }

    public ValueTask GetItemsAsync();
    public ValueTask PageAsync(PagingRequest pagingRequest);
    public ValueTask SortAsync(SortRequest sortRequest);
    public ValueTask FilterAsync(FilterRequest<TGridItem> filterRequest);
}
