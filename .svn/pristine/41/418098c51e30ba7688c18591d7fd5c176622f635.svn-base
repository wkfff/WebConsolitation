using System;
using System.Collections.Generic;

namespace Krista.FM.Extensions
{
    public static class TimeSpanExtensions
    {
        private static readonly Dictionary<int, string[]> Times;

        static TimeSpanExtensions()
        {
            Times = new Dictionary<int, string[]>
                        {
                            { 365*24*60*60, new [] { "год", "года", "лет" } },
                            { 30*24*60*60, new [] { "месяц", "месяца", "месяцев" } },
                            { 7*24*60*60, new [] { "неделя", "недели", "недель" } },
                            { 24*60*60, new [] { "день", "дня", "дней" } },
                            { 60*60, new [] { "час", "часа", "часов" } },
                            { 60, new [] { "минута", "минуты", "минут" } },
                            { 1, new [] { "секунда", "секунды", "секунд" } },
                        };
        }

        public static string Plural(long number, string[] plurals)
        {
            var plural = number % 10 == 1 && number % 100 != 11 
                ? 0
                : number % 10 >= 2 && number % 10 <= 4 && (number % 100 < 10 || number % 100 >= 20) 
                    ? 1 
                    : 2;
            
            return plurals[plural];
        }

        /// <summary>
        /// Преобразовавает интервал времени в строковое представление в виде: 1 год 2 месяца.
        /// </summary>
        /// <param name="timeSpan">Интервал времени.</param>
        public static string ToFrendlyString(this TimeSpan timeSpan)
        {
            return timeSpan.ToFrendlyString(2);
        }

        /// <summary>
        /// Преобразовавает интервал времени в строковое представление в виде: 1 год 2 месяца 4 недели 5 дней 21 час.
        /// </summary>
        /// <param name="timeSpan">Интервал времени.</param>
        /// <param name="precision">Степень детализации интервала.</param>
        public static string ToFrendlyString(this TimeSpan timeSpan, int precision)
        {
            var passed = Convert.ToInt64(timeSpan.TotalSeconds);
            if (passed < 0)
            {
                return "опоздание " + ToFrendlyString(Math.Abs(passed), precision);
            }

            return ToFrendlyString(passed, precision);
        }

        /// <summary>
        /// Преобразовавает интервал времени в строковое представление в виде: 1 год 2 месяца 4 недели 5 дней 21 час.
        /// </summary>
        /// <param name="passed">Интервал времени в секундах.</param>
        /// <param name="precision">Степень детализации интервала.</param>
        public static string ToFrendlyString(long passed, int precision)
        {
            if (passed < 5)
            {
                return "менее 5 секунд";
            }

            var exit = 0;
            var output = new List<string>();
            foreach (var time in Times)
            {
                if (exit >= precision || (exit > 0 && time.Key < 60))
                {
                    break;
                }

                var result = passed / time.Key;
                if (result > 0) 
                {
                    output.Add(result + " " + Plural(result, time.Value));
                    passed -= result * time.Key;
                    exit++;
                } 
                else if (exit > 0)
                {
                    exit++;
                }
            }

            return String.Join(" ", output.ToArray());
        }
    }
}