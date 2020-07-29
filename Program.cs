using System;
using System.Linq;

namespace MoonPhaseConsole
{
    partial class Program
    {
        static void Main(string[] args)
        {
            var dates = new[] {
                new DateTime(2020, 7, 27), new DateTime(2020, 7, 28),
                new DateTime(2020, 8, 3), new DateTime(2020, 8, 11),
                new DateTime(2020, 8, 19),
            };

            var results =
                dates
                    .Select(x => Moon.Calculate(x))
                    .Select((r, i) => $"{r.Emoji} {r.Name} ({r.DaysIntoCycle} days, Visibility {Math.Round(r.Visibility, 2)}%)\n")
                    .Aggregate((a, v) => a + v);
            
            Console.WriteLine();
            Console.WriteLine(results);
            
            var result = Moon.Now();
            Console.WriteLine(result);
        }
    }
}
