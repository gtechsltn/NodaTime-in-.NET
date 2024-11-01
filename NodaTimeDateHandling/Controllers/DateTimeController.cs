using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NodaTime.Text;
using NodaTime;

namespace NodaTimeDateHandling.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DateTimeController : ControllerBase
    {
        private readonly IClock _clock;
        private readonly DateTimeZone _timeZone;

        public DateTimeController(IClock clock)
        {
            _clock = clock;
            _timeZone = DateTimeZoneProviders.Tzdb["America/New_York"];
        }

        [HttpGet("current-utc")]
        public IActionResult GetCurrentUtc()
        {
            var now = _clock.GetCurrentInstant();
            var utcDateTime = now.ToDateTimeUtc();

            // Format the DateTime to a string in ISO 8601 format
            var utcTime = utcDateTime.ToString("o"); // "o" specifies the round-trip format

            return Ok(new { UtcTime = utcTime });
        }

        [HttpGet("current-local")]
        public IActionResult GetCurrentLocal()
        {
            var now = _clock.GetCurrentInstant();
            var localDateTime = now.InZone(_timeZone).ToDateTimeUnspecified();
            return Ok(new { LocalTime = localDateTime });
        }

        [HttpGet("duration")]
        public IActionResult GetDurationBetweenDates([FromQuery] string start, [FromQuery] string end)
        {
            var startDate = LocalDateTimePattern.CreateWithInvariantCulture("yyyy-MM-dd").Parse(start).Value;
            var endDate = LocalDateTimePattern.CreateWithInvariantCulture("yyyy-MM-dd").Parse(end).Value;

            var duration = Period.Between(startDate, endDate, PeriodUnits.Days);
            return Ok(new { DurationDays = duration.Days });
        }

        [HttpGet("next-friday")]
        public IActionResult GetNextFriday()
        {
            var today = LocalDate.FromDateTime(DateTime.Now);
            var nextFriday = today.Next(IsoDayOfWeek.Friday);
            return Ok(new { NextFriday = nextFriday });
        }
    }
}
