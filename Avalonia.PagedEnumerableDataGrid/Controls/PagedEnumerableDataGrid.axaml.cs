using Avalonia.Controls;
using Avalonia.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Avalonia.PagedDataGrid.Controls;

public partial class PagedEnumerableDataGrid : UserControl
{
    private static MethodInfo EnumerableSkipMethodInfo;
    private static MethodInfo EnumerableTakeMethodInfo;
    private static MethodInfo EnumerableCountMethodInfo;

    static PagedEnumerableDataGrid()
    {
        InitMethodInfo();
    }

    [MemberNotNull(nameof(EnumerableSkipMethodInfo),
        nameof(EnumerableTakeMethodInfo),
        nameof(EnumerableCountMethodInfo))]
    private static void InitMethodInfo()
    {
        var EnumerableMethods = typeof(Enumerable)
            .GetMethods(BindingFlags.Static | BindingFlags.Public);
        EnumerableSkipMethodInfo = EnumerableMethods.First(m => m.Name == "Skip");
        EnumerableTakeMethodInfo = EnumerableMethods.First(m => m.Name == "Take");
        EnumerableCountMethodInfo = EnumerableMethods.First(m => m.Name == "Count");
    }

    public PagedEnumerableDataGrid()
    {
        InitializeComponent();
    }

    public static readonly DirectProperty<PagedEnumerableDataGrid, int> CurrentPageProperty =
        AvaloniaProperty.RegisterDirect<PagedEnumerableDataGrid, int>(
            nameof(CurrentPage),
            o => o.CurrentPage,
            (o, v) => o.CurrentPage = v,
            defaultBindingMode: BindingMode.TwoWay);
    public int CurrentPage
    {
        get
        {
            return (int)CurrentPageNumericUpDown.Value;
        }
        set
        {
            CurrentPageNumericUpDown.Value = value;
        }
    }

    public static readonly DirectProperty<PagedEnumerableDataGrid, int> MaxPageProperty =
        AvaloniaProperty.RegisterDirect<PagedEnumerableDataGrid, int>(
            nameof(MaxPage),
            o => o.MaxPage,
            (o, v) => o.MaxPage = v,
            defaultBindingMode: BindingMode.TwoWay);
    public int MaxPage
    {
        get
        {
            return int.Parse(MaxPageTextBlock.Text);
        }
        set
        {
            MaxPageTextBlock.Text = value.ToString();
            CurrentPageNumericUpDown.Maximum = value;
        }
    }

    public static readonly DirectProperty<PagedEnumerableDataGrid, int> ItemCountPerPageProperty =
       AvaloniaProperty.RegisterDirect<PagedEnumerableDataGrid, int>(
           nameof(ItemCountPerPage),
           o => o.ItemCountPerPage,
           (o, v) => o.ItemCountPerPage = v,
           defaultBindingMode: BindingMode.TwoWay);
    public int ItemCountPerPage
    {
        get
        {
            return (int)ItemCountPerPageNumericUpDown.Value;
        }
        set
        {
            ItemCountPerPageNumericUpDown.Value = value;
            CalculateMaxPage();
        }
    }

    public static readonly DirectProperty<PagedEnumerableDataGrid, int> ItemCountProperty =
      AvaloniaProperty.RegisterDirect<PagedEnumerableDataGrid, int>(
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
        if (ItemCount % ItemCountPerPage != 0)
            MaxPage = ItemCount / ItemCountPerPage + 1;
        else
            MaxPage = ItemCount / ItemCountPerPage;
    }

    private void CalculateItemCount()
    {
        if (T is null) return;
        IsEnabled = false;

        ItemCount = (int)EnumerableCountMethodInfo.MakeGenericMethod(T)
            .Invoke(null, [ItemsSource])!;

        CalculateMaxPage();
        IsEnabled = true;
    }

    private void Refresh()
    {
        if (T is null) return;
        IsEnabled = false;

        var r0 = EnumerableSkipMethodInfo.MakeGenericMethod(T).Invoke(null, [ItemsSource,
                (CurrentPage - 1) * ItemCountPerPage]);
        var r1 = EnumerableTakeMethodInfo.MakeGenericMethod(T).Invoke(null, [r0,
                ItemCountPerPage]);
        currentItems = r1 as IEnumerable;

        MainDataGrid.ItemsSource = currentItems;

        IsEnabled = true;
    }

    private void GetItemCountAndRefresh()
    {
        CalculateItemCount();
        Refresh();
    }

    private void JumpButton_Click(object? sender, Interactivity.RoutedEventArgs e)
    {
        CalculateMaxPage();
        Refresh();
    }

    private IEnumerable? currentItems;
    private Type? T;

    public static readonly DirectProperty<PagedEnumerableDataGrid, bool> AutoGenerateColumnsProperty =
        AvaloniaProperty.RegisterDirect<PagedEnumerableDataGrid, bool>(
          nameof(AutoGenerateColumns),
          o => o.AutoGenerateColumns,
          (o, v) => o.AutoGenerateColumns = v,
          defaultBindingMode: BindingMode.TwoWay);
    public bool AutoGenerateColumns
    {
        get { return MainDataGrid.AutoGenerateColumns; }
        set { MainDataGrid.AutoGenerateColumns = value; }
    }

    public static readonly DirectProperty<PagedEnumerableDataGrid, IEnumerable?> ItemsSourceProperty =
      AvaloniaProperty.RegisterDirect<PagedEnumerableDataGrid, IEnumerable?>(
          nameof(ItemsSource),
          o => o.ItemsSource,
          (o, v) => o.ItemsSource = v,
          defaultBindingMode: BindingMode.TwoWay);
    private IEnumerable? _itemsSource = null;
    public IEnumerable? ItemsSource
    {
        get
        {
            return _itemsSource;
        }
        set
        {
            _itemsSource = value;
            T = ItemsSource?.AsQueryable().ElementType;
            GetItemCountAndRefresh();
        }
    }

    public static readonly DirectProperty<PagedEnumerableDataGrid, bool> IsReadOnlyProperty =
        AvaloniaProperty.RegisterDirect<PagedEnumerableDataGrid, bool>(
          nameof(IsReadOnly),
          o => o.IsReadOnly,
          (o, v) => o.IsReadOnly = v,
          defaultBindingMode: BindingMode.TwoWay);
    public bool IsReadOnly
    {
        get { return MainDataGrid.IsReadOnly; }
        set { MainDataGrid.IsReadOnly = value; }
    }

    public static readonly DirectProperty<PagedEnumerableDataGrid, bool> CanUserReorderColumnsProperty =
        AvaloniaProperty.RegisterDirect<PagedEnumerableDataGrid, bool>(
          nameof(CanUserReorderColumns),
          o => o.CanUserReorderColumns,
          (o, v) => o.CanUserReorderColumns = v,
          defaultBindingMode: BindingMode.TwoWay);
    public bool CanUserReorderColumns
    {
        get { return MainDataGrid.CanUserReorderColumns; }
        set { MainDataGrid.CanUserReorderColumns = value; }
    }

    public static readonly DirectProperty<PagedEnumerableDataGrid, bool> CanUserResizeColumnsProperty =
        AvaloniaProperty.RegisterDirect<PagedEnumerableDataGrid, bool>(
          nameof(CanUserResizeColumns),
          o => o.CanUserResizeColumns,
          (o, v) => o.CanUserResizeColumns = v,
          defaultBindingMode: BindingMode.TwoWay);
    public bool CanUserResizeColumns
    {
        get { return MainDataGrid.CanUserResizeColumns; }
        set { MainDataGrid.CanUserResizeColumns = value; }
    }

    public static readonly DirectProperty<PagedEnumerableDataGrid, bool> CanUserSortColumnsProperty =
        AvaloniaProperty.RegisterDirect<PagedEnumerableDataGrid, bool>(
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
}