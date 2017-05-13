using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AccidentalFish.Foundations.Resources.Azure.Extensions
{
    /// <summary>
    /// Extensions for the IEnumerable interface
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Enumerates a set of pages of the given collection
        /// </summary>
        /// <typeparam name="T">Type of the paged items</typeparam>
        /// <param name="source">Source collection</param>
        /// <param name="pageSize">Size of each page</param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<T>> Page<T>(this IEnumerable<T> source, int pageSize)
        {
            using (var enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    List<T> currentPage = new List<T>(pageSize)
                    {
                        enumerator.Current
                    };

                    while (currentPage.Count < pageSize && enumerator.MoveNext())
                    {
                        currentPage.Add(enumerator.Current);
                    }
                    yield return new ReadOnlyCollection<T>(currentPage);
                }
            }
        }
    }
}
