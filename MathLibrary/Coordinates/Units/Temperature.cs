namespace ZippsterStudios.Math
{
    using System;
    public class Temperature
    {
        public double Value { get; private set; }
        public TemperatureUnit UnitType { get; private set; }

        public Temperature(double value, TemperatureUnit unitType)
        {
            Set(value, unitType);
        }
        public void Set(double value)
        {
            Value = value;
        }
        public void Set(TemperatureUnit unitType)
        {
            UnitType = unitType;
        }
        public void Set(double value, TemperatureUnit unitType)
        {
            Value = value;
            UnitType = unitType;
        }
        public double Celsius()
        {
            return ConvertTemperature(TemperatureUnit.Celsius);
        }

        public double Fahrenheit()
        {
            return ConvertTemperature(TemperatureUnit.Fahrenheit);
        }

        public double Kelvin()
        {
            return ConvertTemperature(TemperatureUnit.Kelvin);
        }

        private double ConvertTemperature(TemperatureUnit targetUnit)
        {
            switch (UnitType)
            {
                case TemperatureUnit.Celsius:
                    return targetUnit switch
                    {
                        TemperatureUnit.Fahrenheit => (Value * 9 / 5) + 32,
                        TemperatureUnit.Kelvin => Value + 273.15,
                        _ => Value
                    };
                case TemperatureUnit.Fahrenheit:
                    return targetUnit switch
                    {
                        TemperatureUnit.Celsius => (Value - 32) * 5 / 9,
                        TemperatureUnit.Kelvin => (Value - 32) * 5 / 9 + 273.15,
                        _ => Value
                    };
                case TemperatureUnit.Kelvin:
                    return targetUnit switch
                    {
                        TemperatureUnit.Celsius => Value - 273.15,
                        TemperatureUnit.Fahrenheit => (Value - 273.15) * 9 / 5 + 32,
                        _ => Value
                    };
                default:
                    throw new InvalidOperationException("Unknown temperature unit type");
            }
        }
    }
}