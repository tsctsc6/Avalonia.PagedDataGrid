using Avalonia.PagedDataGrid.Test.Models;
using Avalonia.PagedDataGrid.Test.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Avalonia.PagedDataGrid.Test.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private IEnumerable<Movie>? movies_e;

    [ObservableProperty]
    private IQueryable<Movie>? movies_q;

    public MainViewModel()
    {
        Movies_e = ArrayService.Movies;
        MovieDbContext dbService = new();
        Movies_q = dbService.Movies;
    }
}
