using System.Text.RegularExpressions;

namespace Accelerator
{
    public enum ANSIColors
    {
        Black,
        Red,
        Green,
        Yellow,
        Blue,
        Magenta,
        Cyan,
        White,
        Reset,
        Gray,
        LightGray,
        Bold
    }

    class Color
    {
        public static Dictionary<ANSIColors, string> Enum()
        {
            return new Dictionary<ANSIColors, string>
            {
                { ANSIColors.Black, "\u001b[30m" },
                { ANSIColors.Red, "\u001b[31m" },
                { ANSIColors.Green, "\u001b[32m" },
                { ANSIColors.Yellow, "\u001b[33m" },
                { ANSIColors.Blue, "\u001b[34m" },
                { ANSIColors.Magenta, "\u001b[35m" },
                { ANSIColors.Cyan, "\u001b[36m" },
                { ANSIColors.White, "\u001b[37m" },
                { ANSIColors.Reset, "\u001b[0m" },
                { ANSIColors.LightGray, "\u001b[90m" },
                { ANSIColors.Gray, "\u001b[37m" },
                { ANSIColors.Bold, "\u001b[1m" },
            };
        }

        public static Dictionary<string, ANSIColors> Reverse()
        {
            return new Dictionary<string, ANSIColors>
            {
                { "black", ANSIColors.Black },
                { "red", ANSIColors.Red },
                { "green", ANSIColors.Green },
                { "yellow", ANSIColors.Yellow },
                { "blue", ANSIColors.Blue },
                { "magenta", ANSIColors.Magenta },
                { "cyan", ANSIColors.Cyan },
                { "white", ANSIColors.White },
                { "reset", ANSIColors.Reset },
                { "gray", ANSIColors.Gray },
                { "bold", ANSIColors.Bold },
                { "lgray", ANSIColors.LightGray },
            };
        }

        public static string Colors(ANSIColors ansi, string message)
        {
            var cols = Enum();
            return $"{cols[ansi]}{message}{cols[ANSIColors.Reset]}";
        }

        public static (string csl, string rgt) Formatter(string message)
        {
            var colorTagPattern = new Regex(@"\[([^\]]+)\]\.(\w+)(-b)?", RegexOptions.IgnoreCase);
            var colors = Reverse();
            string result = colorTagPattern.Replace(
                message,
                match =>
                {
                    string text = match.Groups[1].Value;
                    string color = match.Groups[2].Value;
                    bool bold = match.Groups[3].Success;

                    string formattedText = bold ? Colors(colors["bold"], text) : text;
                    if (colors.ContainsKey(color))
                    {
                        return Colors(colors[color], text);
                    }
                    else
                    {
                        return Colors(colors["white"], formattedText);
                    }
                }
            );

            string LoggingsTxT = colorTagPattern.Replace(
                message,
                match =>
                {
                    string text = match.Groups[1].Value;
                    return $"\"{text}\"";
                }
            );

            string regexColors = @"\x1B\[[[(?);]{0,2}(;?\d)*m";
            string messageWithoutColors = Regex.Replace(LoggingsTxT, regexColors, "");

            string regexQuotes = @"/""(\d{1,})"".*?""(\d{1,})""/g";
            string adjustedMessage = Regex.Replace(
                messageWithoutColors,
                regexQuotes,
                (match) =>
                {
                    string num1 = match.Groups[1].Value;
                    string num2 = match.Groups[2].Value;
                    return $"\"{num1}\"{num2}\"";
                }
            );

            return (result, adjustedMessage);
        }
    }
}
