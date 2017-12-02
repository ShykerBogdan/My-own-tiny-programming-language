using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
namespace Freel.Tools
{
 public static class ObservableCollectionExtensions
    {
        public static ObservableCollection<T> CopyFrom<T>(this ObservableCollection<T> observableCollection, List<T> list)
        {
            observableCollection?.Clear();
            foreach (var item in list)
            {
                observableCollection.Add(item);
            }
            return observableCollection;
        }

        public static ObservableCollection<T> CopyFrom<T>(this ObservableCollection<T> observableCollection, List<T> list, List<T> list2)
        {
            observableCollection?.Clear();
            foreach (var item in list)
            {
                observableCollection.Add(item);
            }
            foreach (var item in list2)
            {
                observableCollection.Add(item);
            }

            return observableCollection;
        }
    }
}
