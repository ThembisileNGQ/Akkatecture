using System;
using Akka.Actor;
using Akka.Configuration;
using Akkatecture.Examples.UserAccount.Domain.UserAccountModel;
using Akkatecture.Examples.UserAccount.Domain.UserAccountModel.Commands;

namespace Akkatecture.Examples.UserAccount.Application
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Create actor system
            var config = ConfigurationFactory.ParseString(Config);
            var system = ActorSystem.Create("useraccount-example",config);

            //Create supervising aggregate manager for UserAccount aggregate root actors
            var aggregateManager = system.ActorOf(Props.Create(() => new UserAccountAggregateManager()));

            //Build create user account aggregate command
            var aggregateId = UserAccountId.With("useraccount-9a5e0078-a5cc-48f1-b38f-edfc6c25dee9");//UserAccountId.New;
            //var command = new CreateUserAccountCommand(aggregateId, "Foo");

            
            //Send command, this is equivalent to command.publish() in other cqrs frameworks
            //aggregateManager.Tell(command);

            //send a new command the change the user account name
            var newCommand = new UserAccountChangeNameCommand(aggregateId, "Foo6");
            aggregateManager.Tell(newCommand);
            

            for (var i = 0; i < 8; i++)
            {
                var loopCommand = new UserAccountChangeNameCommand(aggregateId, $"Foo{i}");
                aggregateManager.Tell(loopCommand);
            }
            
            var newCommand2 = new UserAccountChangeNameCommand(aggregateId, "FooEnd");
            aggregateManager.Tell(newCommand2);
            //block end of program
            Console.ReadLine();
        }

        public static string Config = @"
            akka.loglevel = ""DEBUG""
            akka.loglevel = ""DEBUG""
            akka.persistence.journal.plugin = ""akka.persistence.journal.postgresql""
            akka.persistence.journal.postgresql.class = ""Akka.Persistence.PostgreSql.Journal.PostgreSqlJournal, Akka.Persistence.PostgreSql""
            akka.persistence.journal.postgresql.plugin-dispatcher = ""akka.actor.default-dispatcher""
            akka.persistence.journal.postgresql.connection-string = ""Server=192.168.99.100;Port=30102;Database=platform;User Id=rudev;Password=masterkey1;""
            akka.persistence.journal.postgresql.connection-timeout = 30s
            akka.persistence.journal.postgresql.schema-name = ru
            akka.persistence.journal.postgresql.table-name = event_journal
            akka.persistence.journal.postgresql.auto-initialize = off
            akka.persistence.journal.postgresql.timestamp-provider = ""Akka.Persistence.Sql.Common.Journal.DefaultTimestampProvider, Akka.Persistence.Sql.Common""
            akka.persistence.journal.postgresql.metadata-table-name = metadata
            akka.persistence.journal.postgresql.stored-as = JSONB
            akka.persistence.snapshot-store.plugin = ""akka.persistence.snapshot-store.postgresql""
            akka.persistence.snapshot-store.postgresql.class = ""Akka.Persistence.PostgreSql.Snapshot.PostgreSqlSnapshotStore, Akka.Persistence.PostgreSql""
            akka.persistence.snapshot-store.postgresql.plugin-dispatcher = ""akka.actor.default-dispatcher""
            akka.persistence.snapshot-store.postgresql.connection-string = ""Server=192.168.99.100;Port=30102;Database=platform;User Id=rudev;Password=masterkey1;""
            akka.persistence.snapshot-store.postgresql.connection-timeout = 30s
            akka.persistence.snapshot-store.postgresql.schema-name = ru
            akka.persistence.snapshot-store.postgresql.table-name = snapshot_store
            akka.persistence.snapshot-store.postgresql.auto-initialize = off
            akka.persistence.snapshot-store.postgresql.stored-as = ""JSONB""";
    }
}
