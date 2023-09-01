/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.OneWayStreet.Core;

namespace Blazr.Components.BlazrGrid;

public sealed partial class BlazrSortControl<TGridItem> : BlazrControlBase, IDisposable
     where TGridItem : class, new()
{
    private bool showSortingDropdown = false;

    [CascadingParameter] public IBlazrGridContext<TGridItem> GridContext { get; set; } = default!;

    [Parameter] public bool IsMaxColumn { get; set; }

    [Parameter] public string Title { get; set; } = string.Empty;

    [Parameter] public bool IsNoWrap { get; set; } = true;

    [Parameter] public string SortField { get; set; } = string.Empty;

    private string showCss => showSortingDropdown ? "show" : String.Empty;

    private SortDefinition _currentSorter => this.GridContext.ListState.Sorters.FirstOrDefault();

    private bool _isCurrentSortField => !string.IsNullOrWhiteSpace(this.SortField)
        && _currentSorter.SortField.Equals(SortField);

    private bool _canSort => !string.IsNullOrWhiteSpace(SortField);

    protected override Task OnParametersSetAsync()
    {
        if (this.GridContext is null)
            throw new NullReferenceException("There's no cascaded ListController.");

        if (NotInitialized)
            this.GridContext.StateChanged += this.OnStateChanged;

        return Task.CompletedTask;
    }

    private void ShowSorting(bool show)
    {
        showSortingDropdown = show;
        this.StateHasChanged();
    }

    private async Task SortClick(bool descending)
    {
        if (!_canSort)
            return;

        SortRequest request = new() { SortDescending = descending, SortField = this.SortField };

        await this.GridContext.SortAsync(request);
    }

    private string GetActive(bool dir)
    {
        if (!_isCurrentSortField)
            return string.Empty;

        bool sortDescending = _currentSorter.SortDescending;

        return dir == sortDescending ? "active" : string.Empty;

    }

    private string HeaderCss
     => CSSBuilder.Class(BlazrGridCss.HeaderCss)
         .AddClass("align-baseline")
         .Build();

    private string SortIconCss
    => !_isCurrentSortField
        ? BlazrGridCss.NotSortedClass
        : _currentSorter.SortDescending
            ? BlazrGridCss.AscendingClass
            : BlazrGridCss.DescendingClass;

    private void OnStateChanged(object? sender, EventArgs e)
        => this.StateHasChanged();

    public void Dispose()
        => this.GridContext.StateChanged -= this.OnStateChanged;
}
