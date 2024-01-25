using RealEstateApp.Models;
using RealEstateApp.Services;
using RealEstateApp.Views;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

namespace RealEstateApp.ViewModels;

[QueryProperty(nameof(Property), "Property")]
public class ImageListViewModel : BaseViewModel, ILeaving
{
    private Property _property;
    public Property Property
    {
        get { return _property; }
        set { _property = value; OnPropertyChanged(); }
    }

    private bool _isNotSupported;
    public bool IsNotSupported
    {
        get { return _isNotSupported; }
        set { _isNotSupported = value; OnPropertyChanged(); }
    }


    public ImageListViewModel()
    {
        IsNotSupported = !Accelerometer.Default.IsSupported;

        if(Accelerometer.Default.IsSupported)
        {
            if (!Accelerometer.Default.IsMonitoring)
                Accelerometer.Default.Start(SensorSpeed.UI);

            Accelerometer.Default.ReadingChanged += Default_ReadingChanged;
        }
    }

    private int _interval;
    public int Interval
    {
        get => _interval;
        set { _interval = value; OnPropertyChanged(); }
    }

    private void Default_ReadingChanged(object sender, AccelerometerChangedEventArgs e)
    {
        if(e.Reading.Acceleration.X >= 1)
        {
            if (Interval + 1 > Property.ImageUrls.Count - 1)
                Interval = 0;
            else
                Interval++;
        }
        else if (e.Reading.Acceleration.X <= -1)
        {
            if (Interval - 1 < 0)
                Interval = Property.ImageUrls.Count - 1;
            else
                Interval--;
        }
    }

    public void OnLeaving()
    {
        if (Accelerometer.Default.IsMonitoring)
            Accelerometer.Default.Stop();
    }
}
