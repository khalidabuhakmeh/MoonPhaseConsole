using System;
using System.Collections.Generic;
using System.Linq;

namespace MoonPhaseConsole
{
    public static class Moon
    {
        private static readonly IReadOnlyList<string> NorthernHemisphere
            = new List<string> {"🌑", "🌒", "🌓", "🌔", "🌕", "🌖", "🌗", "🌘", "🌑"};

        private static readonly IReadOnlyList<string> SouthernHemisphere
            = NorthernHemisphere.Reverse().ToList();

        private static readonly List<string> Names = new List<string>
        {
            Phase.NewMoon,
            Phase.WaxingCrescent, Phase.FirstQuarter, Phase.WaxingGibbous,
            Phase.FullMoon,
            Phase.WaningGibbous, Phase.ThirdQuarter, Phase.WaningCrescent
        };

        private const double TotalLengthOfCycle = 29.53;
        
        private static readonly List<Phase> allPhases = new List<Phase>();

        static Moon()
        {
            var period = TotalLengthOfCycle / Names.Count;
            // divide the phases into equal parts 
            // making sure there are no gaps
            allPhases = Names
                .Select((t, i) => new Phase(t, period * i, period * (i + 1)))
                .ToList();
        }

        /// <summary>
        /// Calculate the current phase of the moon.
        /// Note: this calculation uses the last recorded new moon to calculate the cycles of
        /// of the moon since then. Any date in the past before 1920 might not work.
        /// </summary>
        /// <param name="utcDateTime"></param>
        /// <remarks>https://www.subsystems.us/uploads/9/8/9/4/98948044/moonphase.pdf</remarks>
        /// <returns></returns>
        public static PhaseResult Calculate(DateTime utcDateTime,
            Earth.Hemispheres viewFromEarth = Earth.Hemispheres.Northern)
        {
            const double julianConstant = 2415018.5;
            var julianDate = utcDateTime.ToOADate() + julianConstant;

            // London New Moon (1920)
            // https://www.timeanddate.com/moon/phases/uk/london?year=1920
            var daysSinceLastNewMoon =
                new DateTime(1920, 1, 21, 5, 25, 00, DateTimeKind.Utc).ToOADate() + julianConstant;

            var newMoons = (julianDate - daysSinceLastNewMoon) / TotalLengthOfCycle;
            var intoCycle = (newMoons - Math.Truncate(newMoons)) * TotalLengthOfCycle;

            var phase =
                allPhases.First(p => intoCycle >= p.Start && intoCycle <= p.End);

            var index = allPhases.IndexOf(phase);
            var currentPhase =
                viewFromEarth switch
                {
                    Earth.Hemispheres.Northern => NorthernHemisphere[index],
                    _ => SouthernHemisphere[index]
                };

            return new PhaseResult
            (
                phase.Name,
                currentPhase,
                Math.Round(intoCycle, 2),
                viewFromEarth,
                utcDateTime
            );
        }

        public static PhaseResult UtcNow(Earth.Hemispheres viewFromEarth = Earth.Hemispheres.Northern)
        {
            return Calculate(DateTime.UtcNow, viewFromEarth);
        }

        public static PhaseResult Now(Earth.Hemispheres viewFromEarth = Earth.Hemispheres.Northern)
        {
            return Calculate(DateTime.Now.ToUniversalTime(), viewFromEarth);
        }

        public class PhaseResult
        {
            public PhaseResult(string name, string emoji, double daysIntoCycle, Earth.Hemispheres hemisphere,
                DateTime moment)
            {
                Name = name;
                Emoji = emoji;
                DaysIntoCycle = daysIntoCycle;
                Hemisphere = hemisphere;
                Moment = moment;
            }

            public string Name { get; }
            public string Emoji { get; set; }
            public double DaysIntoCycle { get; set; }
            public Earth.Hemispheres Hemisphere { get; set; }
            public DateTime Moment { get; }
            public double Visibility  
            {
                get
                {
                    const int FullMoon = 15;
                    const double halfCycle = TotalLengthOfCycle / 2;
                
                    var numerator = DaysIntoCycle > FullMoon
                        // past the full moon, we want to count down
                        ? halfCycle - (DaysIntoCycle % halfCycle)
                        // leading up to the full moon
                        : DaysIntoCycle;

                    return numerator / halfCycle * 100;
                }
            }

            public override string ToString()
            {
                var percent = Math.Round(Visibility , 2);
                return $"The Moon for {Moment} is {DaysIntoCycle} days\n" +
                       $"into the cycle, and is showing as \"{Name}\"\n" +
                       $"with {percent}% visibility, and a face of {Emoji} from the {Hemisphere.ToString().ToLowerInvariant()} hemisphere.";
            }
        }

        public class Phase
        {
            public const string NewMoon = "New Moon";
            public const string WaxingCrescent = "Waxing Crescent";
            public const string FirstQuarter = "First Quarter";
            public const string WaxingGibbous = "Waxing Gibbous";
            public const string FullMoon = "Full Moon";
            public const string WaningGibbous = "Waning Gibbous";
            public const string ThirdQuarter = "Third Quarter";
            public const string WaningCrescent = "Waning Crescent";

            public Phase(string name, double start, double end)
            {
                Name = name;
                Start = start;
                End = end;
            }

            public string Name { get; }

            /// <summary>
            /// The days into the cycle this phase starts
            /// </summary>
            public double Start { get; }

            /// <summary>
            /// The days into the cycle this phase ends
            /// </summary>
            public double End { get; }
        }
    }
}