# Moon Phase Calculations

This console application has a `Moon` static class that allows users to calculate the current phase of the moon.

## How Calculations Happen?

We use the last known **New Moon** to calculate the next full moon. The moon phases in a cycle of **29.53** days and there are
milestones along the way.

```c#
public const string NewMoon = "New Moon";
public const string WaxingCrescent = "Waxing Crescent";
public const string FirstQuarter = "First Quarter";
public const string WaxingGibbous = "Waxing Gibbous";
public const string FullMoon = "Full Moon";
public const string WaningGibbous = "Waning Gibbous";
public const string ThirdQuarter = "Third Quarter";
public const string WaningCrescent = "Waning Crescent";
```

Calculations are based off of this [pdf found on the internet](https://www.subsystems.us/uploads/9/8/9/4/98948044/moonphase.pdf).

This library also outputs the emoji for the moon in its current phase. We can also account for the look of the moon based on which hemisphere the onlooker is viewing from.

```text
🌗 First Quarter (6.98 days)
🌖 Waxing Gibbous (7.98 days)
🌕 Full Moon (13.98 days)
🌓 Third Quarter (21.98 days)
🌑 New Moon (0.45 days)
```

## Usage

Using the the `Moon` class is straighforward.

### Using "Now"

`Now` uses `DateTime.Now` which is converted to `UTC`. Given the date and time, we can calculate the phase of the moon.

```c#
var result = Moon.Now(/* optional hemisphere */);
Console.WriteLine(result);
```

### Using "UtcNow"

`UtcNow` is similar to `Now` but uses `DateTime.UtcNow`.

```c#
var result = Moon.UtcNow(/* optional hemisphere */);
Console.WriteLine(result);
```

### Using "Calculate"

`Calculate` allows users to calculate the phase of the moon.

```c#
var dates = new[] {
    new DateTime(2020, 7, 27), new DateTime(2020, 7, 28),
    new DateTime(2020, 8, 3), new DateTime(2020, 8, 11),
    new DateTime(2020, 8, 19),
};

var results =
    dates
        .Select(x => Moon.Calculate(x))
        .Select((r, i) => $"{r.Emoji} {r.Name} ({r.DaysIntoCycle} days)\n")
        .Aggregate((a, v) => a + v);
```

With an output of:

```text
🌗 First Quarter (6.98 days, Visibility 47.27%)
🌖 Waxing Gibbous (7.98 days, Visibility 54.05%)
🌕 Full Moon (13.98 days, Visibility 94.68%)
🌓 Third Quarter (21.98 days, Visibility 51.13%)
🌑 New Moon (0.45 days, Visibility 3.05%)
```

## Classes

- `Moon` : The Earth's Moon with methods like `Now`, `UtcNow`, and `Calculate`.
- `Earth.Hemispheres` : an enum with values of `Northern` and `Southern`, used to determine the look of the moon.
- `PhaseResult`: includes properties like `Name`, `Emoji`, `DaysIntoCycle`, `Hemisphere`, and `Moment`.

## Caveats 

The calculation is not that exact because it also depends on your location on Earth. The phase of the moon could look slightly different for someone living in Los Angeles, California compared to someone living in Helsinki, Finland. 

The library uses UTC as a median point to determine what the moon will look like at UTC, both in time and location.

**If you need more exact moon phases, you'll need to use a web service and make HTTP calls.**

This library uses the last known new moon I could find on record, which is `January 21st, 1920 at 5:25 AM` over `London, England`.

```c#
// London New Moon (1920)
// https://www.timeanddate.com/moon/phases/uk/london?year=1920
var daysSinceLastNewMoon =
    new DateTime(1920, 1, 21, 5, 25, 00, DateTimeKind.Utc).ToOADate() + julianConstant;
```

## License

Copyright 2020 Khalid Abuhakmeh

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


