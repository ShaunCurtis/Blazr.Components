﻿/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Components;

public abstract class BlazrPagingControlBase<TRecord>
    : BlazrControlBase, IDisposable
    where TRecord : class, new()
{
    [Parameter] public int BlockSize { get; set; } = 10;

    [Parameter] public int DefaultPageSize { get; set; } = 20;

    [Parameter] public bool ShowPageOf { get; set; } = true;

    [CascadingParameter] public IBlazrListContext<TRecord> GridContext { get; set; } = default!;

    protected override async Task OnParametersSetAsync()
    {
        if (this.NotInitialized)
        {
            if (this.GridContext is null)
                throw new NullReferenceException($"No {nameof(this.GridContext)} found.");

            await this.SetPageAsync();

            this.GridContext.StateChanged += this.OnStateChanged;
        }
    }

    protected int Page
        => this.GridContext.ListState.StartIndex / this.GridContext.ListState.PageSize;

    protected int ListCount
        => (int)this.GridContext.TotalCount;

    private int PageSize
        => this.GridContext.ListState.PageSize;

    protected bool hasPages
        => LastPage > 0;

    protected int DisplayPage
        => this.Page + 1;

    protected int LastPage
        => PageSize == 0 || ListCount == 0
            ? 0
            : ((int)Math.Ceiling(Decimal.Divide(this.ListCount, this.PageSize))) - 1;

    protected int LastDisplayPage
        => this.LastPage + 1;

    protected int ReadStartRecord
        => this.Page * this.PageSize;

    protected int Block
        => (int)Math.Floor(Decimal.Divide(this.Page, this.BlockSize));

    protected bool AreBlocks
        => this.ListCount > this.BlockSize * this.PageSize;

    protected int BlockStartPage
        => this.Block * this.BlockSize;

    protected int BlockEndPage
        => this.LastPage > (this.BlockStartPage + (BlockSize)) - 1
            ? (this.BlockStartPage + BlockSize) - 1
            : this.LastPage;

    protected int LastBlock
        => (int)Math.Floor(Decimal.Divide(this.LastPage, this.BlockSize));

    protected int LastBlockStartPage
        => LastBlock * this.BlockSize;

    protected async Task SetPageAsync(PagingRequest? request = null)
        => await this.GridContext.PageAsync(request ?? new PagingRequest() { PageSize = this.DefaultPageSize });

    protected void OnStateChanged(object? sender, EventArgs e)
        => this.StateHasChanged();

    protected async Task GotToPageAsync(int page)
    {
        if (page != this.Page)
        {
            await SetPageAsync(this.GetPagingRequest(page));
        }
    }

    protected string GetCss(int page)
        => page == this.Page ? "btn-primary" : "btn-secondary";

    protected async Task MoveBlockAsync(int block)
    {
        var _page = block switch
        {
            int.MaxValue => this.LastBlockStartPage,
            1 => this.Block + 1 > LastBlock ? LastBlock * this.BlockSize : this.BlockStartPage + BlockSize,
            -1 => this.Block - 1 < 0 ? 0 : this.BlockStartPage - BlockSize,
            _ => 0
        };

        await this.GotToPageAsync(_page);
    }

    protected async Task GoToBlockAsync(int block)
        => await this.GotToPageAsync(block * this.PageSize);

    protected PagingRequest GetPagingRequest(int page)
        => new PagingRequest { PageSize = this.PageSize, StartIndex = this.PageSize * page };

    public void Dispose()
    {
        this.GridContext.StateChanged -= this.OnStateChanged;
    }
}
