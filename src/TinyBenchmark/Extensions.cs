using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TinyBenchmark
{
    internal static class Extensions
    {
        public static string LimitTo(this string message, int maxLength)
        {
            if (maxLength < 0) throw new ArgumentException("Max length must be non-negative.");

            int messageLength = message?.Length ?? 0;

            if (messageLength <= maxLength)
                return message;

            var croppedMessage = message.Substring(0, maxLength);
            return $"{croppedMessage}...";
        }
    }
}
