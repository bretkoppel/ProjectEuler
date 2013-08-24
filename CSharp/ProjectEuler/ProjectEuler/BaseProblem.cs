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
            foreach (var methodInfo in methods.Where(methodInfo => methodInfo.Name != "Benchmarks" && Attribute.GetCustomAttribute((MemberInfo) methodInfo, typeof(TestAttribute)) != null))
            {
                Benchmark((Action)Delegate.CreateDelegate(typeof(Action), this, methodInfo.Name));
            }
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
    }
}