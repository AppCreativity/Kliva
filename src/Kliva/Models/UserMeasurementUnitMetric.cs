using GalaSoft.MvvmLight.Messaging;
using Kliva.Helpers;
using Kliva.Messages;
using Microsoft.Practices.ServiceLocation;
using System.Diagnostics;

namespace Kliva.Models
{
    /// <summary>
    /// This class holds a particular metric value and automatically converts it to the Unit specified by the user  
    /// </summary>
    public class UserMeasurementUnitMetric : BaseClass
    {
        //The value we received from the backend
        readonly float rawValue;

        //DistanceUnitType as received from the backend
        readonly DistanceUnitType? rawDistanceUnitType;
        //DistanceUnitType selected by the user
        private DistanceUnitType? userSelectedDistanceUnitType;

        //SpeedUnit as received from the backend
        readonly SpeedUnit? rawSpeedUnit;
        //SpeedUnit selected by the user
        private SpeedUnit? userSelectedSpeedUnit;

        //The RawValue converted to the User's preferred Unit
        private string _FormattedValue;
        public string FormattedValue
        {
            get { return _FormattedValue; }
            private set
            {
                 Set(() => FormattedValue, ref _FormattedValue, value); 
            }
        }

        //The RawValue converted to the User's preferred Unit, including the unit itself
        private string _FormattedValueWithUnit;
        public string FormattedValueWithUnit
        {
            get { return _FormattedValueWithUnit; }
            private set
            {
                Set(() => FormattedValueWithUnit, ref _FormattedValueWithUnit, value);
            }
        }

        public UserMeasurementUnitMetric(float rawValue, DistanceUnitType rawDistanceUnitType, 
            DistanceUnitType userSelectedDistanceUnitType) : this(rawValue)
        {
            this.rawValue = rawValue;
            this.rawDistanceUnitType = rawDistanceUnitType;
            this.userSelectedDistanceUnitType = userSelectedDistanceUnitType;
            CalculateFormattedValue();
        }

        public UserMeasurementUnitMetric(float rawValue, SpeedUnit rawSpeedUnit,
            SpeedUnit userSelectedSpeedUnit) : this(rawValue)
        {
            this.rawSpeedUnit = rawSpeedUnit;
            this.userSelectedSpeedUnit = userSelectedSpeedUnit;
            CalculateFormattedValue();
        }

        private UserMeasurementUnitMetric(float rawValue)
        {
            this.rawValue = rawValue;
            RegisterForMessage();
        }

        private void RegisterForMessage()
        {
            ServiceLocator.Current.GetInstance<IMessenger>()
                .Register<MeasureUnitChangedMessage>(this, OnMeasureUnitChangedMessge);
        }

        private void OnMeasureUnitChangedMessge(MeasureUnitChangedMessage msg)
        {
            if (msg.NewValue == DistanceUnitType.Kilometres || msg.NewValue == DistanceUnitType.Metres)
            {
                //The user selected metrics as preferred measure unit
                if (userSelectedDistanceUnitType.HasValue)
                {
                    //The current metric is a Distance unit
                    if (userSelectedDistanceUnitType == DistanceUnitType.Feet)
                        userSelectedDistanceUnitType = DistanceUnitType.Metres;
                    else if (userSelectedDistanceUnitType == DistanceUnitType.Miles)
                        userSelectedDistanceUnitType = DistanceUnitType.Kilometres;
                }
                else if(userSelectedSpeedUnit.HasValue)
                {
                    //The current metric is a Speed unit
                    if (userSelectedSpeedUnit == SpeedUnit.MilesPerHour)
                        userSelectedSpeedUnit = SpeedUnit.KilometresPerHour;
                }
            }
            else
            {
                //The user selected imperial as preferred measure unit
                if (userSelectedDistanceUnitType.HasValue)
                {
                    //The current metric is a Distance unit
                    if (userSelectedDistanceUnitType == DistanceUnitType.Metres)
                        userSelectedDistanceUnitType = DistanceUnitType.Feet;
                    else if (userSelectedDistanceUnitType == DistanceUnitType.Kilometres)
                        userSelectedDistanceUnitType = DistanceUnitType.Miles;
                }
                else if (userSelectedSpeedUnit.HasValue)
                {
                    //The current metric is a Speed unit
                    if (userSelectedSpeedUnit == SpeedUnit.KilometresPerHour)
                        userSelectedSpeedUnit = SpeedUnit.MilesPerHour;
                }
            }
            CalculateFormattedValue();
        }

        private void CalculateFormattedValue()
        {
            string newValue = string.Empty;
            if (rawDistanceUnitType.HasValue && userSelectedDistanceUnitType.HasValue)
            {
                newValue = UnitConverter.ConvertDistance(rawValue, rawDistanceUnitType.Value,
                    userSelectedDistanceUnitType.Value).ToString("F1");
            }
            else if (rawSpeedUnit.HasValue)
            {
                newValue = UnitConverter.ConvertSpeed(rawValue, rawSpeedUnit.Value,
                    userSelectedSpeedUnit.Value).ToString("F1");
            }
            else
            {
                //should not occur 
                //If it does occur, please investigate where it comes from
                if (Debugger.IsAttached)
                    Debugger.Break();
            }

            FormattedValue = newValue;
            var unit = userSelectedSpeedUnit.HasValue ?
                Helpers.Converters.SpeedConverter.Convert(userSelectedSpeedUnit.Value, typeof(SpeedUnit), null, string.Empty) :
                Helpers.Converters.DistanceConverter.Convert(userSelectedDistanceUnitType.Value, typeof(DistanceUnitType), null, string.Empty);
            FormattedValueWithUnit = $"{newValue} {unit}";
        }
    }
}
