
namespace Effort.Extra.Tests
{
    using System;
    using Machine.Fakes;
    using Machine.Specifications;

    internal class ObjectDataCollection
    {
        public class GetKeyForItem
        {
            [Subject("ObjectDataCollection.GetKeyForItem")]
            public abstract class get_key_for_item : WithSubject<StubObjectDataCollection>
            {
                protected static Exception thrown_exception;
                protected static Extra.ObjectData item;
                protected static Guid result;

                Because of = () => thrown_exception = Catch.Exception(
                    () => result = Subject.GetKeyForItem(item));
            }

            public class when_item_is_null : get_key_for_item
            {
                Establish context = () =>
                {
                    item = null;
                };

                It throws_an_argument_null_exception =
                    () => thrown_exception.ShouldBeOfExactType<ArgumentNullException>();
            }

            public class when_item_is_valid : get_key_for_item
            {
                Establish context = () =>
                {
                    item = new Extra.ObjectData();
                };

                It does_not_throw_an_exception = () => thrown_exception.ShouldBeNull();

                It the_result_is_the_correct_value = () => result.ShouldEqual(item.Identifier);
            }
        }
        
        public class AddOrUpdate
        {
            [Subject("ObjectDataCollection.AddOrUpdate")]
            public abstract class add_or_update_context : WithSubject<Extra.ObjectDataCollection>
            {
                protected static Exception thrown_exception;
                protected static Extra.ObjectData data;

                Because of = () => thrown_exception = Catch.Exception(() => Subject.AddOrUpdate(data));
            }

            public class when_data_is_not_already_in_the_collection : add_or_update_context
            {
                Establish context = () =>
                {
                    data = new Extra.ObjectData();
                };

                It does_not_throw_an_exception = () => thrown_exception.ShouldBeNull();

                It the_data_is_added_to_the_collection = () => Subject.Contains(data);
            }

            public class when_data_is_already_in_the_collection : add_or_update_context
            {
                static Extra.ObjectData existing;

                Establish context = () =>
                {
                    var guid = Guid.NewGuid();
                    existing = new StubObjectData(guid);
                    Subject.Add(existing);
                    data = new StubObjectData(guid);
                };

                It does_not_throw_an_exception = () => thrown_exception.ShouldBeNull();

                It the_data_is_added_to_the_collection = () => Subject.Contains(data);

                It the_existing_item_is_not_longer_in_the_collection =
                    () => Subject[existing.Identifier].ShouldNotBeTheSameAs(existing);
            }

            public class StubObjectData : Extra.ObjectData
            {
                private readonly Guid identifier;

                public StubObjectData(Guid identifier)
                {
                    this.identifier = identifier;
                }

                internal override Guid Identifier { get { return identifier; } }
            }
        }
        
        public class TryGetValue
        {
            [Subject("ObjectDataCollection.TryGetValue")]
            public abstract class try_get_value_context : WithSubject<Extra.ObjectDataCollection>
            {
                protected static Exception thrown_exception;
                protected static Guid key;
                protected static Extra.ObjectData data;
                protected static bool result;

                Because of = () => thrown_exception = Catch.Exception(
                    () => result = Subject.TryGetValue(key, out data));
            }

            public class when_key_does_not_exist : try_get_value_context
            {
                Establish context = () =>
                {
                    key = Guid.NewGuid();
                };

                It does_not_throw_an_exception = () => thrown_exception.ShouldBeNull();

                It the_data_should_be_null = () => data.ShouldBeNull();

                It the_result_is_false = () => result.ShouldBeFalse();
            }

            public class when_key_exists : try_get_value_context
            {
                Establish context = () =>
                {
                    var item = new Extra.ObjectData();
                    Subject.Add(item);
                    key = item.Identifier;
                };

                It does_not_throw_an_exception = () => thrown_exception.ShouldBeNull();

                It the_data_should_not_be_null = () => data.ShouldNotBeNull();

                It the_result_is_true = () => result.ShouldBeTrue();
            }
        }

        public class StubObjectDataCollection : Extra.ObjectDataCollection
        {
            public new Guid GetKeyForItem(Extra.ObjectData item)
            {
                return base.GetKeyForItem(item);
            }
        }
    }
}
