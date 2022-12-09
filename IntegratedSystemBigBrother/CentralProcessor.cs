using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;

namespace IntegratedSystemBigBrother
{
    public class CentralProcessor : INotifyPropertyChanged
    {
        private ObservableCollection<PeripheralProcessor> _network;
        public ObservableCollection<PeripheralProcessor> Network
        {
            get { return _network; }
            set
            {
                if (!value.SequenceEqual(_network))
                    _network = value;
                OnPropertyChanged(nameof(Network));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
