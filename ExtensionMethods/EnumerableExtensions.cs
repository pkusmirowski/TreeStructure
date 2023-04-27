namespace TreeStructure.ExtensionMethods
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Wybiera elementy z kolekcji rekurencyjnie, używając określonej funkcji selektora.
        /// </summary>
        /// <typeparam name="T">Typ elementów w kolekcji.</typeparam>
        /// <param name="source">Kolekcja źródłowa do przetworzenia.</param>
        /// <param name="selector">Funkcja do wyboru elementów podrzędnych dla każdego elementu w kolekcji.</param>
        /// <returns>Zwraca kolekcję elementów wybranych rekurencyjnie.</returns>
        public static IEnumerable<T> SelectRecursive<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> selector)
        {
            // Przechodzi przez wszystkie elementy w kolekcji źródłowej
            foreach (var parent in source)
            {
                // Zwraca bieżący element (rodzic)
                yield return parent;

                // Wybiera elementy podrzędne dla bieżącego elementu i przetwarza je rekurencyjnie
                foreach (var child in selector(parent).SelectRecursive(selector))
                {
                    // Zwraca przetworzone elementy podrzędne
                    yield return child;
                }
            }
        }
    }
}
