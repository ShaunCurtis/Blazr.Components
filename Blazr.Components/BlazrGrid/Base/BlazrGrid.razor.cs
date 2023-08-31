/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Components.BlazrGrid;

// The Component is configured to get it's data set from one of two sources (in order of precidence):
//   1. The Parameter provided ListContext.
//   2. The cascaded cascadedListContext instance.

public partial class BlazrGrid<TGridItem> : BlazrBaseComponent, IComponent, IHandleEvent, IDisposable
    where TGridItem : class, new()
{
    [CascadingParameter] private IBlazrListContext<TGridItem>? cascadedListContext { get; set; }

    [Parameter] public IBlazrListContext<TGridItem>? ListContext { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }

    private IBlazrListContext<TGridItem>? _listContext { get; set; }
    private IEnumerable<TGridItem> _items => _listContext?.Items ?? Enumerable.Empty<TGridItem>();

    protected readonly List<IBlazrGridColumn<TGridItem>> GridColumns = new();

    public void RegisterColumn(IBlazrGridColumn<TGridItem> column)
    {
        if (!this.GridColumns.Any(item => item.ComponentUid == column.ComponentUid))
            this.GridColumns.Add(column);
    }

    public async Task SetParametersAsync(ParameterView parameters)
    {
        // Sets the component parameters to the supplied values
        parameters.SetParameterProperties(this);

        // Set the list Controller, prioritizing the Parameter Value
        if (this.NotInitialized)
        {
            _listContext = ListContext ?? cascadedListContext;

            ArgumentNullException.ThrowIfNull(_listContext);

            _listContext.ContextChanged += OnContextChanged;

            // Render the component so all the columns can register
            await this.RenderAsync();
        }

        // Render the grid content
        this.StateHasChanged();
    }

    //TODO - is this needed
    public async ValueTask RefreshAsync()
        => await this.RenderAsync();

    public async void OnContextChanged(object? sender, EventArgs e)
        => await this.RenderAsync();

    async Task IHandleEvent.HandleEventAsync(EventCallbackWorkItem item, object? obj)
    {
        await item.InvokeAsync(obj);
        this.StateHasChanged();
    }

    public void Dispose()
    {
        if (_listContext is not null)
            _listContext.ContextChanged += OnContextChanged;
    }
}
