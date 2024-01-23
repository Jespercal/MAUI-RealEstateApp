using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RealEstateApp.Models;
public class PropertyListItem : INotifyPropertyChanged
{
    public PropertyListItem(Property property)
    {
        Property = property;
    }

    private Property _property;
    public Property Property
    {
        get => _property;
        set
        {
            _property = value;
            OnPropertyChanged();
        }
    }

    private double? _distance;
    public double? Distance
    {
        get { return _distance; }
        set { _distance = value;OnPropertyChanged(); }
    }

    public void CalcDistance( Location location )
    {
        try
        {
            Distance = Location.CalculateDistance(Property.Latitude.Value, Property.Longitude.Value, location, DistanceUnits.Kilometers);
        }
        catch ( Exception ex )
        {
            Distance = null;
        }
    }


    public event PropertyChangedEventHandler PropertyChanged;

    public void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
