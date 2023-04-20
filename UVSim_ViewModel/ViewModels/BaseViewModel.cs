using System;
using System.Collections.Generic;
using System.Collections.Specialized;
//using System.ComponentModel;
using CommunityToolkit.Mvvm;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace UVSim.ViewModel
{
    /*
    //Needed only if inheriting from a differnet base class
    //when inheriting from ObservableObject, all INotifyPropertyChanged and INotifyPropertyChanging interfaces are implemented
    [INotifyPropertyChanged]
    */
    public partial class BaseViewModel: ObservableObject
    {
        #region MEMEBERS
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotBusy))]
        bool isBusy;
        [ObservableProperty]
        string title;
        #endregion

        #region PROPERTIES
        public bool IsNotBusy => !IsBusy;
        #endregion

        #region CONSTRUCTORS
        public BaseViewModel()
        {
            isBusy = false;
        }
        #endregion

        /*
        //The following is irrelivant when using CommunityToolkit.MVVM
        //Code needed when not using source generation in CommunityToolkit.MVVM
        #region PROPERTIES
        public bool IsBusy
        {
            get => isBusy;
            set
            {
                if (isBusy == value)
                    return;

                isBusy = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsNotBusy));
            }
        }

        public string Title
        {
            get => title;
            set
            {
                if (title == value)
                    return;

                title = value;
                OnPropertyChanged();
            }
        }

        public bool IsNotBusy => !IsBusy;
        #endregion



        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        */
    }
}
