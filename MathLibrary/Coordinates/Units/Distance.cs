namespace ZippsterStudios.Math
{
    using System;
    public class Distance
    {
        public double Value { get; private set; }
        public DistanceUnit UnitType { get; private set; }

        public Distance(double value, DistanceUnit unitType)
        {
            Set(value, unitType);
        }
        public void Set(double value)
        {
            Value = value;
        }
        public void Set(DistanceUnit unitType)
        {
            UnitType = unitType;
        }
        public void Set(double value, DistanceUnit unitType)
        {
            Value = value;
            UnitType = unitType;
        }
        public double Meters()
        {
            return ConvertDistance(DistanceUnit.Meters);
        }

        public double Feet()
        {
            return ConvertDistance(DistanceUnit.Feet);
        }

        public double Kilometers()
        {
            return ConvertDistance(DistanceUnit.Kilometers);
        }

        public double Miles()
        {
            return ConvertDistance(DistanceUnit.Miles);
        }

        private double ConvertDistance(DistanceUnit targetUnit)
        {
            switch (UnitType)
            {
                case DistanceUnit.Meters:
                    return targetUnit switch
                    {
                        DistanceUnit.Feet => Value * 3.28084,
                        DistanceUnit.Kilometers => Value / 1000.0,
                        DistanceUnit.Miles => Value / 1609.344,
                        _ => Value
                    };
                case DistanceUnit.Feet:
                    return targetUnit switch
                    {
                        DistanceUnit.Meters => Value / 3.28084,
                        DistanceUnit.Kilometers => Value / 3280.84,
                        DistanceUnit.Miles => Value / 5280.0,
                        _ => Value
                    };
                case DistanceUnit.Kilometers:
                    return targetUnit switch
                    {
                        DistanceUnit.Meters => Value * 1000.0,
                        DistanceUnit.Feet => Value * 3280.84,
                        DistanceUnit.Miles => Value / 1.609344,
                        _ => Value
                    };
                case DistanceUnit.Miles:
                    return targetUnit switch
                    {
                        DistanceUnit.Meters => Value * 1609.344,
                        DistanceUnit.Feet => Value * 5280.0,
                        DistanceUnit.Kilometers => Value * 1.609344,
                        _ => Value
                    };
                default:
                    throw new InvalidOperationException("Unknown distance unit type");
            }
        }

    }
}