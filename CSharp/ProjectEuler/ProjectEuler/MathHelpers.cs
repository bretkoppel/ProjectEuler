using System;
using System.Numerics;

namespace ProjectEuler
{
    public static class MathHelpers
    {
        public static double Sqrt(BigInteger i)
        {
            return Root(i, 2);
        }

        public static double Root(BigInteger i, int rootValue)
        {
            return Math.Floor(Math.Exp(BigInteger.Log(i)/rootValue));
        }
    }
}