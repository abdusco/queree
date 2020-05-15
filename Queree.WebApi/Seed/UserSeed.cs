using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Queree.WebApi.Seed
{
    public static class UserSeed
    {
        public static void SeedUsers(this DbContext context)
        {
            var users = context.Set<User>();
            if (users.Any())
            {
                return;
            }

            users.AddRange(new List<User>()
            {
                new User {Id = 1, Name = "Tom Cruise", BirthDate = DateTime.Parse("1950-01-01")},
                new User {Id = 2, Name = "Angelina Jolie", BirthDate = DateTime.Parse("1961-01-01")},
                new User {Id = 3, Name = "Morgan Freeman", BirthDate = DateTime.Parse("1972-01-01")},
                new User {Id = 4, Name = "Meryl Streep", BirthDate = DateTime.Parse("1973-01-01")},
                new User {Id = 5, Name = "Keanu Reeves", BirthDate = DateTime.Parse("1984-01-01")},
                new User {Id = 6, Name = "Jennifer Lawrence", BirthDate = DateTime.Parse("1975-01-01")},
                new User {Id = 7, Name = "Charlize Theron", BirthDate = DateTime.Parse("1986-01-01")},
                new User {Id = 8, Name = "Jim Carrey", BirthDate = DateTime.Parse("1957-01-01")},
            });
            context.SaveChanges();
        }
    }
}