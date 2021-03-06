﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.Caching;
using NUnit.Framework;

namespace ProjectEuler
{
    public class Problem3 : BaseProblem
    {
        /* This test is way too slow for benchmarking. Left for posterity.
        [Test]
        public void Problem3_Slow()
        {
            var primes = new[] {2,3,5,7,11,13,17,19,23,29,31,37,41,43,47,53,59,61,67,71,73,79,83,89,97,101,103,107,109,113,127,131,137,139,149,151,157,163,167,173,179,181,191,193,197,199,211,223,227,229,233,239,241,251,257,263,269,271,277,281,283,293,307,311,313,317,331,337,347,349,353,359,367,373,379,383,389,397,401,409,419,421,431,433,439,443,449,457,461,463,467,479,487,491,499,503,509,521,523,541,547,557,563,569,571,577,587,593,599,601,607,613,617,619,631,641,643,647,653,659,661,673,677,683,691,701,709,719,727,733,739,743,751,757,761,769,773,787,797,809,811,821,823,827,829,839,853,857,859,863,877,881,883,887,907,911,919,929,937,941,947,953,967,971,977,983,991,997,1009};
            const long limit = 600851475143;
            var ceiling = (long) Math.Ceiling(limit/3.0);
            long largestPrime;

            for (long tester = 3; tester < ceiling; tester+=2)
            {
              if (limit%tester == 0 && primes.All(m => tester % m > 0))
              {
                Console.WriteLine("Prime Candidate: " + tester);    
                largestPrime=tester;
              }
            }
            Assert.AreEqual(6857, largestPrime);
        }*/

        [Test]
        public void Problem3_TrialDivision()
        {
            Assert.AreEqual(new BigInteger(29), PrimeFactorsByTrialDivision(13195).Max());
            Assert.AreEqual(new BigInteger(6857), PrimeFactorsByTrialDivision(600851475143).Max());
            Assert.AreEqual(new BigInteger(17), PrimeFactorsByTrialDivision(51).Max());
        }

        [Test]
        public void Problem3_Atkin()
        {
            Assert.AreEqual(new BigInteger(17), PrimeFactorsByAtkin(51).Max());
            Assert.AreEqual(new BigInteger(29), PrimeFactorsByAtkin(13195).Max());
            Assert.AreEqual(new BigInteger(6857), PrimeFactorsByAtkin(600851475143).Max());
        }

        /// <summary>
        /// The prime factors of 13195 are 5, 7, 13 and 29.
        /// What is the largest prime factor of the number 600851475143 ?
        /// </summary>
        [Test]
        public void Problem3_RationalSieve()
        {
            Assert.AreEqual(new BigInteger(29), PrimeFactorsByRationalSieve(13195).Max());
            Assert.AreEqual(new BigInteger(6857), PrimeFactorsByRationalSieve(600851475143).Max());
        }

        [Test]
        public void VerifyBSmooth()
        {
            Assert.IsFalse(IsBSmooth(1620, 3));
            Assert.IsTrue(IsBSmooth(1620, 5));
            Assert.IsTrue(IsBSmooth(252, 7));
        }

        [Test]
        public void VerifyMillerRabin()
        {
            Assert.IsTrue(IsPrimeByMillerRabin(104723, 3));
            Assert.IsTrue(IsPrimeByMillerRabin(93323, 3));
            Assert.IsFalse(IsPrimeByMillerRabin(93325, 3));
            Assert.IsFalse(IsPrimeByMillerRabin(93327, 3));
            Assert.IsFalse(IsPrimeByMillerRabin(10086647, 3));
        }

        [Test]
        public void PrimesAreNotPerfectPowers()
        {
            Assert.IsFalse(IsPerfectPower(2));
            Assert.IsFalse(IsPerfectPower(3));
            Assert.IsFalse(IsPerfectPower(11));
            Assert.IsFalse(IsPerfectPower(7919));
        }

        [Test]
        public void VerifyPerfectPowers()
        {
            Assert.IsTrue(IsPerfectPower(4));
            Assert.IsTrue(IsPerfectPower(32));
            Assert.IsTrue(IsPerfectPower(64));
        }

        [Test]
        public void VerifyAtkins()
        {
            Assert.That(PrimesByAtkin(2), Is.EqualTo(new BigInteger[] { 2 }));
            Assert.That(PrimesByAtkin(3), Is.EqualTo(new BigInteger[] { 2, 3 }));
            Assert.That(PrimesByAtkin(5), Is.EqualTo(new BigInteger[] { 2, 3, 5 }));
            Assert.That(PrimesByAtkin(11), Is.EqualTo(new BigInteger[] { 2, 3, 5, 7, 11 }));
            Assert.That(PrimesByAtkin(40), Is.EqualTo(new BigInteger[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37 }));
            Assert.That(PrimesByAtkin(41), Is.EqualTo(new BigInteger[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41 }));
            Assert.That(PrimesByAtkin(42), Is.EqualTo(new BigInteger[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41 }));
        }

        public IEnumerable<BigInteger> PrimeFactorsByAtkin(BigInteger value)
        {
            if (IsPrimeByMillerRabin(value, 10))
                return new[] { value };

            var firstPrimes = PrimesByAtkin(100).OrderBy(m => m);
            var primes = firstPrimes.Where(m => value % m == 0).ToList();
            var primeFactors = new HashSet<BigInteger>(primes);
            while (primes.Any())
            {
                value = primes.Aggregate(value, (current, prime) => current/prime);

                if (IsPrimeByMillerRabin(value, 5))
                {
                    primeFactors.Add(value);
                    return primeFactors;
                }
                primes = firstPrimes.Where(m => value % m == 0).ToList();
            }

            return primeFactors.Union(PrimeFactorsByTrialDivision(value));
        }

        public IEnumerable<BigInteger> PrimeFactorsByTrialDivision(BigInteger value)
        {
            if (IsPrimeByMillerRabin(value, 10))
                return new[] {value};

            var primeFactors = new HashSet<BigInteger>();
            while (value % 2 == 0)
            {
                primeFactors.Add(2);
                value /= 2;
            }

            var ceiling = new BigInteger(MathHelpers.Sqrt(value)) + 1;
            for (BigInteger tester = 3; tester < ceiling; tester += 2)
            {
                if (value % tester == 0 && IsPrimeByMillerRabin(tester, 3))
                {
                    value /= tester;
                    primeFactors.Add(tester);
                    if (IsPrimeByMillerRabin(value))
                    {
                        primeFactors.Add(value);
                        break;
                    }

                    tester -= 2;
                }
            }

            return primeFactors;
        }

        public IEnumerable<BigInteger> PrimeFactorsByRationalSieve(BigInteger value)
        {

            var bound = value < 10000 ? Math.Ceiling(MathHelpers.Sqrt(value)) : Math.Ceiling(Math.Pow(BigInteger.Log(value, 2), 3));
            return PrimeFactorsByRationalSieve(value, new BigInteger(bound));
        }

        [Caching]
        public IEnumerable<BigInteger> PrimeFactorsByRationalSieve(BigInteger value, BigInteger bound)
        {
            if (IsPrimeByMillerRabin(value, 10))
                return new[] { value };

            var primes = PrimesByAtkin(bound);
            var suitablePrimes = primes.Where(m => value % m == 0);
            if (suitablePrimes.Any())
            {
                var product = suitablePrimes.Aggregate(BigInteger.One, (accumulator, current) => accumulator * current);
                return suitablePrimes.Union(this.PrimeFactorsByRationalSieve(value / product, bound));
            }

            var zValues = new Dictionary<BigInteger, List<List<int>>>();
            for (int i = 1; i < value && zValues.Count < (primes.Count + 3); i++)
            {
                if (!IsBSmooth(i, bound) || !IsBSmooth(i + value, bound))
                    continue;

                var leftList = new List<int>(primes.Count);
                var rightList = new List<int>(primes.Count);
                foreach (var prime in primes)
                {
                    var exponent = 1;
                    while (i % BigInteger.Pow(prime, exponent) == 0)
                    {
                        exponent += 1;
                    }
                    leftList.Add(exponent - 1);

                    exponent = 1;
                    while ((i + value) % BigInteger.Pow(prime, exponent) == 0)
                    {
                        exponent += 1;
                    }
                    rightList.Add(exponent - 1);
                }
                zValues.Add(i, new List<List<int>> { leftList, rightList });
            }

            if (zValues.Any() == false)
                return suitablePrimes;

            var allEvens = zValues.Values.FirstOrDefault(m => m[0].All(n => n % 2 == 0) && m[1].All(n => n % 2 == 0));
            if (allEvens == null)
            {
                for (int i = 0; i < zValues.Count && allEvens == null; i++)
                {
                    for (int n = i + 1; n < zValues.Count && allEvens == null; n++)
                    {
                        var leftList = new List<int>(primes.Count);
                        var rightList = new List<int>(primes.Count);
                        for (int j = 0; j < primes.Count; j++)
                        {
                            var leftAddition = zValues.ElementAt(i).Value[0][j] + zValues.ElementAt(n).Value[0][j];
                            var rightAddition = zValues.ElementAt(i).Value[1][j] + zValues.ElementAt(n).Value[1][j];
                            if (leftAddition == rightAddition)
                            {
                                leftList.Add(0);
                                rightList.Add(0);
                            }
                            else
                            {
                                leftList.Add(leftAddition);
                                rightList.Add(rightAddition);
                            }
                        }

                        if (leftList.All(m => m % 2 == 0) && rightList.All(m => m % 2 == 0))
                            allEvens = new List<List<int>> { leftList, rightList };
                    }
                }
            }

            var leftProduct = BigInteger.One;
            var rightProduct = BigInteger.One;
            for (int i = 0; i < primes.Count; i++)
            {
                leftProduct *= BigInteger.Pow(primes.ElementAt(i), allEvens[0][i]);
                rightProduct *= BigInteger.Pow(primes.ElementAt(i), allEvens[1][i]);
            }

            var leftSquare = MathHelpers.Sqrt(leftProduct);
            var rightSquare = MathHelpers.Sqrt(rightProduct);
            return suitablePrimes.Union(new[] { GreatestCommonFactor(Math.Abs((int)leftSquare - (int)rightSquare), value), GreatestCommonFactor((int)leftSquare + (int)rightSquare, value) });
        }

        [Caching]
        public static BigInteger GreatestCommonFactor(BigInteger x, BigInteger y)
        {
            if (x == y)
                return x;

            var divisor = x > y ? y : x;
            while (divisor > 0 && (x % divisor != 0 || y % divisor != 0))
            {
                divisor -= 1;
            }
            return divisor;
        }

        [Caching]
        public static bool IsBSmooth(BigInteger value, BigInteger bound)
        {
            var primes = PrimesByAtkin(value);
            return !primes.Any(m => value % m == 0 && m > bound);
        }

        [Caching]
        public static bool IsPrimeByMillerRabin(BigInteger candidate, int numberOfRounds = 1)
        {
            var primeList = PrimesByAtkin(1000);
            if (candidate <= 3 || primeList.Contains(candidate))
                return true;

            if (candidate % 2 == 0 || primeList.Any(m => candidate % m == 0))
                return false;

            var s = 0;
            var d = candidate - 1;
            while (d % 2 == 0)
            {
                s++;
                d = d / 2;
            }
            
            var rand = new Random();
            for (int i = 0; i < numberOfRounds; i++)
            {
                var a = rand.Next(2, candidate > int.MaxValue ? int.MaxValue - 2 : (int)candidate - 2);
                var c = BigInteger.ModPow(a, d, candidate);
                if (c == 1 || c == candidate-1)
                    continue;

                for (int r = 0; r < s; r++)
                {
                    c = BigInteger.ModPow(c, 2, candidate);
                    if (c == 1)
                        return false;

                    if (c == candidate - 1)
                        break;

                    if(r == s-1)
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Determines whether the given number is a Perfect Power.
        /// </summary>
        /// <param name="n">The number to test.</param>
        /// <remarks>
        /// https://en.wikipedia.org/wiki/Perfect_power
        /// </remarks>
        public static bool IsPerfectPower(BigInteger n)
        {
            var testLimit = BigInteger.Log(n, 2D);
            var testers = PrimesByAtkin(new BigInteger(Math.Floor(testLimit)));
            foreach (var tester in testers)
            {
                if (Math.Abs(Math.Exp(BigInteger.Log(n)/(double) tester) % 1) < Epsilon)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Gets all of the prime numbers up to and including the limit using the sieve of Atkin(https://en.wikipedia.org/wiki/Sieve_of_atkin).
        /// </summary>
        /// <param name="limit">The limit.</param>
        public static ICollection<BigInteger> PrimesByAtkin(BigInteger limit)
        {
            var primes = new HashSet<BigInteger> { 2, 3, 5 };
            if (limit <= 5)
                return primes.Where(m => m <= limit).ToArray();

            var cachedPrimes = MemoryCache.Default.Get("LargestAtkin");
            if (cachedPrimes != null)
            {
                var castPrimes = (KeyValuePair<BigInteger, IOrderedEnumerable<BigInteger>>)cachedPrimes;
                if (limit == castPrimes.Key)
                    return castPrimes.Value.ToArray();
                
                if (limit < castPrimes.Key)
                    return castPrimes.Value.Where(m => m <= limit).ToArray();
            }

            /*var flip1 = new BigInteger[] { 1, 13, 17, 29, 37, 41, 49, 53 };
            var flip2 = new BigInteger[] { 7, 19, 31, 43 };
            var flip3 = new BigInteger[] { 11, 23, 47, 59 };*/
            var flip1 = new BigInteger[] { 1, 5 };
            var flip2 = new BigInteger[] { 7 };
            var flip3 = new BigInteger[] { 11 };

            // no built in sqrt, but MSDN offers this(http://msdn.microsoft.com/en-us/library/dd268263.aspx)
            var sqrt = new BigInteger(Math.Ceiling(MathHelpers.Sqrt(limit)));
            var numbers = new HashSet<BigInteger>();
            Action<BigInteger> flipNumbers = (n) =>
            {
                if (!numbers.Add(n))
                    numbers.Remove(n);
            };

            for (BigInteger x = 1; x <= sqrt; x++)
            {
                for (BigInteger y = 1; y <= sqrt; y++)
                {
                    BigInteger n = (4 * (x * x)) + (y * y);
                    if (n <= limit && (flip1.Contains(n % 12)))
                        flipNumbers(n);

                    n = (3 * (x * x)) + (y * y);
                    if (n <= limit && (flip2.Contains(n % 12)))
                        flipNumbers(n);

                    n = (3 * (x * x)) - (y * y);
                    if (x > y && n <= limit && (flip3.Contains(n % 12)))
                        flipNumbers(n);
                }
            }

            var sortedNumbers = numbers.OrderBy(m => m);
            foreach (var sieveNumber in sortedNumbers)
            {
                if (!numbers.Contains(sieveNumber))
                    continue;

                primes.Add(sieveNumber);
                var squareSieve = sieveNumber * sieveNumber;
                var multiplier = 1;
                while (multiplier * squareSieve <= limit)
                {
                    numbers.Remove(multiplier * squareSieve);
                    multiplier += 1;
                }
            }

            MemoryCache.Default["LargestAtkin"] = new KeyValuePair<BigInteger, IOrderedEnumerable<BigInteger>>(limit, primes.OrderBy(m => m));
            return primes;
        }
    }
}
