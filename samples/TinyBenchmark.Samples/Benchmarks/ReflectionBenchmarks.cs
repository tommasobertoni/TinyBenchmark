using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using TinyBenchmark.Attributes;

namespace TinyBenchmark.Samples
{
    [BenchmarksContainer(Name = "Method invocations via reflection")]
    public class ReflectionBenchmarks
    {
        private const int _iterations = 20_000;

        private readonly string _arg = "aaa";

        private string Target(string a, string b)
        {
            return a;
        }

        public void Manual()
        {
            MethodInfoInit();
            var swInfo = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < _iterations; i++)
                MethodInfo();
            swInfo.Stop();

            ActionInit();
            var swAction = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < _iterations; i++)
                ActionBenchmark();
            swAction.Stop();
        }

        #region MethodInfo

        private MethodInfo _mInfo;

        public void MethodInfoInit()
        {
            _mInfo = typeof(ReflectionBenchmarks).GetMethod(nameof(Target), BindingFlags.NonPublic | BindingFlags.Instance);
        }

        [Benchmark(Baseline = true, Iterations = _iterations, Name = "Using MethodInfo")]
        [InitWith(nameof(MethodInfoInit))]
        public void MethodInfo()
        {
            _mInfo.Invoke(this, new object[] { _arg, _arg });
        }

        #endregion

        #region Action

        private Action _targetAction;

        public void ActionInit()
        {
            var mInfo = typeof(ReflectionBenchmarks).GetMethod(nameof(Target), BindingFlags.NonPublic | BindingFlags.Instance);

            var instanceType = Expression.Parameter(typeof(ReflectionBenchmarks));
            var aArg = Expression.Constant(_arg);
            var bArg = Expression.Constant(_arg);

            var methodLambda = Expression.Lambda(
                Expression.Call(
                    instanceType,
                    mInfo,
                    aArg,
                    bArg),
                instanceType);

            var targetInstance = Expression.Constant(this);

            var targetInvocationExpression = Expression.Lambda<Action>(
                Expression.Invoke(methodLambda, targetInstance));

            _targetAction = (Action)targetInvocationExpression.Compile();
        }

        [Benchmark(Name = "Using Expression", Iterations = _iterations)]
        [InitWith(nameof(ActionInit))]
        public void ActionBenchmark()
        {
            _targetAction();
        }

        #endregion
    }
}
