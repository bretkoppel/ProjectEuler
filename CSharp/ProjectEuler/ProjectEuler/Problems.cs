using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using NUnit.Framework;

namespace ProjectEuler
{
    public class Problems : BaseProblem
    {
        

        /// <summary>
        /// A palindromic number reads the same both ways. The largest palindrome made from the product of two 2-digit numbers is 9009 = 91 × 99.
        /// Find the largest palindrome made from the product of two 3-digit numbers.
        /// </summary>
        [Test]
        public void Problem4()
        {
            var palindrome = 0;
            for (int i = 999; i > 0; i--)
            {
                for (int j = 999; j > 0; j--)
                {
                    var product = (i * j);
                    if (product <= palindrome)
                        continue;

                    var productString = product.ToString();
                    var l = productString.Substring(0, productString.Length / 2);
                    var r = string.Join("", productString.Reverse()).Substring(0, productString.Length / 2);
                    if (l == r)
                    {
                        palindrome = product;
                        break;
                    }                        
                }
            }
            Assert.AreEqual(906609, palindrome);
        }

        /// <summary>
        /// 2520 is the smallest number that can be divided by each of the numbers from 1 to 10 without any remainder.
        /// What is the smallest positive number that is evenly divisible by all of the numbers from 1 to 20?
        /// </summary>
        [Test]
        public void Problem5()
        {
            int result = 0;
            int multiplier = 1;
            while (result == 0)
            {
                var tester = 20 * multiplier;
                bool isValid = true;
                for (int i = 19; i > 1; i--)
                {
                    if (tester % i > 0)
                    {
                        isValid = false;
                        break;
                    }
                }

                if (isValid)
                    result = tester;
                else
                    multiplier += 1;
            }
            Assert.AreEqual(232792560, result);
        }

        /// <summary>
        /// Determines whether the number is prime using the Baillie–PSW primality test
        /// </summary>
        /// <param name="candidate">The prime candidate</param>
        /// <remarks> 
        /// https://en.wikipedia.org/wiki/Baillie-PSW_primality_test
        /// </remarks>
        /*private static bool IsPrimeByBaillePSW(long candidate)
        {
            // verify that we aren't divisible by a small prime.
            if (PrimesByAtkin(100).Any(m => candidate%m == 0))
                return false;

            if (IsPrimeByMillerRabin(candidate) == false)
                return false;

            if (Math.Abs(Math.Sqrt(candidate)%1) < Epsilon)
                return false;

            
            for (int i = 5; true; i += 2)
            {
                
            }

                if (IsPerfectPower(candidate))
                    return false;

            long r = 0;
            var limit = Math.Floor(Math.Log(candidate, 2));
            var q = limit;
            while (r == 0)
            {
                for (long j = 1; j <= limit; j++)
                {
                    if (Math.Abs((Math.Pow(candidate, j)%q) - (1%q)) < Epsilon)
                        q++;
                    else
                    {
                        r = (long) q;
                        break;
                    }
                }
            }

            for (long a = 1; a <= r; a++)
            {
                if (GreatestCommonFactor(candidate, a) > 1)
                    return false;
            }

            var primeBoundary = Math.Floor(Math.Sqrt(r)*Math.Log(candidate, 2));
            for (long a = 1; a <= primeBoundary; a++)
            {
                if (GreatestCommonFactor(candidate, a) > 1)
                    return false;
            }

                return false;
        }

        [Test]
        public void VerifyJacobiSybol()
        {
            Assert.AreEqual(-1, CalculateJacobiSymbol(1001, 9907));
            Assert.AreEqual(0, CalculateJacobiSymbol(0, 9));
            Assert.AreEqual(1, CalculateJacobiSymbol(4, 11));
        }

        /// <summary>
        /// Caculates the Jacobi symbol of a numerator and an odd denominator.
        /// </summary>
        /// <param name="a">The numerator</param>
        /// <param name="b">The denominator. Must be odd.</param>
        private static double CalculateJacobiSymbol(double a, double b)
        {
            if (Math.Abs(b%2 - 0) < Epsilon)
                throw new ArithmeticException("Argument b must be odd");

            a = a%b;
            if (Math.Abs(a) < Epsilon)
                return 0;

            if (Math.Abs(a - 1) < Epsilon)
                return 1;

            if (Math.Abs(a%2) < Epsilon)
            {
                if (Math.Abs(Math.Abs(b%8.0) - 1) < Epsilon)
                    return CalculateJacobiSymbol(a/2, b);
                else
                    return -1 * CalculateJacobiSymbol(a / 2, b);
            }
            else
            {
                if (Math.Abs(b%4 - 3) < Epsilon && Math.Abs(a%4 - 3) < Epsilon)
                    return -1*CalculateJacobiSymbol(b, a);
                else
                    return CalculateJacobiSymbol(b, a);
            }
        }

        private static bool IsPrimeByLucas(long candidate)
        {
            if (candidate % 2 == 0 || Math.Abs((Math.Sqrt(candidate) % 1) - 0) < Epsilon || PrimesByAtkin(100).Any(m => candidate % m == 0))
                return false;

            var d = 0;
            var i = 5;
            long p = 1;
            double q;
            while (d == 0)
            {
                if (Math.Abs(CalculateJacobiSymbol(i, candidate) - -1) < Epsilon)
                {
                    d = i;
                    q = (1 - d) / 4D;
                }

                i = i < 0 ? Math.Abs(i) + 2 : -(i + 2);
            }

            // holds because we're specifically looking at Jacobi = -1 above.
            var dn = candidate + 1;
            long u = 1;
            long v = 1;
            long u2, v2;
            for (int i = Convert.ToString(dn, 2).Length - 2; i > 0; i--)
            {
                u2 = (u*v)%candidate;
                v2 = (long)(Math.Pow(v, 2) + (d*Math.Pow(u, 2))%candidate);

            }
            var s = 0;
            while (dn % 2 == 0)
            {
                s++;
                dn = dn / 2;
            }

            var fib = MathHelpers.GetNthFibonnaci(dn);
            if (fib%candidate == 0)
                return true;

            return false;
        }*/

        
    }
}

