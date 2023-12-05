

namespace ZippsterStudios.Math
{
using GeoConvert;
using System;
    public static class CoordinateConversions
    {
        public static GeoConvertor converter = new GeoConvertor();
        #region Constants
        private static int N_ROUND_DIGITS = 8; // Single rounding constant for all conversions
        private static double a = 6378137.0; // WGS-84 Earth semimajor axis (m)
        private static double f = 1.0 / 298.257223563; // WGS-84 Flattening
        private static double b = (1.0 - f) * a; // Semiminor axis
        private static double e2 = f * (2 - f); //Square of first eccentricity
        private static double ep2 = e2 / (e - e2);//Square of second eccentricity
        private static double e = System.Math.Sqrt(1 - (b * b) / (a * a));
        private static double ep = System.Math.Sqrt((a * a) - (b * b)) / b;
        #endregion
        #region LLA to ECEF
        public static ECEF LLA2ECEF2(LLA lla)
        {
            double[] ecef = converter.ECEFfromLLA(lla.Lat, lla.Lon, lla.Alt);
            return new ECEF(ecef[0], ecef[1], ecef[2]);
        }
        public static ECEF LLA2ECEF(LLA lla)
        {
            double N = a / System.Math.Sqrt(1 - System.Math.Pow(e * System.Math.Sin(lla.lat.Rad), 2));
            double x = System.Math.Round((N + lla.Alt) * System.Math.Cos(lla.lat.Rad) * System.Math.Cos(lla.lon.Rad), N_ROUND_DIGITS);
            double y = System.Math.Round((N + lla.Alt) * System.Math.Cos(lla.lat.Rad) * System.Math.Sin(lla.lon.Rad), N_ROUND_DIGITS);
            double z = System.Math.Round((N * (1 - e * e) + lla.Alt) * System.Math.Sin(lla.lat.Rad), N_ROUND_DIGITS);

            return new ECEF(x, y, z);
        }
        #endregion
        #region LLA to ENU
        public static ENU LLA2ENU(LLA lla, LLA refLLA)
        {
            double[] enu = converter.ENUfromLLA(lla.Lat, lla.Lon, lla.Alt, refLLA.Lat, refLLA.Lon, refLLA.Alt);
            return new ENU(enu[0], enu[1], enu[2]);
        }

        public static ENU LLA2ENU(LLA lla, ECEF refECEF)
        {
            LLA refLLA = ECEF2LLA(refECEF);
            return LLA2ENU(lla, refLLA);
        }
        #endregion
        #region LLA to RAE
        public static RAE LLA2RAE(LLA lla, LLA refLLA)
        {
            ENU enu = LLA2ENU(lla, refLLA);
            return ENU2RAE(enu);
        }
        public static RAE LLA2RAE(LLA lla, ECEF refECEF)
        {
            ENU enu = LLA2ENU(lla, refECEF);
            return ENU2RAE(enu);
        }
        #endregion

        #region ECEF to LLA
        public static LLA ECEF2LLA(ECEF ecef)
        {
            double lambda = Math.Atan2(ecef.Y, ecef.X);
            double rho = Math.Sqrt(ecef.X * ecef.X + ecef.Y * ecef.Y);
            double beta = Math.Atan2(ecef.Z, (1 - f) * rho);


            double phi = CalculatePhi(ecef, beta, rho);
            double betaNew = CalculateBetaNew(phi);

            int count = 0;
            while (beta != betaNew && count < 5)
            {
                beta = betaNew;
                phi = CalculatePhi(ecef, beta, rho);
                betaNew = CalculateBetaNew(phi);
                count++;
            }

            double sinphi = Math.Sin(phi);
            double N = a / Math.Sqrt(1 - e2 * sinphi * sinphi);
            double h = rho * Math.Cos(phi) + (ecef.Z + e2 * N * sinphi) * sinphi - N;
            phi = ToDegrees(phi);
            lambda = ToDegrees(lambda);

            phi = System.Math.Round(phi, N_ROUND_DIGITS);
            lambda = System.Math.Round(lambda, N_ROUND_DIGITS);
            h = System.Math.Round(h, N_ROUND_DIGITS);

            return new LLA(phi, lambda, h);
        }
        public static double CalculatePhi(ECEF ecef, double beta, double rho)
        {
            return Math.Atan2(ecef.Z + b * ep2 * Math.Pow(Math.Sin(beta), 3),
                          rho - a * e2 * Math.Pow(Math.Cos(beta), 3));
        }
        public static double CalculateBetaNew(double phi)
        {
            return Math.Atan2((1 - f) * Math.Sin(phi), Math.Cos(phi));
        }
        #endregion
        #region ECEF to ENU
        public static ENU ECEF2ENU(ECEF ecef, LLA refLLA)
        {
            LLA lla = ECEF2LLA(ecef);
            return LLA2ENU(lla, refLLA);
        }

        public static ENU ECEF2ENU(ECEF ecef, ECEF refECEF)
        {
            LLA lla = ECEF2LLA(ecef);
            LLA refLLA = ECEF2LLA(refECEF);
            return LLA2ENU(lla, refLLA);
        }
        #endregion
        #region ECEF to RAE
        public static RAE ECEF2RAE(ECEF ecef, LLA refLLA)
        {
            // Convert ECEF to ENU, then ENU to RAE
            ENU enu = ECEF2ENU(ecef, refLLA);
            return ENU2RAE(enu);
        }

        public static RAE ECEF2RAE(ECEF ecef, ECEF refECEF)
        {
            // Convert ECEF to ENU using ECEF reference, then ENU to RAE
            ENU enu = ECEF2ENU(ecef, refECEF);
            return ENU2RAE(enu);
        }
        #endregion

        #region ENU to LLA
        public static LLA ENU2LLA(ENU enu, LLA refLLA)
        {
            double[] lla = converter.LLAfromENU(enu.E, enu.N, enu.U, refLLA.Lat, refLLA.Lon, refLLA.Alt);
            return new LLA(lla[0], lla[1], lla[2]);
        }
        public static LLA ENU2LLA(ENU enu, ECEF refECEF)
        {
            LLA refLLA = ECEF2LLA(refECEF);
            return ENU2LLA(enu, refLLA);
        }
        #endregion
        #region ENU to ECEF
        public static ECEF ENU2ECEF(ENU enu, LLA refLLA)
        {
            LLA lla = ENU2LLA(enu, refLLA);
            return LLA2ECEF(lla);
        }
        public static ECEF ENU2ECEF(ENU enu, ECEF refECEF)
        {
            LLA lla = ENU2LLA(enu, refECEF);
            return LLA2ECEF(lla);
        }
        #endregion
        #region ENU to RAE
        public static RAE ENU2RAE(ENU enu)
        {
            double r = Math.Sqrt(enu.E * enu.E + enu.N * enu.N + enu.U * enu.U);
            double az = ToDegrees(Math.Atan2(enu.E, enu.N)) % 360;
            double el = ToDegrees(Math.Atan2(enu.U, r));
            return new RAE(r, az, el);
        }
        #endregion

        #region RAE to LLA
        public static LLA RAE2LLA(RAE rae, LLA refLLA)
        {
            ENU enu = RAE2ENU(rae);
            return ENU2LLA(enu, refLLA);
        }

        public static LLA RAE2LLA(RAE rae, ECEF refECEF)
        {
            ENU enu = RAE2ENU(rae);
            return ENU2LLA(enu, refECEF);
        }
        #endregion
        #region RAE to ECEF
        public static ECEF RAE2ECEF(RAE rae, LLA refLLA)
        {
            ENU enu = RAE2ENU(rae);
            return ENU2ECEF(enu, refLLA);
        }

        public static ECEF RAE2ECEF(RAE rae, ECEF refECEF)
        {
            ENU enu = RAE2ENU(rae);
            return ENU2ECEF(enu, refECEF);
        }
        #endregion
        #region RAE to ENU
        public static ENU RAE2ENU(RAE rae)
        {
            double grndRange = rae.R * Math.Cos(rae.el.Radians());
            double e = grndRange * Math.Sin(rae.az.Radians());
            double n = grndRange * Math.Cos(rae.az.Radians());
            double u = rae.R * Math.Sin(rae.el.Radians());
            return new ENU(e, n, u);
        }
        #endregion

        #region Helpers
        public static double ToDegrees(double value)
        {
            return value * 180 / Math.PI;
        }
        #endregion
    }
}