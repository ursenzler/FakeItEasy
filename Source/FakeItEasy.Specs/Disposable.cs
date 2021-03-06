﻿namespace FakeItEasy.Specs
{
    using System;
    using FluentAssertions;
    using Xbehave;

    public class Disposable
    {
        private static Exception exception;

        [Scenario]
        public void FakingDisposable(
            IDisposable fake)
        {
            "establish"
                .x(() =>
                    {
                        AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;

                        fake = A.Fake<SomeDisposable>();
                    })
                .Teardown(() => AppDomain.CurrentDomain.UnhandledException -= UnhandledExceptionHandler);

            "when faking a disposable class"
                .x(() =>
                    {
                        fake = null;
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                    });

            "it should not throw when finalized"
                .x(() => exception.Should().BeNull());
        }
        
        private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            exception = (Exception)e.ExceptionObject;
        }

        public abstract class SomeDisposable : IDisposable
        {
            ~SomeDisposable()
            {
                this.Dispose(false);
            }

            public void Dispose()
            {
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected abstract void Dispose(bool shouldCleanupManaged);
        }
    }
}