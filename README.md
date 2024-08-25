# Avalonia.PagedDataGrid
This is a control based on the `Avalonia.DataGrid` that automatically paginates data.

There are two types of PagedDataGrid: `Avalonia.PagedEnumerableDataGrid` and `Avalonia.PagedQueryableDataGrid` . The difference between the two is, `Avalonia.PagedEnumerableDataGrid` for RAM data handling, and `Avalonia.PagedQueryableDataGrid` for database data handling.

`ToArrayAsync` , which provided by [EFCore](https://learn.microsoft.com/en-us/ef/core/), will be called in `Avalonia.PagedQueryableDataGrid` . So the UI thread is not blocked when querying the database.

## Usage
It is similar to the `Avalonia.DataGrid` . For more details, please see [Avalonia.PagedDataGrid.Test](https://github.com/tsctsc6/Avalonia.PagedDataGrid/tree/main/Avalonia.PagedDataGrid.Test).
