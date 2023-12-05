namespace ZippsterStudios.Math
{
    using System;
    public class Weight
    {
        public double Value { get; private set; }
        public WeightUnit UnitType { get; private set; }

        public Weight(double value, WeightUnit unitType)
        {
            Set(value, unitType);
        }
        public void Set(double value)
        {
            Value = value;
        }
        public void Set(WeightUnit unitType)
        {
            UnitType = unitType;
        }
        public void Set(double value, WeightUnit unitType)
        {
            Value = value;
            UnitType = unitType;
        }
        public double Kilograms()
        {
            return ConvertWeight(WeightUnit.Kilograms);
        }

        public double Pounds()
        {
            return ConvertWeight(WeightUnit.Pounds);
        }

        private double ConvertWeight(WeightUnit targetUnit)
        {
            switch (UnitType)
            {
                case WeightUnit.Kilograms:
                    return targetUnit == WeightUnit.Pounds ? Value * 2.20462 : Value;
                case WeightUnit.Pounds:
                    return targetUnit == WeightUnit.Kilograms ? Value / 2.20462 : Value;
                default:
                    throw new InvalidOperationException("Unknown weight unit type");
            }
        }
    }
}