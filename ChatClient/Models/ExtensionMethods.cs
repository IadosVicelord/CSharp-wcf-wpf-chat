using System;
using System.Collections.Generic;
using Caliburn.Micro;

namespace Client.Models
{
    static public class ExtensionMethods
    {
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
