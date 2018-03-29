namespace Akkatecture.TestHelpers.Akka
{
    public static class Configuration
    {
        public static string Config = @"akka {
                                          loglevel = ""DEBUG""
                                          stdout-loglevel = ""DEBUG""
                                          persistence {
                                              journal {
                                                  plugin = ""akka.persistence.journal.inmem""
                                                  inmem {
                                                      class = ""Akka.Persistence.Journal.MemoryJournal, Akka.Persistence""
                                                      plugin-dispatcher = ""akka.actor.default-dispatcher""
                                                  }
                                              }
                                               snapshot-store {
                                                  plugin = ""akka.persistence.snapshot-store.local""
                                                  local {
                                                      class = ""Akka.Persistence.Snapshot.LocalSnapshotStore, Akka.Persistence""
                                                      plugin-dispatcher = ""akka.persistence.dispatchers.default-plugin-dispatcher""
                                                      stream-dispatcher = ""akka.persistence.dispatchers.default-stream-dispatcher""
                                                      dir = ""snapshots""
                                                  }
                                              }
                                          }
                                       ";
    }
}