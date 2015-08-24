﻿namespace FakeItEasy.Specs
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using FluentAssertions;
    using Xbehave;

    public class Lazy
    {
        [Scenario]
        public void when_calling_a_method_that_returns_a_lazy(
            ILazyFactory fake,
            Lazy<IFoo> lazy)
        {
            "establish"._(() =>
            {
                fake = A.Fake<ILazyFactory>();
            });

            "when calling a method that returns a lazy"._(() =>
            {
                lazy = fake.Create();
            });

            "it should return a lazy"._(() =>
            {
                lazy.Should().NotBeNull();
            });

            "it should return a lazy whose value is a dummy"._(() =>
            {
                lazy.Value.Should().Be(FooFactory.Instance);
            });
        }
        
        public interface ILazyFactory
        {
            Lazy<IFoo> Create();
        }

        [SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces", Justification = "Required for testing.")]
        public interface IFoo
        {
        }

        public class FooFactory : DummyFactory<IFoo>, IFoo
        {
            private static IFoo instance = new FooFactory();

            public static IFoo Instance
            {
                get { return instance; }
            }

            protected override IFoo Create()
            {
                return instance;
            }
        }
    }
}