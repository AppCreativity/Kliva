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
        readonly float _rawValue;

        //DistanceUnitType as received from the backend
        readonly DistanceUnitType? _rawDistanceUnitType;
        //DistanceUnitType selected by the user
        private DistanceUnitType? _userSelectedDistanceUnitType;

        //SpeedUnit as received from the backend
        readonly SpeedUnit? _rawSpeedUnit;
        //SpeedUnit selected by the user
        private SpeedUnit? _userSelectedSpeedUnit;

        //The RawValue converted to the User's preferred Unit
        private string _formattedValue;
        public string FormattedValue
        {
            get { return _formattedValue; }
            private set
            {
                 Set(() => FormattedValue, ref _formattedValue, value); 
            }
        }

        //The RawValue converted to the User's preferred Unit, including the unit itself
        private string _formattedValueWithUnit;
        public string FormattedValueWithUnit
        {
            get { return _formattedValueWithUnit; }
            private set
            {
                Set(() => FormattedValueWithUnit, ref _formattedValueWithUnit, value);
            }
        }

        public UserMeasurementUnitMetric(float rawValue, DistanceUnitType rawDistanceUnitType, 
            DistanceUnitType userSelectedDistanceUnitType) : this(rawValue)
        {
            _rawValue = rawValue;
            _rawDistanceUnitType = rawDistanceUnitType;
            _userSelectedDistanceUnitType = userSelectedDistanceUnitType;
            CalculateFormattedValue();
        }

        public UserMeasurementUnitMetric(float rawValue, SpeedUnit rawSpeedUnit,
            SpeedUnit userSelectedSpeedUnit) : this(rawValue)
        {
            _rawSpeedUnit = rawSpeedUnit;
            _userSelectedSpeedUnit = userSelectedSpeedUnit;
            CalculateFormattedValue();
        }

        private UserMeasurementUnitMetric(float rawValue)
        {
            _rawValue = rawValue;
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
                if (_userSelectedDistanceUnitType.HasValue)
                {
                    //The current metric is a Distance unit
                    if (_userSelectedDistanceUnitType == DistanceUnitType.Feet)
                        _userSelectedDistanceUnitType = DistanceUnitType.Metres;
                    else if (_userSelectedDistanceUnitType == DistanceUnitType.Miles)
                        _userSelectedDistanceUnitType = DistanceUnitType.Kilometres;
                }
                else if(_userSelectedSpeedUnit.HasValue)
                {
                    //The current metric is a Speed unit
                    if (_userSelectedSpeedUnit == SpeedUnit.MilesPerHour)
                        _userSelectedSpeedUnit = SpeedUnit.KilometresPerHour;
                }
            }
            else
            {
                //The user selected imperial as preferred measure unit
                if (_userSelectedDistanceUnitType.HasValue)
                {
                    //The current metric is a Distance unit
                    if (_userSelectedDistanceUnitType == DistanceUnitType.Metres)
                        _userSelectedDistanceUnitType = DistanceUnitType.Feet;
                    else if (_userSelectedDistanceUnitType == DistanceUnitType.Kilometres)
                        _userSelectedDistanceUnitType = DistanceUnitType.Miles;
                }
                else if (_userSelectedSpeedUnit.HasValue)
                {
                    //The current metric is a Speed unit
                    if (_userSelectedSpeedUnit == SpeedUnit.KilometresPerHour)
                        _userSelectedSpeedUnit = SpeedUnit.MilesPerHour;
                }
            }
            CalculateFormattedValue();
        }

        private void CalculateFormattedValue()
        {
            string newValue = string.Empty;
            if (_rawDistanceUnitType.HasValue && _userSelectedDistanceUnitType.HasValue)
            {
                newValue = UnitConverter.ConvertDistance(_rawValue, _rawDistanceUnitType.Value,
                    _userSelectedDistanceUnitType.Value).ToString("F1");
            }
            else if (_rawSpeedUnit.HasValue)
            {
                newValue = UnitConverter.ConvertSpeed(_rawValue, _rawSpeedUnit.Value,
                    _userSelectedSpeedUnit.Value).ToString("F1");
            }
            else
            {
                //should not occur 
                //If it does occur, please investigate where it comes from
                if (Debugger.IsAttached)
                    Debugger.Break();
            }

            FormattedValue = newValue;
            var unit = _userSelectedSpeedUnit.HasValue ?
                Helpers.Converters.SpeedConverter.Convert(_userSelectedSpeedUnit.Value, typeof(SpeedUnit), null, string.Empty) :
                Helpers.Converters.DistanceConverter.Convert(_userSelectedDistanceUnitType.Value, typeof(DistanceUnitType), null, string.Empty);
            FormattedValueWithUnit = $"{newValue} {unit}";
        }
    }
}
