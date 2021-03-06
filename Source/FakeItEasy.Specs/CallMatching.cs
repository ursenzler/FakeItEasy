﻿namespace FakeItEasy.Specs
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using FluentAssertions;
    using Xbehave;
    using Xunit;

    public class CallMatching
    {
        [Scenario]
        public void ParameterArrays(
            ITypeWithParameterArray fake)
        {
            "establish"
                .x(() => fake = A.Fake<ITypeWithParameterArray>());

            "when matching calls with parameter arrays"
                .x(() => fake.MethodWithParameterArray("foo", "bar", "baz"));

            "it should be able to match the call"
                .x(() => A.CallTo(() => fake.MethodWithParameterArray("foo", "bar", "baz")).MustHaveHappened());

            "it should be able to match the call with argument constraints"
                .x(() => A.CallTo(() => fake.MethodWithParameterArray(A<string>._, A<string>._, A<string>._)).MustHaveHappened());

            "it should be able to match the call mixing constraints and values"
                .x(() => A.CallTo(() => fake.MethodWithParameterArray(A<string>._, "bar", A<string>._)).MustHaveHappened());

            "it should be able to match using array syntax"
                .x(() => A.CallTo(() => fake.MethodWithParameterArray("foo", A<string[]>.That.IsSameSequenceAs(new[] { "bar", "baz" }))).MustHaveHappened());
        }

        public interface ITypeWithParameterArray
        {
            void MethodWithParameterArray(string arg, params string[] args);
        }
   
        [Scenario]
        public void FailingMatchOfNonGenericCalls(
            IFoo fake,
            Exception exception)
        {
            "establish"
                .x(() => fake = A.Fake<IFoo>());

            "when failing to match non generic calls"
                .x(() =>
                    {
                        fake.Bar(1);
                        fake.Bar(2);
                        exception = Record.Exception(() => A.CallTo(() => fake.Bar(3)).MustHaveHappened());
                    });

            "it should tell us that the call was not matched"
                .x(() => exception.Message.Should().Be(
                    @"

  Assertion failed for the following call:
    FakeItEasy.Specs.CallMatching+IFoo.Bar(3)
  Expected to find it at least once but found it #0 times among the calls:
    1: FakeItEasy.Specs.CallMatching+IFoo.Bar(baz: 1)
    2: FakeItEasy.Specs.CallMatching+IFoo.Bar(baz: 2)

"));
        }

        public interface IFoo
        {
            void Bar(int baz);
        }

        [Scenario]
        public void FailingMatchOfGenericCalls(
            IGenericFoo fake,
            Exception exception)
        {
            "establish"
                .x(() => fake = A.Fake<IGenericFoo>());

            "when failing to match generic calls"
                .x(() =>
                    {
                        fake.Bar(1, 2D);
                        fake.Bar(new Generic<bool, long>(), 3);
                        exception = Record.Exception(() => A.CallTo(() => fake.Bar(A<string>.Ignored, A<string>.Ignored)).MustHaveHappened());
                    });

            "it should tell us that the call was not matched"
                .x(() => exception.Message.Should().Be(
                    @"

  Assertion failed for the following call:
    FakeItEasy.Specs.CallMatching+IGenericFoo.Bar<System.String, System.String>(<Ignored>, <Ignored>)
  Expected to find it at least once but found it #0 times among the calls:
    1: FakeItEasy.Specs.CallMatching+IGenericFoo.Bar<System.Int32, System.Double>(baz1: 1, baz2: 2)
    2: FakeItEasy.Specs.CallMatching+IGenericFoo.Bar<FakeItEasy.Specs.CallMatching+Generic<System.Boolean, System.Int64>, System.Int32>(baz1: FakeItEasy.Specs.CallMatching+Generic`2[System.Boolean,System.Int64], baz2: 3)

"));
        }

        public interface IGenericFoo
        {
            void Bar<T1, T2>(T1 baz1, T2 baz2);
        }

        public class Generic<T1, T2>
        {
        }

        [Scenario]
        public void NoNonGeneriCalls(
            IBarFoo fake,
            Exception exception)
        {
            "establish"
                .x(() => fake = A.Fake<IBarFoo>());

            "when_no_non_generic_calls"
                .x(() => exception = Record.Exception(() => A.CallTo(() => fake.Bar(A<int>.Ignored)).MustHaveHappened()));

            "it should tell us that the call was not matched"
                .x(() => exception.Message.Should().Be(
                    @"

  Assertion failed for the following call:
    FakeItEasy.Specs.CallMatching+IBarFoo.Bar(<Ignored>)
  Expected to find it at least once but no calls were made to the fake object.

"));
        }

        public interface IBarFoo
        {
            void Bar(int baz);
        }

        [Scenario]
        public void NoGenericCalls(
            IGenericBarFoo fake,
            Exception exception)
        {
            "establish"
                .x(() => fake = A.Fake<IGenericBarFoo>());

            "when no generic calls"
                .x(() => exception = Record.Exception(() => A.CallTo(() => fake.Bar<Generic<string>>(A<Generic<string>>.Ignored)).MustHaveHappened()));

            "it should tell us that the call was not matched"
                .x(() => exception.Message.Should().Be(
                    @"

  Assertion failed for the following call:
    FakeItEasy.Specs.CallMatching+IGenericBarFoo.Bar<FakeItEasy.Specs.CallMatching+Generic<System.String>>(<Ignored>)
  Expected to find it at least once but no calls were made to the fake object.

"));
        }

        public interface IGenericBarFoo
        {
            void Bar<T>(T baz);
        }

        public class Generic<T>
        {
        }

        [Scenario]
        public void OutParameter(
            IDictionary<string, string> subject)
        {
            "establish"
                .x(() => subject = A.Fake<IDictionary<string, string>>());

            "when matching a call with an out parameter"
                .x(() =>
                    {
                        string outString = "a constraint string";
                        A.CallTo(() => subject.TryGetValue("any key", out outString))
                            .Returns(true);
                    });

            "it should match without regard to out parameter value"
                .x(() =>
                    {
                        string outString = "a different string";

                        subject.TryGetValue("any key", out outString)
                            .Should().BeTrue();
                    });

            "it should assign the constraint value to the out parameter"
                .x(() =>
                    {
                        string outString = "a different string";

                        subject.TryGetValue("any key", out outString);

                        outString.Should().Be("a constraint string");
                    });

        }

        [Scenario]
        public void FailingMatchOfOutParameter(
            IDictionary<string, string> subject)
        {
            Exception exception = null;

            "establish"
                .x(() => subject = A.Fake<IDictionary<string, string>>());

            "when failing to match a call with an out parameter"
                .x(() =>
                    {
                        string outString = null;

                        exception =
                            Record.Exception(
                                () => A.CallTo(() => subject.TryGetValue("any key", out outString))
                                    .MustHaveHappened());
                    });

            "it should tell us that the call was not matched"
                .x(() => exception.Message.Should().Be(
                    @"

  Assertion failed for the following call:
    System.Collections.Generic.IDictionary`2[[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089],[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]].TryGetValue(""any key"", <out parameter>)
  Expected to find it at least once but no calls were made to the fake object.

"));
        }

        [Scenario]
        public void RefParameter(
            IHaveInterestingParameters subject)
        {
            "establish"
                .x(() => subject = A.Fake<IHaveInterestingParameters>());

            "when matching a call with a ref parameter"
                .x(() =>
                    {
                        string refString = "a constraint string";
                        A.CallTo(() => subject.CheckYourReferences(ref refString))
                            .Returns(true);
                    });

            "it should match when ref parameter value matches"
                .x(() =>
                    {
                        string refString = "a constraint string";

                        subject.CheckYourReferences(ref refString)
                            .Should().BeTrue();
                    });

            "it should not match when ref parameter value does not match"
                .x(() =>
                    {
                        string refString = "a different string";

                        subject.CheckYourReferences(ref refString)
                            .Should().BeFalse();
                    });

            "it should assign the constraint value to the ref parameter"
                .x(() =>
                    {
                        string refString = "a constraint string";

                        subject.CheckYourReferences(ref refString);

                        refString.Should().Be("a constraint string");
                    });
        }

        public interface IHaveInterestingParameters
        {
            bool CheckYourReferences(ref string refString);
        }

        /// <summary>
        /// <see cref="OutAttribute"/> can be applied to parameters that are not
        /// <c>out</c> parameters.
        /// One example is the array parameter in <see cref="System.IO.Stream.Read"/>.
        /// Ensure that such parameters are not confused with <c>out</c> parameters.
        /// </summary>
        [Scenario]
        public void ParameterHavingAnOutAttribute(
             ITooHaveInterestingParameters subject)
        {
            "establish"
                .x(() => subject = A.Fake<ITooHaveInterestingParameters>());

            "when matching a call with a parameter having an out attribute"
                .x(() => A.CallTo(() => subject.Validate("a constraint string"))
                             .Returns(true));

            "it should match when ref parameter value matches"
                .x(() => subject.Validate("a constraint string")
                             .Should().BeTrue());

            "it should not match when ref parameter value does not match"
                .x(() => subject.Validate("a different string")
                             .Should().BeFalse());
        }
       
        public interface ITooHaveInterestingParameters
        {
            bool Validate([Out] string value);
        }
    }
}