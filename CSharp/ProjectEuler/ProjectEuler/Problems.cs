﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;

namespace ProjectEuler
{
    public class Problems
    {
        [Test]
        public void Benchmarks()
        {
            var methods = this.GetType().GetMethods();
            foreach (var methodInfo in methods.Where(methodInfo => methodInfo.Name != "Benchmarks" && Attribute.GetCustomAttribute(methodInfo, typeof (TestAttribute)) != null))
            {
                Benchmark((Action)Delegate.CreateDelegate(typeof(Action), this, methodInfo.Name));
            }
        }

        /// <summary>
        /// If we list all the natural numbers below 10 that are multiples of 3 or 5, we get 3, 5, 6 and 9. The sum of these multiples is 23.
        /// Find the sum of all the multiples of 3 or 5 below 1000.
        /// </summary>
        [Test]
        public void Problem1_Slow()
        {
            var sum = Enumerable.Range(3, 997).Where(m => m % 3 == 0 || m % 5 == 0).Sum();
            Assert.AreEqual(233168, sum);
        }

        /// <summary>
        /// If we list all the natural numbers below 10 that are multiples of 3 or 5, we get 3, 5, 6 and 9. The sum of these multiples is 23.
        /// Find the sum of all the multiples of 3 or 5 below 1000.
        /// </summary>
        [Test]
        public void Problem1_Faster()
        {
            var sum = Enumerable.Range(3, 997).Aggregate(0, (acc, item) =>
                {
                    if (item%3 == 0 || item%5 == 0)
                        acc += item;
                    return acc;
                });
            Assert.AreEqual(233168, sum);
        }

        /// <summary>
        /// If we list all the natural numbers below 10 that are multiples of 3 or 5, we get 3, 5, 6 and 9. The sum of these multiples is 23.
        /// Find the sum of all the multiples of 3 or 5 below 1000.
        /// </summary>
        [Test]
        public void Problem1_Fasterer()
        {
            var sum = Enumerable.Range(3, 997).Aggregate(0, (acc, item) => acc + ((item % 3 == 0 || item % 5 == 0) ? item : 0));
            Assert.AreEqual(233168, sum);
        }

        /// <summary>
        /// If we list all the natural numbers below 10 that are multiples of 3 or 5, we get 3, 5, 6 and 9. The sum of these multiples is 23.
        /// Find the sum of all the multiples of 3 or 5 below 1000.
        /// </summary>
        [Test]
        public void Problem1_Fast()
        {
            var sum = 0;
            var multiplier = 1;
            while (multiplier*3 < 1000)
            {
                sum += multiplier*3;
                var five = multiplier*5;
                if (five < 1000 && five%3 != 0)
                    sum += five;

                multiplier += 1;
            }
            Assert.AreEqual(233168, sum);
        }

        /// <summary>
        /// Each new term in the Fibonacci sequence is generated by adding the previous two terms. By starting with 1 and 2, the first 10 terms will be:
        /// 1, 2, 3, 5, 8, 13, 21, 34, 55, 89, ...
        /// By considering the terms in the Fibonacci sequence whose values do not exceed four million, find the sum of the even-valued terms.
        /// </summary>
        [Test]
        public void Problem2()
        {
            var n = new List<int> { 1, 2 };
            int x, y, z;
            do
            {
                x = n[n.Count - 2];
                y = n[n.Count - 1];
                z = x + y;
                n.Add(z);
            } while (z < 4000000);

            Assert.AreEqual(4613732, n.Where(m => m % 2 == 0).Sum());
        }

        /// <summary>
        /// The prime factors of 13195 are 5, 7, 13 and 29.
        /// What is the largest prime factor of the number 600851475143 ?
        /// </summary>
        [Test]
        public void Problem3()
        {
            Assert.AreEqual(29, PrimeFactorsByRationalSieve(13195).Max());
            Assert.AreEqual(6857, PrimeFactorsByRationalSieve(600851475143).Max());
        }

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

        [Test]
        public void VerifyBSmooth()
        {
            Assert.IsFalse(IsBSmooth(1620, 3));
            Assert.IsTrue(IsBSmooth(1620, 5));
            Assert.IsTrue(IsBSmooth(252, 7));
        }

        private void Benchmark(Action action)
        {
            var watch = new Stopwatch();
            watch.Restart();
            for (int i = 0; i < 500; i++)
            {
                action();
            }
            Debug.WriteLine(action.Method.Name + ":" + watch.ElapsedMilliseconds);
        }

        private IEnumerable<long> PrimeFactorsByRationalSieve(long value)
        {
            return PrimeFactorsByRationalSieve(value, (long)Math.Ceiling(Math.Sqrt(value)));
        }

        private IEnumerable<long> PrimeFactorsByRationalSieve(long value, long bound)
        {
            var primes = PrimesByAtkin(bound);
            var suitablePrimes = primes.Where(m => value % m == 0);
            if (suitablePrimes.Any())
            {
                var product = suitablePrimes.Aggregate((long)1, (accumulator, current) => accumulator * current);
                return suitablePrimes.Union(this.PrimeFactorsByRationalSieve(value / product, bound));
            }

            if (PrimesByAtkin(value).Contains(value))
                return new long[] { value };

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
            return suitablePrimes.Union(new[] { GreatestCommonDenominator(Math.Abs((int)leftSquare - (int)rightSquare), value), GreatestCommonDenominator((int)leftSquare + (int)rightSquare, value) });
        }

        private static long GreatestCommonDenominator(long x, long y)
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

        private static bool IsBSmooth(long value, long bound)
        {
            var primes = PrimesByAtkin(value);
            return !primes.Any(m => value % m == 0 && m > bound);
        }

        private static HashSet<long> PrimesByAtkin(long limit)
        {
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

            for (long x = 1; x <= sqrt; x++)
            {
                if (numbers.Contains(x))
                {
                    var x2 = x * x;
                    var multiplier = 1;
                    while (multiplier * x2 <= limit)
                    {
                        numbers.Remove(multiplier * x2);
                        multiplier += 1;
                    }
                }
            }

            var primes = new HashSet<long> { 2, 3, 5 };
            for (long x = 6; x <= limit; x++)
            {
                if (numbers.Contains(x))
                    primes.Add(x);
            }
            return primes;
        }
    }
}

