﻿namespace NServiceBus.RavenDB.Tests.Outbox
{
    using System;
    using System.Threading.Tasks;
    using NServiceBus.Extensibility;
    using NServiceBus.Outbox;
    using NServiceBus.Persistence.RavenDB;
    using NServiceBus.RavenDB.Outbox;
    using NUnit.Framework;
    using Raven.Client.Documents;

    [TestFixture]
    public class When_cleaning_outbox_messages : RavenDBPersistenceTestBase
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            new OutboxRecordsIndex().Execute(store);
        }

        [Test]
        public async Task Should_delete_all_OutboxRecords_that_have_been_dispatched()
        {
            // arrange
            var context = new ContextBag();
            var incomingMessage = SimulateIncomingMessage(context);
            var persister = new OutboxPersister("TestEndpoint", CreateTestSessionOpener());

            using (var transaction = await persister.BeginTransaction(context))
            {
                await persister.Store(new OutboxMessage("NotDispatched", new TransportOperation[0]), transaction, context);
                await transaction.Commit();
            }

            var outboxMessage = new OutboxMessage(incomingMessage.MessageId, new TransportOperation[0]);

            using (var transaction = await persister.BeginTransaction(context))
            {
                await persister.Store(outboxMessage, transaction, context);
                await transaction.Commit();
            }

            await persister.SetAsDispatched(outboxMessage.MessageId, context);
            await Task.Delay(TimeSpan.FromSeconds(1)); //Need to wait for dispatch logic to finish

            //WaitForUserToContinueTheTest(store);
            WaitForIndexing();

            var cleaner = new OutboxRecordsCleaner(store);

            // act
            await cleaner.RemoveEntriesOlderThan(DateTime.UtcNow.AddMinutes(1));

            // assert
            using (var session = store.OpenAsyncSession())
            {
                var outboxRecords = await session.Query<OutboxRecord>().ToListAsync();

                Assert.AreEqual(1, outboxRecords.Count);
                Assert.AreEqual("NotDispatched", outboxRecords[0].MessageId);
            }
        }
    }
}
