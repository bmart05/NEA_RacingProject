﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Utilities
{
    //NOT MY CODE! Credit to https://stackoverflow.com/questions/2019417/how-to-access-random-item-in-list
    public static class EnumerableExtensions
    {
        public static T PickRandom<T>(this IEnumerable<T> source)
        {
            return source.PickRandom(1).Single();
        }

        public static IEnumerable<T> PickRandom<T>(this IEnumerable<T> source, int count)
        {
            return source.Shuffle().Take(count);
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            return source.OrderBy(x => Guid.NewGuid());
        }
    }
}