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

    private string _measurementLabel;
    public string MeasurementLabel
    {
        get { return _measurementLabel; }
        set { _measurementLabel = value; OnPropertyChanged(); }
    }


    public ObservableCollection<BarometerMeasurement> Observations { get; set; } = new ObservableCollection<BarometerMeasurement>();

    public HeightCalculatorPageViewModel()
    {
        if(!Barometer.Default.IsMonitoring)
            Barometer.Default.Start(SensorSpeed.UI);

        Barometer.Default.ReadingChanged += Default_ReadingChanged;
    }

    private void Default_ReadingChanged(object sender, BarometerChangedEventArgs e)
    {
        CurrentPressure = e.Reading.PressureInHectopascals;
        CurrentAltitude = 44307.694 * (1 - Math.Pow(CurrentPressure / 998.7, 0.190284));
    }

    public void OnLeaving()
    {
        if (Barometer.Default.IsMonitoring)
            Barometer.Default.Stop();
    }

    private double? lastAltitude;
    private Command _addObservationCommand;
    public ICommand AddObservationCommand => _addObservationCommand ??= new Command(() =>
    {
        Observations.Add(new BarometerMeasurement()
        {
            Label = MeasurementLabel,
            Altitude = CurrentAltitude,
            Pressure = CurrentPressure,
            HeightChange = lastAltitude != null ? CurrentAltitude - lastAltitude.Value : 0,
        });
        lastAltitude = CurrentAltitude;
    });
}
