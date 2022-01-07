using NzbDrone.Core.Movies;
using System.Collections.Generic;

namespace RadarrPusherApi.Common.Tests.Factories
{
    public static class MovieFactory
    {
        public static IList<Movie> CreateMovies(int total)
        {
            IList<Movie> movies = new List<Movie>();

            for (var i = 0; i < total; i++)
            {
                movies.Add(new Movie
                {
                    Title = $"Movie Title {i}"
                });
            }

            return movies;
        }
    }
}