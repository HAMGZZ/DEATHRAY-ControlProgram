using System;
using System.IO.Ports;
using System.Threading;


namespace ControlProgram
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
                var t = new Test();
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