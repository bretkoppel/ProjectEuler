using System;
using System.Runtime.Caching;
using System.Text;
using PostSharp.Aspects;
using PostSharp.Serialization;

namespace ProjectEuler
{
    [PSerializable]
    public sealed class CachingAttribute : OnMethodBoundaryAspect
    {
        public override void OnEntry(MethodExecutionArgs args)
        {
            var key = GetKey(args);
            var value = MemoryCache.Default.Get(key);
            if (value == null)
                args.MethodExecutionTag = key;
            else
            {
                args.ReturnValue = value;
                args.FlowBehavior = FlowBehavior.Return;
            }
        }

        public override void OnSuccess(MethodExecutionArgs args)
        {
            var cacheKey = (string)args.MethodExecutionTag;
            MemoryCache.Default[cacheKey] = args.ReturnValue;
        }

        private string GetKey(MethodExecutionArgs args)
        {
            var key = new StringBuilder();
            key.Append(args.Method.DeclaringType);
            key.Append("-");
            key.Append(args.Method.Name);
            key.Append("-");
            foreach (var arg in args.Arguments)
            {
                key.Append(arg);
                key.Append("|");
            }
            return key.ToString();
        }
    }
}