namespace Kliva.Models.Interfaces
{
    public interface ISupportUserMeasurementUnits
    {
        DistanceUnitType MeasurementUnit { get; }
        void SetUserMeasurementUnits(DistanceUnitType measurementUnit);
    }
}
