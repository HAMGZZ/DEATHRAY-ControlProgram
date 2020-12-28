using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ControllProgram
{
   public class OrbitCalculator
    {


        public struct AzEl
        {
            public double az;
            public double el;
        };

        public struct cartesianCoords
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
            public kaplarianElements(double a, double nu, double omega, double q, double w, double i, double e, double m)
            {
                this.a = a;
                this.nu = nu;
                this.omega = omega;
                this.q = q;
                this.w = w;
                this.i = i;
                this.e = e;
                this.m = m;
            }
            public double a; // Semi Major Axis (m)
            public double nu; // True anomaly
            public double omega; // Longitude of Ascending Node
            public double q; //Longitude of perihelion
            public double w; //Argument of Perifocus, w (degrees)
            public double i; //Inclanation
            public double e; //Ecentricity
            public double m;
        };


        private Logger logger;
        private double rads = (4 * Math.Atan(1) / 180);
        private double degs = 180 / Math.PI;
        private ObjectDataRecords systemObject, earth;
        private double longitude, latitude;

        public OrbitCalculator(ObjectDataRecords earth, double longitude, double latitude)
        {
            logger = new Logger("CALCULATOR", Logger.Level.DEBUG);
            this.earth = earth;
            this.longitude = longitude;
            this.latitude = latitude;
        }

        public void DirectionFinder(ObjectDataRecords systemObject)
        {
            this.systemObject = systemObject;
            var ObjectElements = new kaplarianElements((double)systemObject.A, (double)systemObject.Nu, (double)systemObject.Omega, (double)systemObject.Q, (double)systemObject.W, (double)systemObject.I, (double)systemObject.E, (double)systemObject.M);
            var EarthElements = new kaplarianElements((double)earth.A, (double)earth.Nu, (double)earth.Omega, (double)earth.Q, (double)earth.W, (double)earth.I, (double)earth.E, (double)earth.M);
            var objectCart = cartPlaneCalc(ObjectElements);
            var earthCart = cartPlaneCalc(EarthElements);
            systemObject.CartX = objectCart.x;
            systemObject.CartY = objectCart.y;
            systemObject.CartZ = objectCart.z;
            double[] geocentricEclip = { objectCart.x - earthCart.x, objectCart.y - earthCart.y, objectCart.z - earthCart.z };
            var raDec = geo2RaDec(geocentricEclip);
            var st = SiderealTime();
            var hourAngle = HourAngle(st, raDec);
            this.systemObject.LST = st;
            this.systemObject.HourAngle = hourAngle;
            this.systemObject.Ra = raDec.Ra;
            this.systemObject.Dec = raDec.Dec;


            var A = (Math.Atan2(Math.Sin(hourAngle * rads), Math.Cos(hourAngle * rads) * Math.Sin(latitude * rads) - Math.Tan(raDec.Dec * rads) * Math.Cos(latitude * rads)) * degs) + 180;
            var h = Math.Asin(Math.Sin(latitude * rads) * Math.Sin(raDec.Dec * rads) + Math.Cos(latitude * rads) * Math.Cos(raDec.Dec * rads) * Math.Cos(hourAngle * rads)) * degs;
            var happ = h + (0.017) / (Math.Tan((h + (10.26 / (h + 5.10))) * rads));
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

        private RaDec geo2RaDec(double[] geo)
        {
            var delta = Math.Sqrt(Math.Pow(geo[0], 2) + Math.Pow(geo[1], 2) + Math.Pow(geo[2], 2));
            systemObject.Distance = delta;
            var lambda = Math.Atan2(geo[1] * rads, geo[0] * rads);
            var beta = Math.Asin(((geo[2]) / (delta)) * rads);
            var epsilon = 23.4397;
            var dec = Math.Asin((Math.Sin(beta) * Math.Cos(epsilon * rads)) + (Math.Cos(beta) * Math.Sin(epsilon * rads) * Math.Sin(lambda))) * degs;
            var ra = Math.Atan2((Math.Sin(lambda) * Math.Cos(epsilon * rads)) - (Math.Tan(beta) * Math.Sin(epsilon * rads)), Math.Cos(lambda)) * degs;
            logger.log(Logger.Level.DEBUG, "Found RA & DEC: " + ra + "  " + dec);
            var temp = new RaDec { Ra = ra, Dec = dec };
            return temp;
        }

        private cartesianCoords cartPlaneCalc(kaplarianElements elements)
        {

            var E = elements.m;

            for (int i = 0; i < 400; i++)
            {
                E = E - ((E - elements.e * Math.Sin(E) - elements.m) / (1 - elements.e * Math.Cos(E)));
            }


            //Calculate radius vector
            var radius = elements.a * ((1 - Math.Pow(elements.e, 2)) / (1 + elements.e * Math.Cos(elements.nu * rads)));


            var x = radius * (Math.Cos(elements.omega * rads) * Math.Cos((elements.w + elements.nu) * rads) - Math.Sin(elements.omega * rads) * Math.Cos(elements.i * rads) * Math.Sin((elements.w + elements.nu) * rads));
            var y = radius * (Math.Sin(elements.omega * rads) * Math.Cos((elements.w + elements.nu) * rads) + Math.Cos(elements.omega * rads) * Math.Cos(elements.i * rads) * Math.Sin((elements.w + elements.nu) * rads));
            var z = radius * Math.Sin(elements.i * rads) * Math.Sin((elements.w + elements.nu) * rads);

            cartesianCoords temp = new cartesianCoords { x = x, y = y, z = z };

            logger.log(Logger.Level.DEBUG, "Found cartesian coordiantes X:" + x + ", Y:" + y + ", Z:" + z);

            return temp;
        }
    }


}
