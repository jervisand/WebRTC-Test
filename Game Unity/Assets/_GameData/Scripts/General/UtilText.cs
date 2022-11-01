using System.Linq;
using System.Text.RegularExpressions;
using System;
using Lean.Localization;

namespace Nagih
{
    public class UtilText
    {
        public static readonly char[] SingleUnit = new char[] { 'M', 'B', 'T' };
        public static readonly char[] RepeatableUnit = Enumerable.Range('a', 26).Select(x => (char)x).ToArray();

        public static string FormatNumber(double number, int decimalNumber = 3)
        {
            string decimalFormat = decimalNumber == 0 ? string.Empty : ("." + new string('#', 3));
            int power = (int)Math.Log10(number);
            if (power < 6)
            {
                return number.ToString("#,##0");
            }
            else
            {
                //string str = number.ToString().Substring(0, 6);
                //str = Regex.Replace(str, ".{3}(?!$)", "$0,");
                if (power < 15)
                {
                    int index = (power - 6) / 3;
                    number /= Math.Pow(10, index * 3 + 6);
                    return $"{number.ToString("###" + decimalFormat)} {SingleUnit[index]}";
                }
                else
                {
                    int index = (power - 15) / 3;
                    number /= Math.Pow(10, index * 3 + 15);
                    string str = number.ToString("###" + decimalFormat);

                    int count = index / 26 + 1;
                    char ch = RepeatableUnit[index % 26];
                    string unit = count <= 2 ? new string(ch, count) : $"{count}{ch}";

                    return $"{str} {unit}";
                }
            }
        }

        public static string FormatResearchTime(int seconds)
        {
            TimeSpan span = TimeSpan.FromSeconds(seconds);
            string result = $"{span.Hours:D2} : {span.Minutes:D2} : {span.Seconds:D2}";
            return result;
        }

        public static string FormatTimeUnit(int seconds)
        {
            string result = string.Empty;
            int unit = seconds / 3600;
            if (unit > 1)
            {
                result = unit + " " + LeanLocalization.GetTranslationText("Game/Hours");
            }
            else
            {
                unit = seconds / 60;
                if (unit > 1)
                {
                    result = unit + " " + LeanLocalization.GetTranslationText("Game/Minutes");
                }
                else
                {
                    result = seconds + " " + LeanLocalization.GetTranslationText("Game/Seconds");
                }
            }
            return result;
        }

        public static string FormatPercentage(double value)
        {
            return $"{value: +#;-#;0}%";
        }

        public static string SeparateCapital(string text)
        {
            return string.Join(" ", Regex.Split(text, @"(?<!^)(?=[A-Z](?![A-Z]|$))"));
        }
    }
}