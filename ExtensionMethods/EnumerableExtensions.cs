namespace TreeStructure.ExtensionMethods
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Selects elements from a collection recursively using the specified selector function.
        /// </summary>
        /// <typeparam name="T">The type of elements in the collection.</typeparam>
        /// <param name="source">The source collection to process.</param>
        /// <param name="selector">The function to select child elements for each element in the collection.</param>
        /// <returns>Returns a collection of elements selected recursively.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the source or selector is null.</exception>
        public static IEnumerable<T> SelectRecursive<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> selector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (selector == null) throw new ArgumentNullException(nameof(selector));

            var stack = new Stack<IEnumerator<T>>();
            var enumerator = source.GetEnumerator();

            try
            {
                while (true)
                {
                    if (enumerator.MoveNext())
                    {
                        var current = enumerator.Current;
                        yield return current;

                        var children = selector(current);
                        if (children != null)
                        {
                            stack.Push(enumerator);
                            enumerator = children.GetEnumerator();
                        }
                    }
                    else
                    {
                        if (stack.Count == 0) break;

                        enumerator.Dispose();
                        enumerator = stack.Pop();
                    }
                }
            }
            finally
            {
                enumerator.Dispose();
                while (stack.Count > 0)
                {
                    stack.Pop().Dispose();
                }
            }
        }
    }
}