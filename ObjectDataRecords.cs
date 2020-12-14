using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration.Attributes;

namespace ControllProgram
{
    public class ObjectDataRecords
    {
        // CSV: JDTDB,Calendar Date (TDB),EC,QR,IN,OM,W,Tp,N,MA,TA,A,AD,PR,,name
        private float jDTDB;         // EPOCH DATE
        private DateTime date;       // CALENDAR DATE
        private decimal e;           // Eccentricity, e
        private decimal q;           // Periapsis distance, q (au)
        private decimal i;           // Inclination w.r.t X-Y plane, i (degrees)
        private decimal omega;       // Longitude of Ascending Node, OMEGA, (degrees)
        private decimal w;           // Argument of Perifocus, w (degrees)
        private decimal tp;          // Time of periapsis (Julian Day Number)
        private decimal n;           // Mean motion, n (degrees/day)
        private decimal m;           // Mean anomaly, M (degrees)
        private decimal nu;          // True anomaly, nu (degrees)
        private decimal a;           // Semi-major axis, a (au)
        private decimal aD;          // Apoapsis distance (au)
        private decimal pR;          // Sidereal orbit period (day)
        private string name;         // Human readable name

        [Name("JDTDB")]
        public float JDTDB { get => jDTDB; set => jDTDB = value; }

        [Name("Calendar Date (TDB)")]
        public DateTime Date { get => date; set => date = value; }

        [Name("EC")]
        public string EString { get => e.ToString(); set => Decimal.TryParse(value, System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture, out e); }

        [Name("QR")]
        public string QString { get => q.ToString(); set => Decimal.TryParse(value, System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture, out q); }

        [Name("IN")]
        public string IString { get => i.ToString(); set => Decimal.TryParse(value, System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture, out i); }

        [Name("OM")]
        public string OmegaString { get => omega.ToString(); set => Decimal.TryParse(value, System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture, out omega); }

        [Name("W")]
        public string WString { get => w.ToString(); set => Decimal.TryParse(value, System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture, out w); }

        [Name("Tp")]
        public string TpString { get => tp.ToString(); set => Decimal.TryParse(value, System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture, out tp); }

        [Name("N")]
        public string NString { get => n.ToString(); set => Decimal.TryParse(value, System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture, out n); }

        [Name("MA")]
        public string MString { get => m.ToString(); set => Decimal.TryParse(value, System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture, out m); }

        [Name("TA")]
        public string NuString { get => nu.ToString(); set => Decimal.TryParse(value, System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture, out nu); }

        [Name("A")]
        public string AString { get => a.ToString(); set => Decimal.TryParse(value, System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture, out a); }

        [Name("AD")]
        public string ADString { get => aD.ToString(); set => Decimal.TryParse(value, System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture, out aD); }

        [Name("PR")]
        public string PRString { get => pR.ToString(); set => Decimal.TryParse(value, System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture, out pR); }
        [Name("name")]
        public string Name { get => name; set => name = value; }

        
        public decimal E { get => e; set { } }
        public decimal Q { get => q; set { } }
        public decimal I { get => i; set { } }
        public decimal Omega { get => omega; set { } }
        public decimal W { get => w; set { } }
        public decimal Tp { get => tp; set { } }
        public decimal N { get => n; set { } }
        public decimal M { get => m; set { } }
        public decimal Nu { get => nu; set { } }
        public decimal A { get => a; set { } }
        public decimal AD { get => aD; set { } }
        public decimal PR { get => pR; set { } }

        public double Distance;
        public double Ra;
        public double Dec;
        public double Az;
        public double El;
        public double HourAngle;
        public double LST;
        public double CartX;
        public double CartY;
        public double CartZ;

    }
}
