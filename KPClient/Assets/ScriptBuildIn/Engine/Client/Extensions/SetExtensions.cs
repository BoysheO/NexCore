using System.Collections.Generic;

namespace NexCore.Extensions
{
    public static class SetExtensions
    {
        public static ISet<T> AddRang<T>(ISet<T> set, IEnumerable<T> collection)
        {
            foreach (var item in collection)
            {
                set.Add(item);
            }

            return set;
        }
    }
}