namespace ZippsterStudios.Math
{
    public class Coordinate
    {
        private ECEF ecef;
        private ECEF refECEF;
        public ENU Enu => CoordinateConversions.ECEF2ENU(ecef, refECEF);
        public RAE Rae => CoordinateConversions.ECEF2RAE(ecef, refECEF);
        public LLA Lla => CoordinateConversions.ECEF2LLA(ecef);
        public ECEF Ecef => ecef;
        #region Constructors
        public Coordinate(LLA lla)
        {
            Set(lla);
        }
        public Coordinate(LLA lla, LLA refLLA)
        {
            Set(lla, refLLA);
        }
        public Coordinate(LLA lla, ECEF refECEF)
        {
            Set(lla, refECEF);
        }
        public Coordinate(ECEF ecef)
        {
            Set(ecef);
        }
        public Coordinate(ECEF ecef, ECEF refECEF)
        {
            Set(ecef, refECEF);
        }
        public Coordinate(ECEF ecef, LLA refLLA)
        {
            Set(ecef, refLLA);
        }
        public Coordinate(ENU enu, LLA refLLA)
        {
            Set(enu, refLLA);
        }
        public Coordinate(ENU enu, ECEF refECEF)
        {
            Set(enu, refECEF);
        }
        public Coordinate(RAE rae, LLA refLLA)
        {
            Set(rae, refLLA);
        }
        public Coordinate(RAE rae, ECEF refECEF)
        {
            Set(rae, refECEF);
        }
        #endregion
        #region Setters
        public void Set(LLA lla)
        {
            this.ecef = CoordinateConversions.LLA2ECEF(lla);
            this.refECEF = this.ecef;
        }
        public void Set(LLA lla, LLA refLLA)
        {
            this.ecef = CoordinateConversions.LLA2ECEF(lla);
            this.refECEF = CoordinateConversions.LLA2ECEF(refLLA);
        }
        public void Set(LLA lla, ECEF refECEF)
        {
            this.ecef = CoordinateConversions.LLA2ECEF(lla);
            this.refECEF = refECEF;
        }
        public void Set(ECEF ecef)
        {
            this.ecef = ecef;
            this.refECEF = ecef;
        }
        public void Set(ECEF ecef, ECEF refECEF)
        {
            this.ecef = ecef;
            this.refECEF = refECEF;
        }
        public void Set(ECEF ecef, LLA refLLA)
        {
            this.ecef = ecef;
            this.refECEF = CoordinateConversions.LLA2ECEF(refLLA);
        }

        public void Set(ENU enu, LLA refLLA)
        {
            this.ecef = CoordinateConversions.ENU2ECEF(enu, refLLA);
            this.refECEF = CoordinateConversions.LLA2ECEF(refLLA);
        }

        public void Set(ENU enu, ECEF refECEF)
        {
            LLA refLLA = CoordinateConversions.ECEF2LLA(refECEF);
            this.ecef = CoordinateConversions.ENU2ECEF(enu, refLLA);
            this.refECEF = refECEF;
        }

        public void Set(RAE rae, LLA refLLA)
        {
            ENU enu = CoordinateConversions.RAE2ENU(rae); // Assuming RAE2ENU method exists
            this.ecef = CoordinateConversions.ENU2ECEF(enu, refLLA);
            this.refECEF = CoordinateConversions.LLA2ECEF(refLLA);
        }

        public void Set(RAE rae, ECEF refECEF)
        {
            LLA refLLA = CoordinateConversions.ECEF2LLA(refECEF);
            ENU enu = CoordinateConversions.RAE2ENU(rae); // Assuming RAE2ENU method exists
            this.ecef = CoordinateConversions.ENU2ECEF(enu, refLLA);
            this.refECEF = refECEF;
        }
        #endregion
        #region Reference Settings
        public void SetRefOnly(ECEF ecef)
        {
            this.refECEF = ecef;
        }
        public void SetRefOnly(LLA lla)
        {
            this.refECEF = CoordinateConversions.LLA2ECEF(lla);
        }
        public void SetValue(ECEF ecef)
        {
            this.ecef = ecef;
        }
        public void SetValue(LLA lla)
        {
            this.ecef = CoordinateConversions.LLA2ECEF(lla);
        }
        #endregion
        public Coordinate CompareCoordinates(Coordinate otherCoordinate)
        {
            return new Coordinate(ecef, otherCoordinate.ecef);
        }
    }
}