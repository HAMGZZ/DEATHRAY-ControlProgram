using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static System.Math;

namespace ControlProgram
{
   public class OrbitCalculator
    {


        public struct AzEl
        {
            public double az;
            public double el;
        };

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


        private Logger logger;
        private const double rads = PI / 180;
        private const double degs = 180 / PI;
        private ObjectDataRecords systemObject, earth;
        private double longitude, latitude;

        public OrbitCalculator(ObjectDataRecords earth, double longitude, double latitude)
        {
            logger = new Logger("CALCULATOR", Logger.Level.DEBUG);
            this.earth = earth;
            this.longitude = longitude * rads;
            this.latitude = latitude * rads;
        }

        public void DirectionFinder(ObjectDataRecords systemObject)
        {
            this.systemObject = systemObject;
            var objectCart = cartPlaneCalc(new kaplarianElements(systemObject));
            var earthCart = cartPlaneCalc(new kaplarianElements(earth));
            systemObject.CartX = objectCart.x;
            systemObject.CartY = objectCart.y;
            systemObject.CartZ = objectCart.z;
            var geocentricEclip = new vector3D { x = objectCart.x - earthCart.x, y = objectCart.y - earthCart.y, z = objectCart.z - earthCart.z };
            var raDec = geo2RaDec(geocentricEclip);
            var st = SiderealTime();
            var hourAngle = HourAngle(st, raDec);
            this.systemObject.LST = st;
            this.systemObject.HourAngle = hourAngle;
            this.systemObject.Ra = raDec.Ra;
            this.systemObject.Dec = raDec.Dec;


            var A = (Atan2(Sin(hourAngle ), Cos(hourAngle ) * Sin(latitude ) - Tan(raDec.Dec ) * Cos(latitude )) * degs) + 180;
            var h = Asin(Sin(latitude ) * Sin(raDec.Dec ) + Cos(latitude ) * Cos(raDec.Dec ) * Cos(hourAngle )) * degs;
            var happ = h + (0.017) / (Tan((h + (10.26 / (h + 5.10))) ));
            this.systemObject.Az = A;
            this.systemObject.El = happ;
        }

        private double HourAngle(double st, RaDec raDec)
        {
            var H = st - raDec.Ra;
            return H;
        }

        private double SiderealTime()
        {
            double II = (double)(earth.Omega + earth.W);
            double M = (double)earth.M;
            var t = new TimeSpan(0, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Millisecond);
            var tz = TimeZoneInfo.Local.GetUtcOffset(DateTime.Now);
            double merSt = (M + II + 15 * ((double)(t.TotalHours - tz.TotalHours))) % 360;
            double st = (merSt + (longitude)) % 360;
            logger.log(Logger.Level.DEBUG, "Found Sidereal Time at:" + st);
            return st;
        }

        private RaDec geo2RaDec(vector3D geo)
        {
            var delta = Sqrt(geo.x*geo.x + geo.y*geo.y + geo.z*geo.z);
            systemObject.Distance = delta;
            var lambda = Atan2(geo.y * rads, geo.x * rads);
            var beta = Asin(((geo.z) / (delta)) * rads);
            const double epsilon = 23.4397 * rads;
            var dec = Asin((Sin(beta) * Cos(epsilon)) + (Cos(beta) * Sin(epsilon) * Sin(lambda)));
            var ra = Atan2((Sin(lambda) * Cos(epsilon)) - (Tan(beta) * Sin(epsilon)), Cos(lambda));
            logger.log(Logger.Level.DEBUG, "Found RA & DEC: " + ra*degs + "  " + dec*degs);
            return new RaDec { Ra = ra, Dec = dec };
        }

        private vector3D cartPlaneCalc(kaplarianElements elements)
        {

            var m = elements.m;

            for (int i = 0; i < 400; i++)
            {
                m = m - ((m - elements.e * Sin(m) - m) / (1 - elements.e * Cos(m)));
            }


            //Calculate radius vector
            var radius = elements.a * ((1 - (elements.e*elements.e)) / (1 + elements.e * Cos(elements.nu * rads)));


            var x = radius * (Cos(elements.omega * rads) * Cos((elements.w + elements.nu) * rads) - Sin(elements.omega * rads) * Cos(elements.i * rads) * Sin((elements.w + elements.nu) * rads));
            var y = radius * (Sin(elements.omega * rads) * Cos((elements.w + elements.nu) * rads) + Cos(elements.omega * rads) * Cos(elements.i * rads) * Sin((elements.w + elements.nu) * rads));
            var z = radius * Sin(elements.i * rads) * Sin((elements.w + elements.nu) * rads);

            vector3D temp = new vector3D { x = x, y = y, z = z };

            logger.log(Logger.Level.DEBUG, "Found Cartesian coordinates X:" + x + ", Y:" + y + ", Z:" + z);

            return temp;
        }
    }


}
