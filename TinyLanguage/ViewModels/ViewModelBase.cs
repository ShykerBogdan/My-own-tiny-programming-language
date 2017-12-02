using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Freel.ViewModels
{
    class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string name = null)
        {
            bool propertyChanged = false;

            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                OnPropertyChanged(name);
                propertyChanged = true;
            }

            return propertyChanged;
        }

        protected void OnPropertyChanged([CallerMemberName]string name = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
