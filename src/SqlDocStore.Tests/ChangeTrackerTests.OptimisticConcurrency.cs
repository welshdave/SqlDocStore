namespace SqlDocStore.Tests
{
    using System;
    using System.Linq;
    using Documents;
    using Ploeh.AutoFixture;
    using Shouldly;
    using Xunit;

    public partial class ChangeTrackerTests
    {
        [Fact]
        public void optimistic_insert_change_should_be_tracked()
        {
            var changeTracker = GetChangeTracker(ConcurrencyModel.Optimistic);
            var fixture = new Fixture();
            var person = fixture.Create<Person>();
            var id = person.Id.GetHashCode();
            changeTracker.Insert(person);
            changeTracker.Inserts.First().ShouldBeSameAs(person);
        }

        [Fact]
        public void optimistic_insert_change_should_be_in_change_list()
        {
            var changeTracker = GetChangeTracker(ConcurrencyModel.Optimistic);
            var fixture = new Fixture();
            var person = fixture.Create<Person>();
            changeTracker.Insert(person);
            changeTracker.Changes.First().Document.ShouldBeSameAs(person);
        }

        [Fact]
        public void optimistic_delete_change_should_tracked()
        {
            var changeTracker = GetChangeTracker(ConcurrencyModel.Optimistic);
            var fixture = new Fixture();
            var person = fixture.Create<Person>();
            changeTracker.Insert(person);
            changeTracker.Delete(person);
            changeTracker.Deletions.First().ShouldBeSameAs(person);
        }

        [Fact]
        public void optimistic_delete_unknown_id_should_succeed()
        {
            var changeTracker = GetChangeTracker(ConcurrencyModel.Optimistic);
            var guidToDelete = Guid.NewGuid();
            changeTracker.DeleteById(guidToDelete);
            ((DeletedDocument)changeTracker.Changes.First().Document).Id.ShouldBe(guidToDelete);
        }

        [Fact]
        public void optimistic_saved_documents_should_not_be_seen_as_changes()
        {
            var changeTracker = GetChangeTracker(ConcurrencyModel.Pessimistic);
            var fixture = new Fixture();
            var person = fixture.Create<Person>();
            changeTracker.Insert(person);
            var person2 = fixture.Create<Person>();
            changeTracker.Insert(person2);
            changeTracker.MarkChangesSaved();
            changeTracker.Changes.ShouldBeEmpty();
        }

    }
}
