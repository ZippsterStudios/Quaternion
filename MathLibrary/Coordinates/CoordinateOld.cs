

namespace ZippsterStudios.Old.Coordinate
{
    using System;
    using UnityEngine;

    #region Coordinate Structures
    [Serializable]
    public struct LLA
    {
        public double Latitude;  // in degrees
        public double Longitude; // in degrees
        public double Altitude;  // in meters
        public override string ToString()
        {
            return $"Latitude = {Latitude}, Longitude = {Longitude}, Altitude = {Altitude}";
        }
    }
    [Serializable]
    public struct ECEF
    {
        public double X; // in meters
        public double Y; // in meters
        public double Z; // in meters
        public override string ToString()
        {
            return $"X = {X}, Y = {Y}, Z = {Z}";
        }
    }
    [Serializable]
    public struct RAE
    {
        public double Range;    // in meters
        public double Azimuth;  // in degrees
        public double Elevation; // in degrees
        public override string ToString()
        {
            return $"Range = {Range}, Azimuth = {Azimuth}, Elevation = {Elevation}";
        }
    }
    [Serializable]
    public struct ENU
    {
        public double East;  // in meters
        public double North; // in meters
        public double Up;    // in meters

        public override string ToString()
        {
            return $"East = {East}, North = {North}, Up = {Up}";
        }
    }
    public class NED
    {
        public double North { get; set; }
        public double East { get; set; }
        public double Down { get; set; }
        public override string ToString()
        {
            return $"North = {North}, East = {East}, Down = {Down}";
        }
    }
    [Serializable]
    public struct RPY
    {
        public double Roll;  // in degrees
        public double Pitch; // in degrees
        public double Yaw;   // in degrees
        public override string ToString()
        {
            return $"Roll = {Roll}, Pitch = {Pitch}, Yaw = {Yaw}";
        }
    }
    #endregion
    public class Coordinate
    {
        #region Constants
        // Constants
        private const int N_ROUND_DIGITS = 8; // Single rounding constant for all conversions
        private const double a = 6378137.0; // WGS-84 Earth semimajor axis (m)
        private const double f = 1.0 / 298.257223563; // WGS-84 Flattening
        private const double b = (1.0 - f) * a; // Minor axis
        double e
        {
            get
            {
                return System.Math.Sqrt(1 - (b * b) / (a * a));
            }
        }
        double ep
        {
            get
            {
                return System.Math.Sqrt((a * a) - (b * b)) / b;
            }
        }
        #endregion
        #region LLA, ECEF, Attitude
        public LLA LLAValue { get; private set; }
        public ECEF ECEFValue { get; private set; }
        public RPY Attitude { get; set; } // Added an attitude property, you can set/get this as needed
        #endregion
        #region Constructors and ECEF/LLA conversions
        // Constructors
        public Coordinate(LLA lla)
        {
            LLAValue = lla;
            ECEFValue = LLAToECEF(lla);
        }
        public Coordinate(ECEF ecef)
        {
            ECEFValue = ecef;
            LLAValue = ECEFToLLA(ecef);
        }
        #endregion
        #region Matrix Multiplies
        public Vector3 RotateVectorByRPY(Vector3 v, RPY rpy)
        {
            // Unity's Quaternion.Euler function takes the angles in the order: pitch (x), yaw (y), roll (z).
            Quaternion rotation = Quaternion.Euler((float)rpy.Pitch, (float)rpy.Yaw, (float)rpy.Roll);
            // Rotate the vector using the quaternion
            return rotation * v;
        }
        #endregion
        #region ToENU and ToRAE from Coordinate Input
        public ENU ToENU(Coordinate target)
        {
            // Use this Coordinate's ECEF as the reference
            ECEF refECEF = this.ECEFValue;

            // Convert target's LLA to ECEF
            ECEF targetECEF = target.ECEFValue;

            // Compute difference from reference
            double dx = targetECEF.X - refECEF.X;
            double dy = targetECEF.Y - refECEF.Y;
            double dz = targetECEF.Z - refECEF.Z;

            // Convert this Coordinate's latitude and longitude from degrees to radians
            double latRad = this.LLAValue.Latitude * System.Math.PI / 180.0;
            double lonRad = this.LLAValue.Longitude * System.Math.PI / 180.0;

            // Rotation matrix for ECEF to ENU conversion
            double[,] R = new double[,] {
        {-System.Math.Sin(lonRad), System.Math.Cos(lonRad), 0},
        {-System.Math.Cos(lonRad)*System.Math.Sin(latRad), -System.Math.Sin(lonRad)*System.Math.Sin(latRad), System.Math.Cos(latRad)},
        {System.Math.Cos(lonRad)*System.Math.Cos(latRad), System.Math.Sin(lonRad)*System.Math.Cos(latRad), System.Math.Sin(latRad)}
    };

            // Multiply rotation matrix with difference
            double east = R[0, 0] * dx + R[0, 1] * dy + R[0, 2] * dz;
            double north = R[1, 0] * dx + R[1, 1] * dy + R[1, 2] * dz;
            double up = R[2, 0] * dx + R[2, 1] * dy + R[2, 2] * dz;

            // Convert ENU results to Unity's Vector3 format for easy rotation
            Vector3 enuVector = new Vector3((float)east, (float)north, (float)up);

            // Rotate the ENU vector by the negative of the reference attitude to account for its rotation
            // Use the inverse of the reference attitude's rotation
            RPY inverseRotation = new RPY
            {
                Roll = -this.Attitude.Roll,
                Pitch = -this.Attitude.Pitch,
                Yaw = -this.Attitude.Yaw
            };

            Vector3 rotatedENUVector = RotateVectorByRPY(enuVector, inverseRotation);

            // Return the rotated ENU values
            return new ENU { East = rotatedENUVector.x, North = rotatedENUVector.y, Up = rotatedENUVector.z };
        }
        public RAE ToRAE(Coordinate target)
        {
            // Get the ENU values of the target relative to this Coordinate using ToENU method
            ENU enu = ToENU(target);

            // Compute the range as the magnitude of the ENU vector
            double range = System.Math.Sqrt(enu.East * enu.East + enu.North * enu.North + enu.Up * enu.Up);

            // Compute the azimuth
            double azimuth = System.Math.Atan2(enu.East, enu.North); // Note: east corresponds to the sin component

            // Compute the elevation
            double horizontalDist = System.Math.Sqrt(enu.East * enu.East + enu.North * enu.North);
            double elevation = System.Math.Atan2(enu.Up, horizontalDist);

            // Convert radians to degrees for azimuth and elevation
            azimuth = azimuth * (180.0 / System.Math.PI);
            elevation = elevation * (180.0 / System.Math.PI);

            // Normalize azimuth to [0, 360] range
            if (azimuth < 0)
                azimuth += 360;

            return new RAE { Range = range, Azimuth = azimuth, Elevation = elevation };
        }
        public NED ToNED(Coordinate reference)
        {
            // This is a complex conversion and might require a proper geodetic library.
            // For now, this is a simplified, placeholder logic.
            double dNorth = this.LLAValue.Latitude - reference.LLAValue.Latitude;
            double dEast = this.LLAValue.Longitude - reference.LLAValue.Longitude;
            double dDown = reference.LLAValue.Altitude - this.LLAValue.Altitude;

            return new NED { North = dNorth, East = dEast, Down = dDown };
        }
        public double DistanceTo(Coordinate other)
        {
            // Simple distance calculation using ECEF values
            double dx = this.ECEFValue.X - other.ECEFValue.X;
            double dy = this.ECEFValue.Y - other.ECEFValue.Y;
            double dz = this.ECEFValue.Z - other.ECEFValue.Z;

            return Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }
        public double BearingTo(Coordinate target)
        {
            double latitude1 = DegreesToRadians(this.LLAValue.Latitude);
            double latitude2 = DegreesToRadians(target.LLAValue.Latitude);

            double deltaLongitude = DegreesToRadians(target.LLAValue.Longitude - this.LLAValue.Longitude);

            double x = Math.Atan2(
                Math.Sin(deltaLongitude) * Math.Cos(latitude2),
                Math.Cos(latitude1) * Math.Sin(latitude2) - Math.Sin(latitude1) * Math.Cos(latitude2) * Math.Cos(deltaLongitude)
            );

            // Convert bearing from radians to degrees
            double bearing = RadiansToDegrees(x);

            // Normalize the bearing to a value between 0° and 360°
            return (bearing + 360) % 360;
        }
        #endregion
        #region Create Coordinate point from ENU or RAE
        public Coordinate CreateCoordinateFromRAE(RAE rae)
        {
            ENU enu = RAE2ENU(rae);

            // Convert relative ENU to absolute ECEF
            ECEF refECEF = this.ECEFValue; // The reference point's ECEF
            double latRad = this.LLAValue.Latitude * Math.PI / 180.0;
            double lonRad = this.LLAValue.Longitude * Math.PI / 180.0;

            double deltaX = -enu.East * Math.Sin(lonRad) - enu.North * Math.Sin(latRad) * Math.Cos(lonRad) + enu.Up * Math.Cos(latRad) * Math.Cos(lonRad);
            double deltaY = enu.East * Math.Cos(lonRad) - enu.North * Math.Sin(latRad) * Math.Sin(lonRad) + enu.Up * Math.Cos(latRad) * Math.Sin(lonRad);
            double deltaZ = enu.North * Math.Cos(latRad) + enu.Up * Math.Sin(latRad);

            ECEF targetECEF = new ECEF
            {
                X = refECEF.X + deltaX,
                Y = refECEF.Y + deltaY,
                Z = refECEF.Z + deltaZ
            };

            // Convert ECEF to LLA
            LLA lla = ECEFToLLA(targetECEF);

            return new Coordinate(lla);
        }
        public Coordinate CreateCoordinateFromENU(ENU enu)
        {
            // Step 1: Convert relative ENU to relative ECEF
            ECEF relECEF = ENU2ECEF(enu, this.LLAValue);

            // Step 2: Convert relative ECEF to absolute ECEF
            ECEF absECEF = new ECEF
            {
                X = this.ECEFValue.X + relECEF.X,
                Y = this.ECEFValue.Y + relECEF.Y,
                Z = this.ECEFValue.Z + relECEF.Z
            };

            // Step 3: Convert absolute ECEF to LLA
            LLA lla = ECEFToLLA(absECEF);

            return new Coordinate(lla);
        }
        #endregion
        #region Coordinate Conversions
        public ECEF LLAToECEF(LLA lla)
        {
            ECEF ecef = new ECEF();

            double latRad = System.Math.PI / 180.0 * lla.Latitude;
            double lonRad = System.Math.PI / 180.0 * lla.Longitude;

            double N = a / System.Math.Sqrt(1 - System.Math.Pow(e * System.Math.Sin(latRad), 2));
            ecef.X = System.Math.Round((N + lla.Altitude) * System.Math.Cos(latRad) * System.Math.Cos(lonRad), N_ROUND_DIGITS);
            ecef.Y = System.Math.Round((N + lla.Altitude) * System.Math.Cos(latRad) * System.Math.Sin(lonRad), N_ROUND_DIGITS);
            ecef.Z = System.Math.Round((N * (1 - e * e) + lla.Altitude) * System.Math.Sin(latRad), N_ROUND_DIGITS);

            return ecef;
        }
        public LLA ECEFToLLA(ECEF ecef)
        {
            LLA lla = new LLA();

            double p = System.Math.Sqrt(ecef.X * ecef.X + ecef.Y * ecef.Y);
            double theta = System.Math.Atan2(ecef.Z * a, p * b);

            double lonRad = System.Math.Atan2(ecef.Y, ecef.X);
            double latRad = System.Math.Atan2(
                ecef.Z + ep * ep * b * System.Math.Pow(System.Math.Sin(theta), 3),
                p - e * e * a * System.Math.Pow(System.Math.Cos(theta), 3)
            );

            double N = a / System.Math.Sqrt(1 - e * e * System.Math.Sin(latRad) * System.Math.Sin(latRad));
            double alt = p / System.Math.Cos(latRad) - N;

            lla.Latitude = System.Math.Round(latRad * 180.0 / System.Math.PI, N_ROUND_DIGITS);
            lla.Longitude = System.Math.Round(lonRad * 180.0 / System.Math.PI, N_ROUND_DIGITS);
            lla.Altitude = System.Math.Round(alt, N_ROUND_DIGITS);

            return lla;
        }
        public ENU RAE2ENU(RAE rae)
        {
            // Convert RAE to ENU
            double R = rae.Range;
            double Az = rae.Azimuth * Math.PI / 180.0; // Convert to radians
            double El = rae.Elevation * Math.PI / 180.0; // Convert to radians

            double E = R * Math.Sin(El) * Math.Sin(Az);
            double N = R * Math.Sin(El) * Math.Cos(Az);
            double U = R * Math.Cos(El);

            ENU enu = new ENU { East = E, North = N, Up = U };
            return enu;
        }
        public RAE ENU2RAE(ENU enu)
        {
            double R = Math.Sqrt(enu.East * enu.East + enu.North * enu.North + enu.Up * enu.Up);
            double Az = Math.Atan2(enu.East, enu.North); // in radians
            double El = Math.Asin(enu.Up / R); // in radians

            // Convert azimuth and elevation from radians to degrees
            Az = Az * 180.0 / Math.PI;
            El = El * 180.0 / Math.PI;

            return new RAE { Range = R, Azimuth = Az, Elevation = El };
        }
        public ECEF ENU2ECEF(ENU enu, LLA refLLA)
        {
            // Convert the reference LLA's latitude and longitude from degrees to radians
            double latRad = refLLA.Latitude * Math.PI / 180.0;
            double lonRad = refLLA.Longitude * Math.PI / 180.0;

            // Rotation matrix for ENU to ECEF conversion
            double[,] R = new double[,] {
        {-Math.Sin(lonRad), -Math.Cos(lonRad)*Math.Sin(latRad), Math.Cos(lonRad)*Math.Cos(latRad)},
        {Math.Cos(lonRad), -Math.Sin(lonRad)*Math.Sin(latRad), Math.Sin(lonRad)*Math.Cos(latRad)},
        {0, Math.Cos(latRad), Math.Sin(latRad)}
    };

            // Multiply rotation matrix with ENU values
            double x = R[0, 0] * enu.East + R[0, 1] * enu.North + R[0, 2] * enu.Up;
            double y = R[1, 0] * enu.East + R[1, 1] * enu.North + R[1, 2] * enu.Up;
            double z = R[2, 0] * enu.East + R[2, 1] * enu.North + R[2, 2] * enu.Up;

            return new ECEF { X = x, Y = y, Z = z };
        }
        #endregion
        #region Validity Checks
        public bool IsValidLLA()
        {
            return
                this.LLAValue.Latitude >= -90 && this.LLAValue.Latitude <= 90 &&
                this.LLAValue.Longitude >= -180 && this.LLAValue.Longitude <= 180 &&
                this.LLAValue.Altitude >= -11000 && this.LLAValue.Altitude <= 85000; // Earth's depth & altitude range
        }
        public bool IsValidECEF()
        {
            // You might need to set a range for valid ECEF values.
            // For now, this is a basic check.
            double earthRadius = 6371000; // Approx radius in meters
            double distanceFromOrigin = Math.Sqrt(ECEFValue.X * ECEFValue.X + ECEFValue.Y * ECEFValue.Y + ECEFValue.Z * ECEFValue.Z);

            return distanceFromOrigin <= 2 * earthRadius; // Just an arbitrary check for now
        }
        #endregion
        #region Deg and Rad conversions
        private double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }

        private double RadiansToDegrees(double radians)
        {
            return radians * 180.0 / Math.PI;
        }
        #endregion
    }
}