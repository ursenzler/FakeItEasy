﻿namespace FakeItEasy.Specs
{
    using System.Threading.Tasks;
    using FluentAssertions;
    using Xbehave;

    public interface IFooAsyncAwaitService
    {
        Task CommandAsync();

        Task<int> QueryAsync();
    }

    public class AsyncAwait
    {
        [Scenario]
        public void when_calling_defined_method_with_return_value(
            FooAsyncAwait foo,
            Task task)
        {
            "establish"
                .x(() =>
                    {
                        var service = A.Fake<IFooAsyncAwaitService>();
                        A.CallTo(() => service.QueryAsync()).Returns(Task.FromResult(9));
                        foo = new FooAsyncAwait(service);
                    });

            "when calling defined method with return value"
                .x(() => task = foo.QueryAsync());

            "it should return"
                .x(() => task.IsCompleted.Should().BeTrue());
        }

        [Scenario]
        public void when_calling_defined_void_method(
            FooAsyncAwait foo,
            Task task)
        {
            "establish"
                .x(() =>
                    {
                        var service = A.Fake<IFooAsyncAwaitService>();
                        A.CallTo(() => service.CommandAsync()).Returns(Task.FromResult<object>(null));
                        foo = new FooAsyncAwait(service);
                    });

            "when calling defined void method"
                .x(() => task = foo.CommandAsync());

            "it should return"
                .x(() => task.IsCompleted.Should().BeTrue());
        }

        [Scenario]
        public void when_calling_undefined_method_with_return_value(
            FooAsyncAwait foo,
            Task task)
        {
            "establish"
                .x(() => foo = new FooAsyncAwait(A.Fake<IFooAsyncAwaitService>()));

            "when calling undefined method with return value"
                .x(() => task = foo.QueryAsync());

            "it should return"
                .x(() => task.IsCompleted.Should().BeTrue());
        }

        [Scenario]
        public void when_calling_undefined_void_method(
            FooAsyncAwait foo,
            Task task)
        {
            "establish"
                .x(() => foo = new FooAsyncAwait(A.Fake<IFooAsyncAwaitService>()));

            "when calling undefined void method"
                .x(() => task = foo.CommandAsync());

            "it should return"
                .x(() => task.IsCompleted.Should().BeTrue());
        }
    }

    public class FooAsyncAwait
    {
        private readonly IFooAsyncAwaitService service;

        public FooAsyncAwait(IFooAsyncAwaitService service)
        {
            this.service = service;
        }

        public async Task CommandAsync()
        {
            await this.service.CommandAsync();
        }

        public async Task<int> QueryAsync()
        {
            return await this.service.QueryAsync();
        }
    }
}
