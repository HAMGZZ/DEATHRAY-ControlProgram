using System;
using System.IO.Ports;
using System.Threading;


namespace ControllProgram
{
    public class Program
    {

        public static void Main()
        {
            Console.WriteLine("DeathRay computer prompt  Copyright(C) 2020  Lewis Hamilton\n\rThis program comes with ABSOLUTELY NO WARRANTY; for details type `warranty'.\n\r");
            Console.Write("Test? ");
            var ans = Console.ReadLine();
            if(ans == "y")
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
                            var orbitcalc = new OrbitCalculator(db.data[db.search("EARTH")], 151, -33);
                            orbitcalc.DirectionFinder(currentObject);
                            Console.WriteLine(currentObject.ToString());
                            break;
                        default:
                            break;
                    }
                }
            }



            Console.WriteLine("Available Ports:");
            foreach (string s in SerialPort.GetPortNames())
            {
                Console.WriteLine("   {0}", s);
            }




            var database = new StarDatabase();


            Remote remote = new Remote();
            Term vt4100 = new Term(database, remote);


            Console.CancelKeyPress += delegate
            {
                vt4100.end();
                Environment.Exit(0);
            };

            vt4100.draw();

            Thread.Sleep(100);
            while (true)
            {
                vt4100.update();
                Thread.Sleep(75);
            }
            
        }

    }
}