using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace ProjectEuler
{
    public class BaseProblem
    {
        [Test]
        public void Benchmarks()
        {
            var methods = this.GetType().GetMethods();
            var testMethods = methods.Where(methodInfo => methodInfo.Name != "Benchmarks" && Attribute.GetCustomAttribute((MemberInfo) methodInfo, typeof(TestAttribute)) != null);
            if (testMethods.Any() == false)
                return;

            foreach (var methodInfo in testMethods)
            {
                var elapsed = Benchmark((Action)Delegate.CreateDelegate(typeof(Action), this, methodInfo.Name));
                Debug.WriteLine(methodInfo.Name + ": " + elapsed + "ms");
            }
        }

        private long Benchmark(Action action)
        {
            // warm up the benchmark
            action();
            GC.Collect();
            GC.WaitForPendingFinalizers();

            var watch = new Stopwatch();
            watch.Restart();
            for (int i = 0; i < 1000; i++)
            {
                action();
            }
            watch.Stop();
            return watch.ElapsedMilliseconds;
        }
    }
}