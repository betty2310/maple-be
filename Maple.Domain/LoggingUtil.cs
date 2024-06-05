// namespace Maple.API.Utils;
//
// public static class LoggingUtil
// {
//     public static void LogMessage(ILogger logger, LogLevel logLevel, string message)
//     {
//         switch (logLevel)
//         {
//             case LogLevel.Information:
//                 logger.LogInformation(message);
//                 break;
//             case LogLevel.Warning:
//                 logger.LogWarning(message);
//                 break;
//             case LogLevel.Error:
//                 logger.LogError(message);
//                 break;
//             case LogLevel.Critical:
//                 logger.LogCritical(message);
//                 break;
//             case LogLevel.Trace:
//             case LogLevel.Debug:
//             case LogLevel.None:
//             default:
//                 logger.LogTrace(message);
//                 break;
//         }
//     }
// }