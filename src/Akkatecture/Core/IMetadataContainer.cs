using System;
using System.Collections.Generic;

namespace Akkatecture.Core
{
    public interface IMetadataContainer : IReadOnlyDictionary<string, string>
    {
        string GetMetadataValue(string key);
        T GetMetadataValue<T>(string key, Func<string, T> converter);
    }
}