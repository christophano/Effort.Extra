﻿
namespace Effort.Extra.Tests
{
    using System;
    using System.Collections.Generic;
    using Effort.DataLoaders;
    using Machine.Fakes;
    using Machine.Specifications;

    internal class ObjectTableDataLoader
    {
        public class Ctor
        {
            [Subject("ObjectTableDataLoader.Ctor")]
            public abstract class ctor_context : WithFakes
            {
                protected static Exception thrown_exception;
                protected static TableDescription table;
                protected static IEnumerable<Fella> entities;
                protected static ObjectTableDataLoader<Fella> subject;

                Because of = () => thrown_exception = Catch.Exception(
                    () => subject = new ObjectTableDataLoader<Fella>(table, entities));
            }

            public class when_table_is_null : ctor_context
            {
                Establish context = () =>
                {
                    table = null;
                    entities = new List<Fella>();
                };

                It throws_an_argument_null_exception =
                    () => thrown_exception.ShouldBeOfExactType<ArgumentNullException>();
            }

            public class when_entities_is_null : ctor_context
            {
                Establish context = () =>
                {
                    table = Builder.CreateTableDescription("Fella", typeof(Fella));
                    entities = null;
                };

                It throws_an_argument_null_exception =
                    () => thrown_exception.ShouldBeOfExactType<ArgumentNullException>();
            }

            public class when_arguments_are_valid : ctor_context
            {
                Establish context = () =>
                {
                    table = Builder.CreateTableDescription("Fella", typeof(Fella));
                    entities = new List<Fella>();
                };

                It does_not_throw_an_exception = () => thrown_exception.ShouldBeNull();
            }
        }
        
        public class CreateFormatter
        {
            [Subject("ObjectTableDataLoader.CreateFormatter")]
            public abstract class create_formatter_context : WithSubject<StubObjectTableDataLoader>
            {
                protected static Exception thrown_exception;
                protected static Func<Fella, object[]> formatter;

                Because of = () => thrown_exception = Catch.Exception(
                    () => formatter = Subject.CreateFormatter());
            }

            public class when_formatter_is_created : create_formatter_context
            {
                It does_not_throw_an_exception = () => thrown_exception.ShouldBeNull();

                It the_formatter_is_not_null = () => formatter.ShouldNotBeNull();

                It the_formatter_behaves_correctly = () =>
                {
                    var formatted = formatter(new Fella { Name = "Fred" });
                    formatted.Length.ShouldEqual(1);
                    formatted[0].ShouldEqual("Fred");
                };
            }
        }

        public class StubObjectTableDataLoader : ObjectTableDataLoader<Fella>
        {
            public StubObjectTableDataLoader() 
                : base(Builder.CreateTableDescription("Fella", typeof(Fella)), new List<Fella>())
            { }

            public new Func<Fella, object[]> CreateFormatter()
            {
                return base.CreateFormatter();
            }
        }
    }
}
