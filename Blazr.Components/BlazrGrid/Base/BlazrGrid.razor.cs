/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Components.BlazrGrid;

public partial class BlazrGrid<TGridItem> : BlazrBaseComponent, IComponent, IHandleEvent, IDisposable
    where TGridItem : class, new()
{
    [Parameter, EditorRequired] public IBlazrGridContext<TGridItem>? GridContext { get; set; } = default!;
    [Parameter] public RenderFragment? ChildContent { get; set; }
    private IEnumerable<TGridItem> _items => GridContext?.Items ?? Enumerable.Empty<TGridItem>();

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
            ArgumentNullException.ThrowIfNull(GridContext);

            this.GridContext.StateChanged += OnContextChanged;

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
            this.GridContext.StateChanged += OnContextChanged;
    }
}
