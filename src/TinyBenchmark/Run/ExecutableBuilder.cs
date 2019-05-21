using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using TinyBenchmark.Analysis;

namespace TinyBenchmark.Run
{
    internal class ExecutableBuilder
    {
        public Action Create(object instance, MethodInfo method, object[] methodParameters)
        {
            var instanceType = Expression.Parameter(instance.GetType());
            var parameters = methodParameters?.Select(Expression.Constant);

            var methodLambda = Expression.Lambda(Expression.Call(instanceType, method, parameters), instanceType);
            var targetInstance = Expression.Constant(instance);

            var targetInvocationExpression = Expression.Lambda<Action>(Expression.Invoke(methodLambda, targetInstance));
            var action = targetInvocationExpression.Compile();

            return action;
        }

        public FluentBuilder<TBenchmarksContainer> Using<TBenchmarksContainer>(TBenchmarksContainer benchmarksContainer) =>
            new FluentBuilder<TBenchmarksContainer>(this, benchmarksContainer);
    }

    internal class FluentBuilder<TBenchmarksContainer>
    {
        private readonly ExecutableBuilder _builder;
        private readonly TBenchmarksContainer _benchmarksContainer;
        private readonly List<InitReference> _initReferences = new List<InitReference>();
        private readonly List<WarmupReference> _warmupReferences = new List<WarmupReference>();
        private MethodInfo _benchmarkMethod;
        private object[] _benchmarkMethodParameters;

        public FluentBuilder(ExecutableBuilder builder, TBenchmarksContainer benchmarksContainer)
        {
            _builder = builder;
            _benchmarksContainer = benchmarksContainer;
        }

        internal FluentBuilder<TBenchmarksContainer> With(InitReference init)
        {
            if (init != null)
                _initReferences.Add(init);

            return this;
        }

        internal FluentBuilder<TBenchmarksContainer> With(params WarmupReference[] warmups) => this.With(warmups?.AsEnumerable());

        internal FluentBuilder<TBenchmarksContainer> With(IEnumerable<WarmupReference> warmups)
        {
            if (warmups?.Any() == true)
                _warmupReferences.AddRange(warmups);

            return this;
        }

        internal FluentBuilder<TBenchmarksContainer> For(MethodInfo method, object[] methodParameters)
        {
            _benchmarkMethod = method;
            _benchmarkMethodParameters = methodParameters;

            return this;
        }

        public Executable Create()
        {
            Action benchmark = _builder.Create(_benchmarksContainer, _benchmarkMethod, _benchmarkMethodParameters);

            List<Action> inits = null;
            if (_initReferences?.Any() == true)
            {
                inits = new List<Action>();
                foreach (var initReference in _initReferences)
                {
                    var init = _builder.Create(_benchmarksContainer, initReference.Method, null);
                    inits.Add(init);
                }
            }

            List<Action> warmups = null;
            if (_warmupReferences?.Any() == true)
            {
                warmups = new List<Action>();
                foreach (var warmupReference in _warmupReferences)
                {
                    var warmup = _builder.Create(_benchmarksContainer, warmupReference.Method, null);
                    warmups.Add(warmup);
                }
            }

            return new Executable(inits?.AsReadOnly(), warmups?.AsReadOnly(), benchmark);
        }
    }

    internal class Executable
    {
        private readonly IReadOnlyList<Action> _inits;
        private readonly IReadOnlyList<Action> _warmups;
        private readonly Action _benchmark;

        public Executable(
            IReadOnlyList<Action> inits,
            IReadOnlyList<Action> warmups,
            Action benchmark)
        {
            _inits = inits;
            _warmups = warmups;
            _benchmark = benchmark;
        }

        public void ExecuteInits()
        {
            if (_inits?.Any() != true)
                return;

            foreach (var init in _inits)
                init();
        }

        public void ExecuteWarmups()
        {
            if (_warmups?.Any() != true)
                return;

            foreach (var warmup in _warmups)
                warmup();
        }

        public void ExecuteBenchmark() => _benchmark();
    }
}
