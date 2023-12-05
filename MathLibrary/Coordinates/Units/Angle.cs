namespace ZippsterStudios.Math
{
    using System;

    public class Angle
    {
        public double Value { get; private set; }
        public double Rad => Radians();
        public double Deg => Degrees();
        public AngleUnit UnitType { get; private set; }

        public Angle(double value, AngleUnit unitType)
        {
            Set(value, unitType);
        }

        public void Set(double value)
        {
            Value = value;
        }

        public void Set(AngleUnit unitType)
        {
            UnitType = unitType;
        }

        public void Set(double value, AngleUnit unitType)
        {
            Value = value;
            UnitType = unitType;
        }

        public double Degrees()
        {
            return ConvertAngle(AngleUnit.Degrees);
        }

        public double Radians()
        {
            return ConvertAngle(AngleUnit.Radians);
        }

        public DMS DMS()
        {
            double degrees = Degrees();
            int wholeDegrees = (int)degrees;
            double fractionalDegrees = degrees - wholeDegrees;

            double minutes = fractionalDegrees * 60;
            int wholeMinutes = (int)minutes;
            double seconds = (minutes - wholeMinutes) * 60;

            return new DMS(wholeDegrees, wholeMinutes, seconds);
        }

        public static Angle FromDMS(DMS dms)
        {
            double degrees = dms.deg.Degrees() + (dms.min.Degrees() / 60) + (dms.sec.Degrees() / 3600);
            return new Angle(degrees, AngleUnit.Degrees);
        }

        private double ConvertAngle(AngleUnit targetUnit)
        {
            switch (UnitType, targetUnit)
            {
                case (AngleUnit.Degrees, AngleUnit.Radians):
                    return Value * (Math.PI / 180.0);
                case (AngleUnit.Radians, AngleUnit.Degrees):
                    return Value * (180.0 / Math.PI);
                case (_, AngleUnit.DMS):
                    return DMS().Deg; // Placeholder for returning degrees part of DMS
                case (AngleUnit.DMS, _):
                    return FromDMS(DMS()).Degrees(); // Convert DMS back to specified unit
                default:
                    return Value;
            }
        }
    }
}