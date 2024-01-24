using RealEstateApp.Models;
using RealEstateApp.Services;
using RealEstateApp.Views;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

namespace RealEstateApp.ViewModels;

[QueryProperty(nameof(Property), "property")]
public class CompassViewModel : BaseViewModel, ILeaving
{
	private string _currentAspect;
	public string CurrentAspect
    {
		get { return _currentAspect; }
		set { _currentAspect = value; OnPropertyChanged(); }
	}

	private Property _property;
	public Property Property
	{
		get { return _property; }
		set { _property = value; OnPropertyChanged(); Property.Aspect = "Test"; }
	}

	private double _rotationAngle;
	public double RotationAngle
    {
		get { return _rotationAngle; }
		set { _rotationAngle = value; OnPropertyChanged(); }
	}

    private double _currentHeading;
    public double CurrentHeading
    {
        get { return _currentHeading; }
        set { _currentHeading = value; OnPropertyChanged(); }
    }

    public CompassViewModel()
    {
		if (!Compass.Default.IsMonitoring)
			Compass.Default.Start(SensorSpeed.UI);

        Compass.Default.ReadingChanged += Default_ReadingChanged;
    }

    private void Default_ReadingChanged(object sender, CompassChangedEventArgs e)
    {
        CurrentHeading = e.Reading.HeadingMagneticNorth;
        RotationAngle = -e.Reading.HeadingMagneticNorth;
        if (e.Reading.HeadingMagneticNorth > 45 + 90 + 90 + 90 || e.Reading.HeadingMagneticNorth <= 45)
            CurrentAspect = "North";
        else if (e.Reading.HeadingMagneticNorth > 45 && e.Reading.HeadingMagneticNorth <= 45+90)
            CurrentAspect = "East";
        else if (e.Reading.HeadingMagneticNorth > 45 + 90 && e.Reading.HeadingMagneticNorth < 45 + 90 + 90)
            CurrentAspect = "South";
        else if (e.Reading.HeadingMagneticNorth > 45 + 90 + 90 && e.Reading.HeadingMagneticNorth < 45 + 90 + 90 + 90)
            CurrentAspect = "West";
    }

    public void OnLeaving()
    {
        if (Compass.Default.IsMonitoring)
            Compass.Default.Stop();

        Property.Aspect = CurrentAspect;
    }
}
