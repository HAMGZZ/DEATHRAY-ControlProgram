using System;

using static System.Math;

namespace ControlProgram
{
    public static class OrbitCalculator
    {
        public struct vector3D
        {
            public double x;
            public double y;
            public double z;
        };

        public struct RaDec
        {
            public double Ra;
            public double Dec;
            public double Distance;
        };

        public struct kaplarianElements
        {
            public kaplarianElements(ObjectDataRecords obj)
            {
                this.a = (double)obj.A;
                this.nu = (double)obj.Nu * rads;
                this.omega = (double)obj.Omega * rads;
                this.q = (double)obj.Q * rads;
                this.w = (double)obj.W * rads;
                this.i = (double)obj.I * rads;
                this.e = (double)obj.E;
                this.m = (double)obj.M * rads;
            }

            public double a; // Semi Major Axis (m)
            public double nu; // True anomaly
            public double omega; // Longitude of Ascending Node
            public double q; //Longitude of perihelion
            public double w; //Argument of Perifocus, w (degrees)
            public double i; //Inclanation
            public double e; //Ecentricity
            public double m; // Mean Anomaly
        };

        private static Logger logger = new Logger("CALCULATOR", Logger.Level.DEBUG);
        private const double rads = PI / 180;
        private const double degs = 180 / PI;

        public static ObjectDataCalculations DirectionFinder(ObjectDataRecords systemObject, ObjectDataRecords earth, double longitude, double latitude)
        {
            longitude = longitude * rads;
            latitude = latitude * rads;
            var objectCart = cartPlaneCalc(new kaplarianElements(systemObject));
            var earthCart = cartPlaneCalc(new kaplarianElements(earth));
            systemObject.Calculations.CartX = objectCart.x;
            systemObject.Calculations.CartY = objectCart.y;
            systemObject.Calculations.CartZ = objectCart.z;
            var geocentricEclip = new vector3D { x = objectCart.x - earthCart.x, y = objectCart.y - earthCart.y, z = objectCart.z - earthCart.z };
            var raDec = geo2RaDec(geocentricEclip);
            var st = SiderealTime(new kaplarianElements(earth), longitude);
            var hourAngle = HourAngle(st, raDec);

            var A = (Atan2(Sin(hourAngle), Cos(hourAngle) * Sin(latitude) - Tan(raDec.Dec) * Cos(latitude)) * degs) + 180;
            var h = Asin(Sin(latitude) * Sin(raDec.Dec) + Cos(latitude) * Cos(raDec.Dec) * Cos(hourAngle)) * degs;

            return new ObjectDataCalculations { Az = A, El = h, Ra = raDec.Ra, Dec = raDec.Dec, HourAngle = hourAngle, LST = st, Distance = raDec.Distance };
        }

        private static double HourAngle(double st, RaDec raDec)
        {
            var H = st - raDec.Ra;
            return H * rads;
        }

        private static double SiderealTime(kaplarianElements earth, double longitude)
        {
            double II = (double)(earth.omega * degs + earth.w * degs);
            double M = (double)earth.m *degs;
            var t = new TimeSpan(0, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Millisecond);
            var tz = TimeZoneInfo.Local.GetUtcOffset(DateTime.Now);
            double merSt = (M + II + 15 * ((double)(t.TotalHours - tz.TotalHours))) % 360;
            double st = (merSt + (longitude * degs)) % 360;
            logger.log(Logger.Level.DEBUG, "Found Sidereal Time at:" + st);
            return st;
        }

        private static RaDec geo2RaDec(vector3D geo)
        {
            var delta = Sqrt(geo.x * geo.x + geo.y * geo.y + geo.z * geo.z);
            var lambda = Atan2(geo.y * rads, geo.x * rads);
            var beta = Asin(((geo.z) / (delta)) * rads);
            const double epsilon = 23.4397 * rads;
            var dec = Asin((Sin(beta) * Cos(epsilon)) + (Cos(beta) * Sin(epsilon) * Sin(lambda)));
            var ra = Atan2((Sin(lambda) * Cos(epsilon)) - (Tan(beta) * Sin(epsilon)), Cos(lambda));
            logger.log(Logger.Level.DEBUG, "Found RA & DEC: " + ra * degs + "  " + dec * degs);
            return new RaDec { Ra = ra, Dec = dec, Distance = delta };
        }

        private static vector3D cartPlaneCalc(kaplarianElements elements)
        {

            var E = elements.m;

            for (int i = 0; i < 400; i++)
            {
                m = m - ((m - elements.e * Sin(m) - m) / (1 - elements.e * Cos(m)));
            }

            //Calculate radius vector
            var radius = elements.a * ((1 - (elements.e * elements.e)) / (1 + elements.e * Cos(elements.nu)));

            var x = radius * (Cos(elements.omega ) * Cos((elements.w + elements.nu) ) - Sin(elements.omega ) * Cos(elements.i ) * Sin((elements.w + elements.nu) ));
            var y = radius * (Sin(elements.omega ) * Cos((elements.w + elements.nu) ) + Cos(elements.omega ) * Cos(elements.i ) * Sin((elements.w + elements.nu) ));
            var z = radius * Sin(elements.i ) * Sin((elements.w + elements.nu) );

            logger.log(Logger.Level.DEBUG, "Found Cartesian coordinates X:" + x + ", Y:" + y + ", Z:" + z);

            return new vector3D { x = x, y = y, z = z };
        }
    }
}