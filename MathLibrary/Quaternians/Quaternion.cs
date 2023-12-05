namespace ZippsterStudios.Math
{
    using System;

    public class Quaternion
    {
        #region Properties
        public double W { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        #endregion
        #region Constructors
        public Quaternion() : this(1, 0, 0, 0) { }
        public Quaternion(double w, double x, double y, double z)
        {
            W = w;
            X = x;
            Y = y;
            Z = z;
        }
        public Quaternion(double roll, double pitch, double yaw)
        {
            roll = DegreesToRadians(roll);
            pitch = DegreesToRadians(pitch);
            yaw = DegreesToRadians(yaw);

            double cy = Math.Cos(yaw * 0.5);
            double sy = Math.Sin(yaw * 0.5);
            double cp = Math.Cos(pitch * 0.5);
            double sp = Math.Sin(pitch * 0.5);
            double cr = Math.Cos(roll * 0.5);
            double sr = Math.Sin(roll * 0.5);

            W = cr * cp * cy + sr * sp * sy;
            X = sr * cp * cy - cr * sp * sy;
            Y = cr * sp * cy + sr * cp * sy;
            Z = cr * cp * sy - sr * sp * cy;
        }
        #endregion
        #region Degree/Radian Conversions
        private static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }
        private static double RadiansToDegrees(double radians)
        {
            return radians * 180.0 / Math.PI;
        }
        #endregion
        #region Norm and Normalization
        // Norm (magnitude) of the quaternion
        public double Norm()
        {
            return Math.Sqrt(W * W + X * X + Y * Y + Z * Z);
        }
        public Quaternion Normalize()
        {
            double norm = Norm();
            return new Quaternion(W / norm, X / norm, Y / norm, Z / norm);
        }
        #endregion
        #region Conjugate and Inverse
        public Quaternion Conjugate()
        {
            return new Quaternion(W, -X, -Y, -Z);
        }
        public Quaternion Inverse()
        {
            double normSquared = Norm() * Norm();
            Quaternion conjugate = Conjugate();
            return new Quaternion(conjugate.W / normSquared, conjugate.X / normSquared, conjugate.Y / normSquared, conjugate.Z / normSquared);
        }
        #endregion
        #region Operators
        public static Quaternion operator *(Quaternion a, Quaternion b)
        {
            return new Quaternion(
                a.W * b.W - a.X * b.X - a.Y * b.Y - a.Z * b.Z,
                a.W * b.X + a.X * b.W + a.Y * b.Z - a.Z * b.Y,
                a.W * b.Y - a.X * b.Z + a.Y * b.W + a.Z * b.X,
                a.W * b.Z + a.X * b.Y - a.Y * b.X + a.Z * b.W
            );
        }
        #endregion
        #region Rotation Methods
        public static Quaternion LookAt(Vector3 position, Vector3 target, Vector3 worldUp)
        {
            Vector3 forward = (target - position).Normalize();
            return LookTowards(forward, worldUp);
        }
        public static Quaternion LookTowards(Vector3 forward, Vector3 up)
        {
            Vector3 forwardNorm = forward.Normalize();
            Vector3 right = Vector3.Cross(up, forwardNorm).Normalize();
            Vector3 upNorm = Vector3.Cross(forwardNorm, right);

            double m00 = right.X;
            double m01 = right.Y;
            double m02 = right.Z;
            double m10 = upNorm.X;
            double m11 = upNorm.Y;
            double m12 = upNorm.Z;
            double m20 = forwardNorm.X;
            double m21 = forwardNorm.Y;
            double m22 = forwardNorm.Z;

            double num8 = (m00 + m11) + m22;
            Quaternion quaternion = new Quaternion();
            if (num8 > 0f)
            {
                var num = Math.Sqrt(num8 + 1f);
                quaternion.W = num * 0.5f;
                num = 0.5f / num;
                quaternion.X = (m12 - m21) * num;
                quaternion.Y = (m20 - m02) * num;
                quaternion.Z = (m01 - m10) * num;
                return quaternion;
            }
            if ((m00 >= m11) && (m00 >= m22))
            {
                var num7 = Math.Sqrt(((1f + m00) - m11) - m22);
                var num4 = 0.5f / num7;
                quaternion.X = 0.5f * num7;
                quaternion.Y = (m01 + m10) * num4;
                quaternion.Z = (m02 + m20) * num4;
                quaternion.W = (m12 - m21) * num4;
                return quaternion;
            }
            if (m11 > m22)
            {
                var num6 = Math.Sqrt(((1f + m11) - m00) - m22);
                var num3 = 0.5f / num6;
                quaternion.X = (m10 + m01) * num3;
                quaternion.Y = 0.5f * num6;
                quaternion.Z = (m21 + m12) * num3;
                quaternion.W = (m20 - m02) * num3;
                return quaternion;
            }
            var num5 = Math.Sqrt(((1f + m22) - m00) - m11);
            var num2 = 0.5f / num5;
            quaternion.X = (m20 + m02) * num2;
            quaternion.Y = (m21 + m12) * num2;
            quaternion.Z = 0.5f * num5;
            quaternion.W = (m01 - m10) * num2;
            return quaternion;
        }
        #endregion
        #region Interpolation
        public static Quaternion Lerp(Quaternion a, Quaternion b, double t)   // Linear Interpolation (Lerp)
        {
            t = Math.Clamp(t, 0.0, 1.0);
            return new Quaternion(
                a.W + (b.W - a.W) * t,
                a.X + (b.X - a.X) * t,
                a.Y + (b.Y - a.Y) * t,
                a.Z + (b.Z - a.Z) * t
            ).Normalize();
        }
        #endregion
        #region ToEulerAngles and ToString
        public (double Roll, double Pitch, double Yaw) ToEulerAngles()
        {
            // Roll (X-axis rotation)
            double sinr_cosp = 2 * (W * X + Y * Z);
            double cosr_cosp = 1 - 2 * (X * X + Y * Y);
            double roll = Math.Atan2(sinr_cosp, cosr_cosp);

            // Pitch (Y-axis rotation)
            double sinp = 2 * (W * Y - Z * X);
            double pitch;
            if (Math.Abs(sinp) >= 1)
                pitch = Math.PI / 2 * Math.Sign(sinp); // Use 90 degrees with the sign of sinp
            else
                pitch = Math.Asin(sinp);

            // Yaw (Z-axis rotation)
            double siny_cosp = 2 * (W * Z + X * Y);
            double cosy_cosp = 1 - 2 * (Y * Y + Z * Z);
            double yaw = Math.Atan2(siny_cosp, cosy_cosp);

            return (RadiansToDegrees(roll), RadiansToDegrees(pitch), RadiansToDegrees(yaw));
        }
        public override string ToString()  // ToString for easy debugging
        {
            return $"({W}, {X}, {Y}, {Z})";
        }
        #endregion
    }
}
