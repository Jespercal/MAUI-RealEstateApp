using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateApp.Services
{
    public class ConnectivityService
    {
        public bool IsConnected { get; set; }

        public delegate void ConnectivityStatusCallback(bool hasAccess);

        public ConnectivityStatusCallback OnStatusChanged { get; set; } = delegate { };

        public ConnectivityService()
        {
            IsConnected = Connectivity.Current.NetworkAccess == NetworkAccess.Internet;
            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
        }

        private void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            IsConnected = e.NetworkAccess == NetworkAccess.Internet;
            OnStatusChanged?.Invoke(IsConnected);
        }
    }
}
