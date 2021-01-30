using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlProgram
{
    class Test
    {
        public Test()
        {
            var db = new StarDatabase();


            ObjectDataRecords currentObject;
            while (true)
            {
                Console.Write("Enter command: ");
                var res = Console.ReadLine();
                switch (res)
                {
                    case "loaddb":
                        db.load(null);
                        break;
                    case "find":
                        Console.Write("Enter planet name: ");
                        res = Console.ReadLine();
                        var planet = db.search(res.ToUpper());
                        Console.WriteLine("Found planet at: {0}", planet);
                        break;
                    case "calc":
                        Console.Write("Enter object name: ");
                        res = Console.ReadLine();
                        try
                        {
                            currentObject = db.data[db.search(res)];
                        }
                        catch (Exception)
                        {
                            currentObject = null;
                            break;
                        }
                        currentObject.Calculations = OrbitCalculator.DirectionFinder(currentObject, db.data[db.search("EARTH")], 151, -33);
                        Console.WriteLine(currentObject.ToString());
                        Console.WriteLine("AZ EL >> ", +currentObject.Calculations.Az + " " + currentObject.Calculations.El);
                        break;
                    case "remote simple":
                        Remote remoteTemp = new Remote();
                        Console.WriteLine("Current read from Deathbox: ");
                        while (true)
                        {

                            string s = string.Format("CAz: {0}\n\rCEl: {1}\n\rDAz: {2}\n\rDEl: {3}\n\rSAz: {4}\n\rSEl: {5}\n\rMoving? {6}\n\rVolts: {7}\n\rLights: {8}\n\rConnect: {9}\n\r",
                                    remoteTemp.data.rxCurrentAzEl.Az.ToString(),
                                    remoteTemp.data.rxCurrentAzEl.El.ToString(),
                                    remoteTemp.data.rxDesiredAzEl.Az.ToString(),
                                    remoteTemp.data.rxDesiredAzEl.El.ToString(),
                                    remoteTemp.data.txAzEl.Az.ToString(),
                                    remoteTemp.data.txAzEl.El.ToString(),
                                    remoteTemp.data.moving.ToString(),
                                    remoteTemp.data.volts.ToString(),
                                    remoteTemp.data.lights.ToString(),
                                    remoteTemp.data.connect.ToString());
                            Console.WriteLine(s);
                        }
                        break;
                    default:
                        Console.WriteLine("Unknown command: res");
                        break;
                }
            }
        }
    }
}
