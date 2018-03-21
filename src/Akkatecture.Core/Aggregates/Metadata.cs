using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Akkatecture.Core;
using Akkatecture.Extensions;
using Newtonsoft.Json;

namespace Akkatecture.Aggregates
{
    public class Metadata : MetadataContainer, IMetadata
    {
        public static IMetadata Empty { get; } = new Metadata();

        public static IMetadata With(IEnumerable<KeyValuePair<string, string>> keyValuePairs)
        {
            return new Metadata(keyValuePairs);
        }

        public static IMetadata With(params KeyValuePair<string, string>[] keyValuePairs)
        {
            return new Metadata(keyValuePairs);
        }

        public static IMetadata With(IDictionary<string, string> keyValuePairs)
        {
            return new Metadata(keyValuePairs);
        }

        public ISourceId SourceId
        {
            get { return GetMetadataValue(MetadataKeys.SourceId, v => new SourceId(v)); }
            set { Add(MetadataKeys.SourceId, value.Value); }
        }

        [JsonIgnore]
        public string EventName
        {
            get { return GetMetadataValue(MetadataKeys.EventName); }
            set { Add(MetadataKeys.EventName, value); }
        }

        [JsonIgnore]
        public int EventVersion
        {
            get { return GetMetadataValue(MetadataKeys.EventVersion, int.Parse); }
            set { Add(MetadataKeys.EventVersion, value.ToString()); }
        }

        [JsonIgnore]
        public DateTimeOffset Timestamp
        {
            get { return GetMetadataValue(MetadataKeys.Timestamp, DateTimeOffset.Parse); }
            set { Add(MetadataKeys.Timestamp, value.ToString("O")); }
        }

        [JsonIgnore]
        public long TimestampEpoch
        {
            get
            {
                string timestampEpoch;
                return TryGetValue(MetadataKeys.TimestampEpoch, out timestampEpoch)
                    ? long.Parse(timestampEpoch)
                    : Timestamp.ToUnixTime();
            }
        }

        [JsonIgnore]
        public int AggregateSequenceNumber
        {
            get { return GetMetadataValue(MetadataKeys.AggregateSequenceNumber, int.Parse); }
            set { Add(MetadataKeys.AggregateSequenceNumber, value.ToString()); }
        }

        [JsonIgnore]
        public string AggregateId
        {
            get { return GetMetadataValue(MetadataKeys.AggregateId); }
            set { Add(MetadataKeys.AggregateId, value); }
        }

        [JsonIgnore]
        public IEventId EventId
        {
            get { return GetMetadataValue(MetadataKeys.EventId, Aggregates.EventId.With); }
            set { Add(MetadataKeys.EventId, value.Value); }
        }

        [JsonIgnore]
        public string AggregateName
        {
            get { return GetMetadataValue(MetadataKeys.AggregateName); }
            set { Add(MetadataKeys.AggregateName, value); }
        }

        public Metadata()
        {
            // Empty
        }

        public Metadata(IDictionary<string, string> keyValuePairs)
            : base(keyValuePairs)
        {
        }

        public Metadata(IEnumerable<KeyValuePair<string, string>> keyValuePairs)
            : base(keyValuePairs.ToDictionary(kv => kv.Key, kv => kv.Value))
        {
        }

        public Metadata(params KeyValuePair<string, string>[] keyValuePairs)
            : this((IEnumerable<KeyValuePair<string, string>>)keyValuePairs)
        {
        }

        public IMetadata CloneWith(params KeyValuePair<string, string>[] keyValuePairs)
        {
            return CloneWith((IEnumerable<KeyValuePair<string, string>>)keyValuePairs);
        }

        public IMetadata CloneWith(IEnumerable<KeyValuePair<string, string>> keyValuePairs)
        {
            var metadata = new Metadata(this);
            foreach (var kv in keyValuePairs)
            {
                if (metadata.ContainsKey(kv.Key))
                {
                    throw new ArgumentException($"Key '{kv.Key}' is already present!");
                }
                metadata[kv.Key] = kv.Value;
            }
            return metadata;
        }
    }
}
