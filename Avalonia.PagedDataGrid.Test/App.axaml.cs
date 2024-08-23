using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.PagedDataGrid.Test.Models;
using Avalonia.PagedDataGrid.Test.Services;
using Avalonia.PagedDataGrid.Test.ViewModels;
using Avalonia.PagedDataGrid.Test.Views;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Avalonia.PagedDataGrid.Test;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            MovieDbContext dbService = new();
            dbService.Database.EnsureCreated();
            if (!dbService.Movies.Any())
            {
                dbService.Movies.AddRange(ArrayService.Movies.Select(m =>
                {
                    m.Id = 0;
                    return m;
                }));
                dbService.SaveChanges();
            }
            // Line below is needed to remove Avalonia data validation.
            // Without this line you will get duplicate validations from both Avalonia and CT
            BindingPlugins.DataValidators.RemoveAt(0);
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainViewModel()
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = new MainViewModel()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}