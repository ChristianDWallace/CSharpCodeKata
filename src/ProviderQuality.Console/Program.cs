using System;
using System.Collections.Generic;
using System.Linq;

namespace ProviderQuality.Console
{
    public class Program
    {
        public IList<Award> Awards
        {
            get;
            set;
        }

        static void Main(string[] args)
        {
            System.Console.WriteLine("Updating award metrics...!");

            var app = new Program()
            {
                Awards = new List<Award>
                {
                    new Award(Award.AwardType.Gov_Quality_Plus),
                    new Award(Award.AwardType.Blue_First),
                    new Award(Award.AwardType.Acme_Partner_Facility),
                    new Award(Award.AwardType.Blue_Distinction_Plus),
                    new Award(Award.AwardType.Blue_Compare),
                    new Award(Award.AwardType.Top_Connected_Providers),
                    new Award(Award.AwardType.Blue_Star)
                }

            };

            string keepGoing = "";

            while(keepGoing != "q")
            {
                app.UpdateQuality();
                System.Console.WriteLine(Environment.NewLine + "Keep Going? Press q to stop program.");
                keepGoing = System.Console.ReadLine().ToLower(); 
            }
            


        }

        public void UpdateQuality()
        {
            foreach(var award in Awards)
            {
                award.UpdateQuality(); 
            }
            //Sort the awards by quality value. 
            Awards = Awards.OrderByDescending(a => a.Quality).ToList();

            PrintAwardsList(); 
        }

        private void PrintAwardsList()
        {
            System.Console.Clear(); 
            for(int i = 0; i < Awards.Count; i++)
            {
                string printString = (i+1) + ". Name: " + Awards[i].Name + " | Quality: " + Awards[i].Quality + " | Expires In: " + Awards[i].ExpiresIn;
                System.Console.WriteLine(printString);
            }
        }

    }

}
