using Avalonia.PagedDataGrid.Test.Models;
using System;

namespace Avalonia.PagedDataGrid.Test.Services;

public class ArrayService
{
    public static Movie[] Movies = [
        new ()
        {
            Id = 1,
            Title = "lalala",
            ReleaseDate = DateTime.Now,
            Genre = "a",
            Price = 5,
        },
        new ()
        {
            Id = 2,
            Title = "bang",
            ReleaseDate = DateTime.Now,
            Genre = "b",
            Price = 2,
        },
        ];
}
