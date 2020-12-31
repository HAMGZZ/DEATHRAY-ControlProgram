using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;

namespace ControlProgram
{
    class objectDataRecordsMap : ClassMap<ObjectDataRecords>
    {
        public objectDataRecordsMap()
        {
            AutoMap(CultureInfo.InvariantCulture);
            Map(member => member.E).Ignore();
            Map(member => member.Q).Ignore();
            Map(member => member.I).Ignore();
            Map(member => member.Omega).Ignore();
            Map(member => member.W).Ignore();
            Map(member => member.Tp).Ignore();
            Map(member => member.N).Ignore();
            Map(member => member.M).Ignore();
            Map(member => member.Nu).Ignore();
            Map(member => member.A).Ignore();
            Map(member => member.AD).Ignore();
            Map(member => member.PR).Ignore();
            Map(member => member.Distance).Ignore();
            Map(member => member.Az).Ignore();
            Map(member => member.El).Ignore();
            Map(member => member.Ra).Ignore();
            Map(member => member.Dec).Ignore();
            Map(member => member.HourAngle).Ignore();
            Map(member => member.LST).Ignore();
            Map(member => member.CartX).Ignore();
            Map(member => member.CartY).Ignore();
            Map(member => member.CartZ).Ignore();
        }
    }
    class StarDatabase
    {
        public List<ObjectDataRecords> data = new List<ObjectDataRecords>();
        private int totalCount;
        private int currentCount = 0;
        private Term term;
        private Logger logger;
        public StarDatabase()
        {
            logger = new Logger("DATABASE", Logger.Level.INFO);
        }

        public void load(Term term)
        {
            this.term = term;
            logger.log(Logger.Level.INFO, "Loading all planet data...");
            string topPath = "./Horizons/data/";
            if(term != null)
            {
                totalCount = Directory.GetFiles(topPath, "*.csv", SearchOption.AllDirectories).Length;
                Thread barGraphThread = new Thread(barGraphWork);
                barGraphThread.Start();
                barGraphThread.Join();
            }
            else
            {
                processDir(topPath);
            }
        }

        public int search(string name)
        {
            logger.log(Logger.Level.INFO, "Searching for " + name);
            int loc = data.FindIndex(x => (x.Name.ToUpper() == name.ToUpper() && x.Date >= DateTime.UtcNow));
            if(loc < 0)
                logger.log(Logger.Level.INFO, "Could not find " + name);
            else
                logger.log(Logger.Level.INFO, "Found " + name + " at " + loc);
            return loc;
        }

        private void barGraphWork()
        {
            while (currentCount < totalCount)
            {
                term.barGraph(new Term.CursorAddress { x = 0, y = 2 }, new Term.CursorAddress { x = 0, y = 2 }, 80, "#", currentCount, totalCount);
                Thread.Sleep(80);
            }
        }

        private void processDir(string Path)
        {
            string[] fileEntries = Directory.GetFiles(Path);
            foreach (var file in fileEntries)
            {
                if (file.EndsWith(".csv"))
                {
                    logger.log(Logger.Level.INFO, "Found File \"" + file + "\" ");
                    processFile(file);
                    currentCount++;
                }
            }
            string[] subDirectoryEntries = Directory.GetDirectories(Path);
            foreach (var dir in subDirectoryEntries)
                processDir(dir);

        }

        private void processFile(string filePath)
        {
            DateTime todayDate = DateTime.UtcNow.Subtract(TimeSpan.FromHours(12));
            DateTime futureDate = DateTime.UtcNow.AddDays(5);
            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Configuration.RegisterClassMap<objectDataRecordsMap>();
                try
                {

                    var record = csv.GetRecords<ObjectDataRecords>();

                    foreach (var row in record)
                    {
                        if (row.Date >= todayDate && row.Date <= futureDate)
                            data.Add(row);
                        else
                            logger.log(Logger.Level.DEBUG, "Too far in advance to load...");
                    }
                    logger.log(Logger.Level.DEBUG, "Stored " + record);
                }
                catch (Exception ex)
                {
                    logger.log(Logger.Level.WARNING, ex.ToString());
                }
            }
        }
    }
}
