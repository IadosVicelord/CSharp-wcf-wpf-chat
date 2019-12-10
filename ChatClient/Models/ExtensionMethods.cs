using System;
using System.Collections.Generic;
using Caliburn.Micro;

namespace Client.Models
{
    //Методы расширения
    static public class ExtensionMethods
    {
        /// <summary>
        /// Сортировка коллекции BindableCollection
        /// </summary>
        /// <typeparam name="T">Коллекция BindableCollection</typeparam>
        /// <param name="collection">Коллекция</param>
        /// <param name="comparison">Выражение сравнения</param>
        public static void Sort<T>(this BindableCollection<T> collection, Comparison<T> comparison)
        {
            var sortableList = new List<T>(collection);
            sortableList.Sort(comparison);
            sortableList.Reverse();

            for (int i = 0; i < sortableList.Count; i++)
            {
                collection.Move(collection.IndexOf(sortableList[i]), i);
            }
        }
    }
}
