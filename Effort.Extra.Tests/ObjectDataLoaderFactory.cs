
namespace Effort.Extra.Tests
{
    using System;
    using Effort.DataLoaders;
    using Machine.Fakes;
    using Machine.Specifications;

    internal class ObjectDataLoaderFactory
    {
        public class Ctor
        {
            [Subject("ObjectDataLoaderFactory.Ctor")]
            public abstract class ctor_context : WithFakes
            {
                protected static Exception thrown_exception;
                protected static Extra.ObjectData data;
                protected static Extra.ObjectDataLoaderFactory subject;

                Because of = () => thrown_exception = Catch.Exception(
                    () => subject = new Extra.ObjectDataLoaderFactory(data));
            }

            public class when_data_is_null : ctor_context
            {
                Establish context = () =>
                {
                    data = null;
                };

                It throws_an_argument_null_exception =
                    () => thrown_exception.ShouldBeOfExactType<ArgumentNullException>();
            }

            public class when_data_is_valid : ctor_context
            {
                Establish context = () =>
                {
                    data = new Extra.ObjectData();
                };

                It does_not_throw_an_exception = () => thrown_exception.ShouldBeNull();
            }
        }

        public class CreateTableDataLoader
        {
            [Subject("ObjectDataLoaderFactory.CreateTableDataLoader")]
            public abstract class create_table_data_loader : WithSubject<Extra.ObjectDataLoaderFactory>
            {
                protected static Extra.ObjectData data;
                protected static Exception thrown_exception;
                protected static TableDescription table;
                protected static ITableDataLoader result;

                Establish context = () =>
                {
                    data = new Extra.ObjectData();
                    Configure(x => x.For<Extra.ObjectData>().Use(() => data));
                };

                Because of = () => thrown_exception = Catch.Exception(
                    () => result = Subject.CreateTableDataLoader(table));
            }

            public class when_table_is_null : create_table_data_loader
            {
                Establish context = () =>
                {
                    table = null;
                };

                It throws_an_argument_null_exception = () =>
                    thrown_exception.ShouldBeOfExactType<ArgumentNullException>();
            }

            public class when_there_is_no_data_for_table : create_table_data_loader
            {
                Establish context = () =>
                {
                    table = Builder.CreateTableDescription("Fred", typeof(object));
                };

                It does_not_throw_an_exception = () => thrown_exception.ShouldBeNull();

                It the_result_is_empty_table_data_loader =
                    () => result.ShouldBeOfExactType<EmptyTableDataLoader>();
            }

            public class when_there_is_data_for_table : create_table_data_loader
            {
                Establish context = () =>
                {
                    data.Table<Person>().Add(new Person { Name = "Fred" });
                    table = Builder.CreateTableDescription(typeof(Person).Name, typeof(Person));
                };

                It does_not_throw_an_exception = () => thrown_exception.ShouldBeNull();

                It the_result_is_entity_table_data_loader =
                    () => result.ShouldBeOfExactType<ObjectTableDataLoader<Person>>();
            }
        }
    }
}
