using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TinyBenchmark.Attributes;

namespace TinyBenchmark.Analysis
{
    internal class ArgumentsReferenceConstructor
    {
        internal ArgumentsReferenceConstructor()
        {
        }

        internal IReadOnlyList<ArgumentsReference> TryCreateArgumentsReferences(MethodInfo method)
        {
            var argumentsAttributes = method.GetCustomAttributes<ArgumentsAttribute>();
            if (argumentsAttributes?.Any() != true) return null;

            var argumentsReferences = argumentsAttributes.Select(a => CreateArgumentsReference(method, a)).ToList();
            return argumentsReferences.AsReadOnly();
        }

        internal ArgumentsReference CreateArgumentsReference(MethodInfo method, ArgumentsAttribute attr)
        {
            var expectedMethodArguments = method.GetParameters().ToList();
            var arguments = attr.Arguments;

            // Verify that arguments are allowed.
            if (expectedMethodArguments?.Any() != true && arguments.Any())
                throw ArgumentsNotAccepted(arguments);

            // Verify arguments count.
            if (expectedMethodArguments.Count != arguments.Count)
                throw ArgumentsCountDoNotMatch(arguments, expected: expectedMethodArguments.Count, actual: arguments.Count);

            var argumentsReference = new ArgumentsReference();

            // Verify argument type.
            for (int i = 0; i < expectedMethodArguments.Count; i++)
            {
                var arg = arguments[i];
                var methodArgument = expectedMethodArguments[i];

                if (arg == null)
                {
                    if (methodArgument.ParameterType.IsValueType)
                    {
                        var underlyingType = Nullable.GetUnderlyingType(methodArgument.ParameterType);
                        if (underlyingType == null)
                            throw ArgumentNullCannotBeAssignedToValueType(arguments, methodArgument);
                    }
                }
                else if (!methodArgument.ParameterType.IsAssignableFrom(arg.GetType()))
                    throw ArgumentTypesDoNotMatch(arguments, expected: methodArgument.GetType(), actual: arg.GetType());

                argumentsReference.Add(methodArgument.Name, arg);
            }

            return argumentsReference;

            // Local functions

            string ToString(IEnumerable<object> args) => string.Join(", ", args.Select(x => x?.ToString()));

            string GetExceptionPrefix(IEnumerable<object> args) =>
                $"Mismatching arguments on method {method.DeclaringType.Name}.{method.Name}, arguments: {ToString(args)}";

            InvalidOperationException ArgumentsNotAccepted(IEnumerable<object> args) => new InvalidOperationException(
                $"{GetExceptionPrefix(args)}: the method doesn't accept arguments.");

            InvalidOperationException ArgumentsCountDoNotMatch(IEnumerable<object> args, int expected, int actual) => new InvalidOperationException(
                $"{GetExceptionPrefix(args)}: expected {expected} arguments but {actual} arguments were provided.");

            InvalidOperationException ArgumentNullCannotBeAssignedToValueType(IEnumerable<object> args, ParameterInfo argumentInfo) => new InvalidOperationException(
                $"{GetExceptionPrefix(args)}: cannot assign null to the argument {argumentInfo.Name} of type {argumentInfo.ParameterType.Name}.");

            InvalidOperationException ArgumentTypesDoNotMatch(IEnumerable<object> args, Type expected, Type actual) => new InvalidOperationException(
                $"{GetExceptionPrefix(args)}: expected {expected.Name} but {actual.Name} was provided.");
        }
    }
}
