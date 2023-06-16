using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace px_mobile
{
    public static class RandomNumberGenerator
    {
        public static Random Random = new Random();
        public static int GenerateRandomNumber(int min, int max)
        {
            var rDouble = min * 1000 + Random.NextDouble() * (max * 1000); //for doubles
            return (int)rDouble;
        }

        public static int RandomNumberBetween(int min, int max)
        {
            return Random.Next(min, max);
        }

        public static double RandomNumberWithDecimals(int min, int max, int numberOfDecimals)
        {
            double randomNumber = Math.Round(Random.NextDouble() * (max - min) + min, numberOfDecimals);
            return randomNumber;
        }

    }
}
