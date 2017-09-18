using System;
using System.Threading.Tasks;

namespace ITLibrium.Hexagon.App.Decorators
{
    internal static class DecoratorExctensions
    {
        internal static async Task Execute(ExecutionOrder executionOrder, Func<Task> decorated, Func<Task> decorator)
        {
            switch (executionOrder)
            {
                case ExecutionOrder.BeforeDecorated:
                    await decorator().ConfigureAwait(false);
                    await decorated().ConfigureAwait(false);
                    break;
                case ExecutionOrder.AfterDecorated:
                    await decorated().ConfigureAwait(false);
                    await decorator().ConfigureAwait(false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(executionOrder));
            }
        }

        internal static async Task Execute<TIn>(TIn input, ExecutionOrder executionOrder, Func<TIn, Task> decorated, Func<TIn, Task> decorator)
        {
            switch (executionOrder)
            {
                case ExecutionOrder.BeforeDecorated:
                    await decorator(input).ConfigureAwait(false);
                    await decorated(input).ConfigureAwait(false);
                    break;
                case ExecutionOrder.AfterDecorated:
                    await decorated(input).ConfigureAwait(false);
                    await decorator(input).ConfigureAwait(false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(executionOrder));
            }
        }

        internal static async Task<TOut> Execute<TOut>(ExecutionOrder executionOrder, Func<Task<TOut>> decorated, Func<Task> decorator)
        {
            switch (executionOrder)
            {
                case ExecutionOrder.BeforeDecorated:
                    await decorator().ConfigureAwait(false);
                    return await decorated().ConfigureAwait(false);
                case ExecutionOrder.AfterDecorated:
                    TOut result = await decorated().ConfigureAwait(false);
                    await decorator().ConfigureAwait(false);
                    return result;
                default:
                    throw new ArgumentOutOfRangeException(nameof(executionOrder));
            }
        }

        internal static async Task<TOut> Execute<TIn, TOut>(TIn input, ExecutionOrder executionOrder, Func<TIn, Task<TOut>> decorated, Func<TIn, Task> decorator)
        {
            switch (executionOrder)
            {
                case ExecutionOrder.BeforeDecorated:
                    await decorator(input).ConfigureAwait(false);
                    return await decorated(input).ConfigureAwait(false);
                case ExecutionOrder.AfterDecorated:
                    TOut result = await decorated(input).ConfigureAwait(false);
                    await decorator(input).ConfigureAwait(false);
                    return result;
                default:
                    throw new ArgumentOutOfRangeException(nameof(executionOrder));
            }
        }
    }
}