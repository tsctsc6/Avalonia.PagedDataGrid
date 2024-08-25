using Avalonia.Controls;
using Avalonia.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Avalonia.PagedDataGrid.Controls;

public partial class PagedQueryableDataGrid : UserControl
{
    private static MethodInfo QueryableSkipMethodInfo;
    private static MethodInfo QueryableTakeMethodInfo;
    private static MethodInfo QueryableToArrayAsyncMethodInfo;
    private static MethodInfo QueryableCountAsyncMethodInfo;

    static PagedQueryableDataGrid()
    {
        InitMethodInfo();
    }

    [MemberNotNull(nameof(QueryableSkipMethodInfo),
        nameof(QueryableTakeMethodInfo),
        nameof(QueryableToArrayAsyncMethodInfo),
        nameof(QueryableCountAsyncMethodInfo))]
    private static void InitMethodInfo()
    {
        var QueryableMethods = typeof(Queryable)
            .GetMethods(BindingFlags.Static | BindingFlags.Public);
        var QueryableAsyncMethods = typeof(EntityFrameworkQueryableExtensions)
            .GetMethods(BindingFlags.Static | BindingFlags.Public);
        var EnumerableMethods = typeof(Enumerable)
            .GetMethods(BindingFlags.Static | BindingFlags.Public);
        QueryableSkipMethodInfo = QueryableMethods.First(m => m.Name == "Skip");
        QueryableTakeMethodInfo = QueryableMethods.First(m => m.Name == "Take");
        QueryableToArrayAsyncMethodInfo = QueryableAsyncMethods.First(m => m.Name == "ToArrayAsync");
        QueryableCountAsyncMethodInfo = QueryableAsyncMethods.First(m => m.Name == "CountAsync");
    }

    public PagedQueryableDataGrid()
    {
        InitializeComponent();
    }

    public static readonly DirectProperty<PagedQueryableDataGrid, int> CurrentPageProperty =
        AvaloniaProperty.RegisterDirect<PagedQueryableDataGrid, int>(
            nameof(CurrentPage),
            o => o.CurrentPage,
            (o, v) => o.CurrentPage = v,
            defaultBindingMode: BindingMode.TwoWay);
    public int CurrentPage
    {
        get
        {
            return Convert.ToInt32(CurrentPageNumericUpDown.Value);
        }
        set
        {
            CurrentPageNumericUpDown.Value = value;
        }
    }

    public static readonly DirectProperty<PagedQueryableDataGrid, int> MaxPageProperty =
        AvaloniaProperty.RegisterDirect<PagedQueryableDataGrid, int>(
            nameof(MaxPage),
            o => o.MaxPage,
            (o, v) => o.MaxPage = v,
            defaultBindingMode: BindingMode.TwoWay);
    public int MaxPage
    {
        get
        {
            return int.Parse(MaxPageTextBlock.Text!);
        }
        set
        {
            MaxPageTextBlock.Text = value.ToString();
            CurrentPageNumericUpDown.Maximum = value;
        }
    }

    public static readonly DirectProperty<PagedQueryableDataGrid, int> ItemCountPerPageProperty =
       AvaloniaProperty.RegisterDirect<PagedQueryableDataGrid, int>(
           nameof(ItemCountPerPage),
           o => o.ItemCountPerPage,
           (o, v) => o.ItemCountPerPage = v,
           defaultBindingMode: BindingMode.TwoWay);
    public int ItemCountPerPage
    {
        get
        {
            return Convert.ToInt32(ItemCountPerPageNumericUpDown.Value);
        }
        set
        {
            ItemCountPerPageNumericUpDown.Value = value;
            CalculateMaxPage();
        }
    }

    public static readonly DirectProperty<PagedQueryableDataGrid, int> ItemCountProperty =
      AvaloniaProperty.RegisterDirect<PagedQueryableDataGrid, int>(
          nameof(ItemCount),
          o => o.ItemCount,
          (o, v) => o.ItemCount = v,
          defaultBindingMode: BindingMode.TwoWay);
    private int _itemCount = 0;
    public int ItemCount
    {
        get
        {
            return _itemCount;
        }
        set
        {
            _itemCount = value;
            CalculateMaxPage();
        }
    }

    private void CalculateMaxPage()
    {
        if (ItemCountPerPage == 0) ItemCountPerPage = 1;
        if (ItemCount % ItemCountPerPage != 0)
            MaxPage = ItemCount / ItemCountPerPage + 1;
        else
            MaxPage = ItemCount / ItemCountPerPage;
    }

    private async ValueTask CalculateItemCountAsync()
    {
        if (ElementType is null) return;
        IsEnabled = false;
        int retryCount = 3;
        int currentCount = 0;
        while (true)
        {
            try
            {
                ItemCount = await (QueryableCountAsyncMethodInfo.MakeGenericMethod(ElementType)
                        .Invoke(null, [ItemsSource, default]) as Task<int>)!;
                break;
            }
            catch (InvalidOperationException)
            {
                // System.InvalidOperationException:
                // "BeginExecuteReader requires an open and available Connection. The connection's current state is closed."
                await Task.Delay(1);
                currentCount++;
                if (currentCount >= retryCount) throw;
            }
        }
        CalculateMaxPage();
        IsEnabled = true;
    }

    private async ValueTask RefreshAsync()
    {
        if (ElementType is null) return;
        IsEnabled = false;
        var r0 = QueryableSkipMethodInfo.MakeGenericMethod(ElementType).Invoke(null, [ItemsSource,
                (CurrentPage - 1) * ItemCountPerPage]);
        var r1 = QueryableTakeMethodInfo.MakeGenericMethod(ElementType).Invoke(null, [r0,
                ItemCountPerPage]);
        var task = QueryableToArrayAsyncMethodInfo.MakeGenericMethod(ElementType).Invoke(null,
            [r1, default]);

        var TaskContinueWithMethodInfo = task.GetType().GetMethods()
            .Where(m => m.Name == "ContinueWith")
            .Where(m => m.GetParameters().Length == 1)
            .Where(m => m.DeclaringType.Name == "Task`1")
            .Where(m => m.ReturnType.Name == "Task`1")
            .First().MakeGenericMethod(typeof(IEnumerable));
        var task2 = (TaskContinueWithMethodInfo
            .Invoke(task, new Func<Task, IEnumerable?>[] { ContinueWith_Lambda }) as Task<IEnumerable>)!;
        currentItems = await task2;

        MainDataGrid.ItemsSource = currentItems;

        IsEnabled = true;
    }

    private IEnumerable? ContinueWith_Lambda(Task task)
    {
        var Result = task.GetType().GetProperty("Result")!.GetValue(task);
        return Result as IEnumerable;
    }

    private async ValueTask GetItemCountAndRefreshAsync()
    {
        await CalculateItemCountAsync();
        await RefreshAsync();
    }

    private async void JumpButton_Click(object? sender, Interactivity.RoutedEventArgs e)
    {
        CalculateMaxPage();
        await RefreshAsync();
    }

    private IEnumerable? currentItems;
    private Type? ElementType;

    public static readonly DirectProperty<PagedQueryableDataGrid, bool> AutoGenerateColumnsProperty =
        AvaloniaProperty.RegisterDirect<PagedQueryableDataGrid, bool>(
          nameof(AutoGenerateColumns),
          o => o.AutoGenerateColumns,
          (o, v) => o.AutoGenerateColumns = v,
          defaultBindingMode: BindingMode.TwoWay);
    public bool AutoGenerateColumns
    {
        get { return MainDataGrid.AutoGenerateColumns; }
        set { MainDataGrid.AutoGenerateColumns = value; }
    }

    public static readonly DirectProperty<PagedQueryableDataGrid, IQueryable?> ItemsSourceProperty =
      AvaloniaProperty.RegisterDirect<PagedQueryableDataGrid, IQueryable?>(
          nameof(ItemsSource),
          o => o.ItemsSource,
          (o, v) => o.ItemsSource = v,
          defaultBindingMode: BindingMode.TwoWay);
    private IQueryable? _itemsSource = null;
    public IQueryable? ItemsSource
    {
        get
        {
            return _itemsSource;
        }
        set
        {
            _itemsSource = value;
            ElementType = ItemsSource?.ElementType;
            GetItemCountAndRefreshAsync();
        }
    }

    public static readonly DirectProperty<PagedQueryableDataGrid, bool> IsReadOnlyProperty =
        AvaloniaProperty.RegisterDirect<PagedQueryableDataGrid, bool>(
          nameof(IsReadOnly),
          o => o.IsReadOnly,
          (o, v) => o.IsReadOnly = v,
          defaultBindingMode: BindingMode.TwoWay);
    public bool IsReadOnly
    {
        get { return MainDataGrid.IsReadOnly; }
        set { MainDataGrid.IsReadOnly = value; }
    }

    public static readonly DirectProperty<PagedQueryableDataGrid, bool> CanUserReorderColumnsProperty =
        AvaloniaProperty.RegisterDirect<PagedQueryableDataGrid, bool>(
          nameof(CanUserReorderColumns),
          o => o.CanUserReorderColumns,
          (o, v) => o.CanUserReorderColumns = v,
          defaultBindingMode: BindingMode.TwoWay);
    public bool CanUserReorderColumns
    {
        get { return MainDataGrid.CanUserReorderColumns; }
        set { MainDataGrid.CanUserReorderColumns = value; }
    }

    public static readonly DirectProperty<PagedQueryableDataGrid, bool> CanUserResizeColumnsProperty =
        AvaloniaProperty.RegisterDirect<PagedQueryableDataGrid, bool>(
          nameof(CanUserResizeColumns),
          o => o.CanUserResizeColumns,
          (o, v) => o.CanUserResizeColumns = v,
          defaultBindingMode: BindingMode.TwoWay);
    public bool CanUserResizeColumns
    {
        get { return MainDataGrid.CanUserResizeColumns; }
        set { MainDataGrid.CanUserResizeColumns = value; }
    }

    public static readonly DirectProperty<PagedQueryableDataGrid, bool> CanUserSortColumnsProperty =
        AvaloniaProperty.RegisterDirect<PagedQueryableDataGrid, bool>(
          nameof(CanUserSortColumns),
          o => o.CanUserSortColumns,
          (o, v) => o.CanUserSortColumns = v,
          defaultBindingMode: BindingMode.TwoWay);
    public bool CanUserSortColumns
    {
        get { return MainDataGrid.CanUserSortColumns; }
        set { MainDataGrid.CanUserSortColumns = value; }
    }

    public ObservableCollection<DataGridColumn> DataGridColumns
    {
        get
        {
            return MainDataGrid.Columns;
        }
    }

    public static readonly DirectProperty<PagedQueryableDataGrid, object> SelectedItemProperty =
    AvaloniaProperty.RegisterDirect<PagedQueryableDataGrid, object>(
      nameof(SelectedItem),
      o => o.SelectedItem,
      (o, v) => o.SelectedItem = v,
      defaultBindingMode: BindingMode.TwoWay);
    public object SelectedItem
    {
        get { return MainDataGrid.SelectedItem; }
        set { MainDataGrid.SelectedItem = value; }
    }

    public IList SelectedItems
    {
        get { return MainDataGrid.SelectedItems; }
    }

    public static readonly DirectProperty<PagedQueryableDataGrid, int> SelectedIndexProperty =
        AvaloniaProperty.RegisterDirect<PagedQueryableDataGrid, int>(
          nameof(SelectedIndex),
          o => o.SelectedIndex,
          (o, v) => o.SelectedIndex = v,
          defaultBindingMode: BindingMode.TwoWay);
    public int SelectedIndex
    {
        get { return MainDataGrid.SelectedIndex; }
        set { MainDataGrid.SelectedIndex = value; }
    }

    public static readonly DirectProperty<PagedQueryableDataGrid, DataGridSelectionMode> SelectionModeProperty =
        AvaloniaProperty.RegisterDirect<PagedQueryableDataGrid, DataGridSelectionMode>(
          nameof(SelectionMode),
          o => o.SelectionMode,
          (o, v) => o.SelectionMode = v,
          defaultBindingMode: BindingMode.TwoWay);
    public DataGridSelectionMode SelectionMode
    {
        get { return MainDataGrid.SelectionMode; }
        set { MainDataGrid.SelectionMode = value; }
    }
}