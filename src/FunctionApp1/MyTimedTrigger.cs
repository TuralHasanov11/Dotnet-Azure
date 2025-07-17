using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FunctionApp1;

internal class MyTimedTrigger
{
    [Function(nameof(MyTimedTrigger))]
    [FixedDelayRetry(5, "00:00:10")]
    public static void Run([TimerTrigger("0 */5 * * * *")] TimerInfo timerInfo, FunctionContext context)
    {
        var logger = context.GetLogger(nameof(MyTimedTrigger));
        logger.LogInformation($"Function Ran. Next timer schedule = {timerInfo.ScheduleStatus?.Next}");
    }
}
