using RealEstateApp.Models;
using RealEstateApp.Services;
using RealEstateApp.Views;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace RealEstateApp.ViewModels;

public class HeightCalculatorPageViewModel : BaseViewModel, ILeaving
{
	private double _currentPressure;
	public double CurrentPressure
	{
		get { return _currentPressure; }
		set { _currentPressure = value; OnPropertyChanged(); }
	}

    private double _currentAltitude;
    public double CurrentAltitude
    {
        get { return _currentAltitude; }
        set { _currentAltitude = value; OnPropertyChanged(); }
    }

    public ObservableCollection<BarometerData> Observations { get; set; } = new ObservableCollection<BarometerData>();

    public HeightCalculatorPageViewModel()
    {
        if(!Barometer.Default.IsMonitoring)
            Barometer.Default.Start(SensorSpeed.UI);

        Barometer.Default.ReadingChanged += Default_ReadingChanged;
    }

    private void Default_ReadingChanged(object sender, BarometerChangedEventArgs e)
    {
        CurrentAltitude = 44307.694 * (1 - Math.Pow(CurrentPressure / 998.7, 0.190284));
    }

    public void OnLeaving()
    {
        if (Barometer.Default.IsMonitoring)
            Barometer.Default.Stop();
    }
}
