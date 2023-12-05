
namespace ZippsterStudios.Math
{
    public struct ECEF
    {
        public Distance x;
        public Distance y;
        public Distance z;
        public Vector3 Ecef => new Vector3(X, Y, Z);
        public double X => x.Meters();
        public double Y => y.Meters();
        public double Z => z.Meters();
        public ECEF(double xMeters, double yMeters, double zMeters) : this()
        {
            this.x = new Distance(xMeters, DistanceUnit.Meters);
            this.y = new Distance(yMeters, DistanceUnit.Meters);
            this.z = new Distance(zMeters, DistanceUnit.Meters);
        }
        public override string ToString()
        {
            return $"({X},{Y},{Z})";
        }
    }
    public struct ENU
    {
        public Distance e;
        public Distance n;
        public Distance u;
        public Vector3 Enu => new Vector3(E, N, U);
        public double E => e.Meters();
        public double N => n.Meters();
        public double U => u.Meters();


        public ENU(double eastMeters, double northMeters, double upMeters) : this()
        {
            e = new Distance(eastMeters, DistanceUnit.Meters);
            n = new Distance(northMeters, DistanceUnit.Meters);
            u = new Distance(upMeters, DistanceUnit.Meters);
        }

        public override string ToString()
        {
            return $"({E},{N},{U})";
        }
    }
    public struct RAE
    {
        public Distance r;
        public Angle az;
        public Angle el;
        public Vector3 Rae => new Vector3(R, Az, El);
        public double R => r.Meters();
        public double Az => az.Degrees();
        public double El => el.Degrees();


        public RAE(double rangeMeters, double azimuthDegrees, double elevationDegrees) : this()
        {
            r = new Distance(rangeMeters, DistanceUnit.Meters);
            az = new Angle(azimuthDegrees, AngleUnit.Degrees);
            el = new Angle(elevationDegrees, AngleUnit.Degrees);
        }

        public override string ToString()
        {
            return $"({R},{Az},{El})";
        }
    }
    public struct LLA
    {
        public Angle lat;
        public Angle lon;
        public Distance alt;
        public Vector3 Lla => new Vector3(Lat, Lon, Alt);

        public double Lat => lat.Degrees();
        public double Lon => lon.Degrees();
        public double Alt => alt.Meters();

        public LLA(double latitudeDegrees, double longitudeDegrees, double altitudeMeters) : this()
        {
            lat = new Angle(latitudeDegrees, AngleUnit.Degrees);
            lon = new Angle(longitudeDegrees, AngleUnit.Degrees);
            alt = new Distance(altitudeMeters, DistanceUnit.Meters);
        }

        public override string ToString()
        {
            return $"({Lat},{Lon},{Alt})";
        }
    }
    public struct DMS
    {
        public Angle deg;
        public Angle min;
        public Angle sec;
        public Vector3 Dms => new Vector3(Deg, Min, Sec);
        public double Deg => deg.Degrees();
        public double Min => min.Degrees();
        public double Sec => sec.Degrees();


        public DMS(double degrees, double minutes, double seconds)
        {
            deg = new Angle(degrees, AngleUnit.Degrees);
            min = new Angle(minutes, AngleUnit.Degrees);
            sec = new Angle(seconds, AngleUnit.Degrees);
        }
        public override string ToString()
        {
            return $"{Deg}°{Min}'{Sec}\"";
        }
    }
}