using Avalonia.PagedDataGrid.Test.Models;
using System;

namespace Avalonia.PagedDataGrid.Test.Services;

public class MovieArrayService
{
    public static Movie[] Movies = [
        new()
            {
                Id = 1,
                Title = "Inception",
                ReleaseDate = new DateTime(2010, 7, 16),
                Genre = "SciFi",
                Price = 19.99m
            },
        new()
            {
                Id = 2,
                Title = "The Matrix",
                ReleaseDate = new DateTime(1999, 3, 31),
                Genre = "Action",
                Price = 14.99m
            },
        new()
            {
                Id = 3,
                Title = "Titanic",
                ReleaseDate = new DateTime(1997, 12, 19),
                Genre = "Drama",
                Price = 9.99m
            },
        new()
            {
                Id = 4,
                Title = "The Godfather",
                ReleaseDate = new DateTime(1972, 3, 24),
                Genre = "Drama",
                Price = 12.99m
            },
        new()
            {
                Id = 5,
                Title = "Pulp Fiction",
                ReleaseDate = new DateTime(1994, 10, 14),
                Genre = "Comedy",
                Price = 11.99m
            },
        new()
            {
                Id = 6,
                Title = "Jurassic Park",
                ReleaseDate = new DateTime(1993, 6, 11),
                Genre = "SciFi",
                Price = 15.99m
            },
        new()
            {
                Id = 7,
                Title = "Star Wars: A New Hope",
                ReleaseDate = new DateTime(1977, 5, 25),
                Genre = "SciFi",
                Price = 18.99m
            },
        new()
            {
                Id = 8,
                Title = "The Dark Knight",
                ReleaseDate = new DateTime(2008, 7, 18),
                Genre = "Action",
                Price = 16.99m
            },
        new()
            {
                Id = 9,
                Title = "Forrest Gump",
                ReleaseDate = new DateTime(1994, 7, 6),
                Genre = "Drama",
                Price = 13.99m
            },
        new()
            {
                Id = 10,
                Title = "The Shawshank Redemption",
                ReleaseDate = new DateTime(1994, 9, 23),
                Genre = "Drama",
                Price = 10.99m
            }
    ];
}
