using RealEstateApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateApp.Services
{
    public class BatteryService
    {
        public BatteryState State { get; set; }
        public double ChargeLevel { get; set; }
        public BatteryPowerSource PowerSource { get; set; }
        public EnergySaverStatus EnergySaverStatus { get; set; }

        public delegate void BatteryStatusCallback();
        public BatteryStatusCallback OnStatusChanged { get; set; } = delegate { };

        public BatteryService()
        {
            State = Battery.State;
            ChargeLevel = Battery.ChargeLevel;
            PowerSource = Battery.PowerSource;
            EnergySaverStatus = Battery.EnergySaverStatus;

            Battery.BatteryInfoChanged += Battery_BatteryInfoChanged;
            Battery.EnergySaverStatusChanged += Battery_EnergySaverStatusChanged;
        }

        private void Battery_EnergySaverStatusChanged(object sender, EnergySaverStatusChangedEventArgs e)
        {
            this.EnergySaverStatus = e.EnergySaverStatus;
            OnStatusChanged?.Invoke();
        }

        private void Battery_BatteryInfoChanged(object sender, BatteryInfoChangedEventArgs e)
        {
            this.State = e.State;
            this.PowerSource = e.PowerSource;
            this.ChargeLevel = e.ChargeLevel;

            OnStatusChanged?.Invoke();
        }
    }
}
