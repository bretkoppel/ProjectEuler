using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;

namespace ProjectEuler
{
    public class Problem3 : BaseProblem
    {
        /* This test is way too slow for benchmarking. Left for posterity.
        [Test]
        public void Problem3_Slow()
        {
            const long limit = 600851475143;
            var ceiling = (long) Math.Ceiling(limit/2.0);

            // since 2 is the smallest prime, it's largest factor can't be any larger
            long largestPrime = 0;
            for (long tester = 2; tester < ceiling; tester++)
            {
                if (limit%tester == 0)
                    largestPrime = tester;

                if (tester%100000 == 0)
                    Debug.WriteLine(tester);
            }
            Assert.AreEqual(6857, largestPrime);
        }*/

        /// <summary>
        /// The prime factors of 13195 are 5, 7, 13 and 29.
        /// What is the largest prime factor of the number 600851475143 ?
        /// </summary>
        [Test]
        public void Problem3_RationalSieve()
        {
            Assert.AreEqual(29, PrimeFactorsByRationalSieve(13195).Max());
            Assert.AreEqual(6857, PrimeFactorsByRationalSieve(600851475143).Max());
        }

        [Test]
        public void VerifyBSmooth()
        {
            Assert.IsFalse(IsBSmooth(1620, 3));
            Assert.IsTrue(IsBSmooth(1620, 5));
            Assert.IsTrue(IsBSmooth(252, 7));
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
            Assert.That(PrimesByAtkin(2), Is.EqualTo(new long[] { 2 }));
            Assert.That(PrimesByAtkin(3), Is.EqualTo(new long[] { 2, 3 }));
            Assert.That(PrimesByAtkin(5), Is.EqualTo(new long[] { 2, 3, 5 }));
            Assert.That(PrimesByAtkin(11), Is.EqualTo(new long[] { 2, 3, 5, 7, 11 }));
            Assert.That(PrimesByAtkin(40), Is.EqualTo(new long[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37 }));
            Assert.That(PrimesByAtkin(41), Is.EqualTo(new long[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41 }));
            Assert.That(PrimesByAtkin(42), Is.EqualTo(new long[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41 }));
        }

        public IEnumerable<long> PrimeFactorsByRationalSieve(long value)
        {
            return PrimeFactorsByRationalSieve(value, (long)Math.Ceiling(Math.Sqrt(value)));
        }

        [Caching]
        public IEnumerable<long> PrimeFactorsByRationalSieve(long value, long bound)
        {
            var primes = PrimesByAtkin(bound);
            var suitablePrimes = primes.Where(m => value % m == 0);
            if (suitablePrimes.Any())
            {
                var product = suitablePrimes.Aggregate((long)1, (accumulator, current) => accumulator * current);
                return suitablePrimes.Union(this.PrimeFactorsByRationalSieve(value / product, bound));
            }

            if (IsPrimeByMillerRabin(value, 3) && PrimesByAtkin(value).Contains(value))
                return new[] { value };

            var zValues = new Dictionary<int, List<List<int>>>();
            for (int i = 1; i < value && zValues.Count < (primes.Count + 3); i++)
            {
                if (!IsBSmooth(i, bound) || !IsBSmooth(i + value, bound))
                    continue;

                var leftList = new List<int>(primes.Count);
                var rightList = new List<int>(primes.Count);
                foreach (var prime in primes)
                {
                    var exponent = 1;
                    while (i % Math.Pow(prime, exponent) == 0)
                    {
                        exponent += 1;
                    }
                    leftList.Add(exponent - 1);

                    exponent = 1;
                    while ((i + (double)value) % Math.Pow(prime, exponent) == 0)
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

            double leftProduct = 1;
            double rightProduct = 1;
            for (int i = 0; i < primes.Count; i++)
            {
                leftProduct *= Math.Pow(primes.ElementAt(i), allEvens[0][i]);
                rightProduct *= Math.Pow(primes.ElementAt(i), allEvens[1][i]);
            }

            var leftSquare = Math.Sqrt(leftProduct);
            var rightSquare = Math.Sqrt(rightProduct);
            return suitablePrimes.Union(new[] { GreatestCommonFactor(Math.Abs((int)leftSquare - (int)rightSquare), value), GreatestCommonFactor((int)leftSquare + (int)rightSquare, value) });
        }

        [Caching]
        public static long GreatestCommonFactor(long x, long y)
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
        public static bool IsBSmooth(long value, long bound)
        {
            var primes = PrimesByAtkin(value);
            return !primes.Any(m => value % m == 0 && m > bound);
        }

        [Caching]
        public static bool IsPrimeByMillerRabin(long candidate, int numberOfRounds = 1)
        {
            if (candidate % 2 == 0 || Math.Abs((Math.Sqrt(candidate) % 1) - 0) < Epsilon || PrimesByAtkin(1000).Any(m => candidate % m == 0))
                return false;

            var s = 0;
            var d = candidate - 1;
            while (d % 2 == 0)
            {
                s++;
                d = d / 2;
            }

            var rand = new Random();
            var probablePrime = true;
            for (int i = 0; i < numberOfRounds; i++)
            {
                var a = rand.Next(2, candidate > int.MaxValue ? int.MaxValue : (int)candidate);
                var potentialPrime = false;
                for (int r = 0; r <= s; r++)
                {
                    if (Math.Abs(Math.Pow(a, Math.Pow(2, r) * d) % candidate - (candidate - 1)) < Epsilon)
                    {
                        potentialPrime = true;
                        break;
                    }
                }

                if (potentialPrime == false)
                {
                    probablePrime = false;
                    break;
                }
            }

            return probablePrime;
        }

        /// <summary>
        /// Determines whether the given number is a Perfect Power.
        /// </summary>
        /// <param name="n">The number to test.</param>
        /// <remarks>
        /// https://en.wikipedia.org/wiki/Perfect_power
        /// </remarks>
        public static bool IsPerfectPower(long n)
        {
            var testLimit = Math.Log(n, 2D);
            var testers = PrimesByAtkin((long)Math.Truncate(testLimit));
            foreach (var tester in testers)
            {
                if (Math.Abs(Math.Pow(n, (1D / tester)) % 1) < Epsilon)
                    return true;
            }
            return false;
        }

        private static KeyValuePair<long, ICollection<long>> _cachedPrimes = new KeyValuePair<long, ICollection<long>>();

        /// <summary>
        /// Gets all of the prime numbers up to and including the limit using the sieve of Atkin(https://en.wikipedia.org/wiki/Sieve_of_atkin).
        /// </summary>
        /// <param name="limit">The limit.</param>
        public static ICollection<long> PrimesByAtkin(long limit)
        {
            if (limit < 2)
                return new HashSet<long>();
            else if (limit < 3)
                return new HashSet<long> { 2 };
            else if (limit < 5)
                return new HashSet<long> { 2, 3 };

            if (limit <= _cachedPrimes.Key)
                return _cachedPrimes.Value.Where(m => m <= limit).ToArray();

            var flip1 = new long[] { 1, 13, 17, 29, 37, 41, 49, 53 };
            var flip2 = new long[] { 7, 19, 31, 43 };
            var flip3 = new long[] { 11, 23, 47, 59 };
            var sqrt = Math.Sqrt(limit);
            var numbers = new HashSet<long>();
            Action<long> flipNumbers = (n) =>
            {
                if (!numbers.Add(n))
                    numbers.Remove(n);
            };

            for (long x = 1; x <= sqrt; x++)
            {
                for (long y = 1; y <= sqrt; y++)
                {
                    long n = (4 * (x * x)) + (y * y);
                    if (n <= limit && (flip1.Contains(n % 60)))
                        flipNumbers(n);

                    n = (3 * (x * x)) + (y * y);
                    if (n <= limit && (flip2.Contains(n % 60)))
                        flipNumbers(n);

                    n = (3 * (x * x)) - (y * y);
                    if (x > y && n <= limit && (flip3.Contains(n % 60)))
                        flipNumbers(n);
                }
            }

            var sortedNumbers = numbers.OrderBy(m => m);
            var primes = new HashSet<long> { 2, 3, 5 };

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

            _cachedPrimes = new KeyValuePair<long, ICollection<long>>(limit, primes);
            return primes;
        }
    }
}
