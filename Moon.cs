using System;
using System.Collections.Generic;
using System.Linq;

namespace MoonPhaseConsole
{
    public static class Moon
    {
        private static readonly IReadOnlyList<string> SouthernHemisphere
            = new List<string> {"🌑", "🌒", "🌓", "🌔", "🌕", "🌖", "🌗", "🌘", "🌑"};

        private static readonly IReadOnlyList<string> NorthernHemisphere
            = SouthernHemisphere.Reverse().ToList();

        private static readonly List<string> names = new List<string>
        {
            Phase.NewMoon,
            Phase.WaxingCrescent, Phase.FirstQuarter, Phase.WaxingGibbous,
            Phase.FullMoon,
            Phase.WaningGibbous, Phase.ThirdQuarter, Phase.WaningCrescent,
            Phase.NewMoon
        };

        private const double TotalLengthOfCycle = 29.53;

        private static readonly double[] daysIntoCycle = new double[]
            // added some buffer space in the beginning and end so that new moon gets a chance
            {0, 1, 3.5, 7, 11, 15, 18.5, 22, 25.75, 29.00, TotalLengthOfCycle};

        public static IReadOnlyList<Phase> Phases => allPhases.AsReadOnly();
        private static readonly List<Phase> allPhases = new List<Phase>();

        static Moon()
        {
            var phases = new List<Phase>();
            for (var i = 0; i < names.Count(); i++)
            {
                var phase = new Phase(names[i], daysIntoCycle[i], daysIntoCycle[i + 1]);
                phases.Add(phase);
            }

            allPhases = phases;
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
                allPhases
                    .Where(p => intoCycle >= p.Start && intoCycle < p.End)
                    .First();

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

            public override string ToString()
            {
                var percent = Math.Round(100 - ((DaysIntoCycle / TotalLengthOfCycle) * 100), 2);
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