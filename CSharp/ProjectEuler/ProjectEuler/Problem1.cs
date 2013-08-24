using System.Linq;
using NUnit.Framework;

namespace ProjectEuler
{
    /// <summary>
    /// If we list all the natural numbers below 10 that are multiples of 3 or 5, we get 3, 5, 6 and 9. The sum of these multiples is 23.
    /// Find the sum of all the multiples of 3 or 5 below 1000.
    /// </summary>
    public class Problem1 : BaseProblem
    {
        [Test]
        public void Problem1_Slow()
        {
            var sum = Enumerable.Range(3, 997).Where(m => m % 3 == 0 || m % 5 == 0).Sum();
            Assert.AreEqual(233168, sum);
        }

        [Test]
        public void Problem1_Faster()
        {
            var sum = Enumerable.Range(3, 997).Aggregate(0, (acc, item) =>
                {
                    if (item % 3 == 0 || item % 5 == 0)
                        acc += item;
                    return acc;
                });
            Assert.AreEqual(233168, sum);
        }

        [Test]
        public void Problem1_Fasterer()
        {
            var sum = Enumerable.Range(3, 997).Aggregate(0, (acc, item) => acc + ((item % 3 == 0 || item % 5 == 0) ? item : 0));
            Assert.AreEqual(233168, sum);
        }

        [Test]
        public void Problem1_Fast()
        {
            var sum = 0;
            var multiplier = 1;
            while (multiplier * 3 < 1000)
            {
                sum += multiplier * 3;
                var five = multiplier * 5;
                if (five < 1000 && five % 3 != 0)
                    sum += five;

                multiplier += 1;
            }
            Assert.AreEqual(233168, sum);
        }
    }
}