using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RecipesProject
{
    //-- Методы для работы с временем
    public static class CookingTimeMethods
    {
        //-- Конвертация минут в строку по типу [число]ч [число]м
        public static string ConvertingFromMinutesToString(int minute)
        {
            string result = "";

            if (minute > 60)
            {
                result += $"{minute / 60}ч";

            }

            if (minute % 60 != 0)
            {
                result += $" {minute % 60}м";
            }

            return result;
        }

        //-- Конвертация строки по типу [число]ч [число]м в числовые данные
        public static (int hour, int minute) ConvertingTimeString(string CookingTime)
        {
            int hour = 0;
            int minute = 0;

            if (!String.IsNullOrEmpty(CookingTime))
            {
                Regex regexHour = new Regex(@"\b([0-9]{1,2})ч\b");
                Regex regexMinute = new Regex(@"\b([0-9]{1,2})м\b");

                foreach (Match match in regexHour.Matches(CookingTime))
                {
                    if (int.TryParse(match.Groups[1].Value, out int hourValue))
                    {
                        hour = hourValue;
                    }
                }

                foreach (Match match in regexMinute.Matches(CookingTime))
                {
                    if (int.TryParse(match.Groups[1].Value, out int min))
                    {
                        minute = min;
                    }
                }
            }
            return (hour, minute);
        }

        //-- Конвертация строки по типу [число]ч [число]м в количество минут
        public static int ConvertingTimeFromStringToMinutes(string CookingTime)
        {
            var time = ConvertingTimeString(CookingTime);

            return time.hour * 60 + time.minute;
        }
    }
}
