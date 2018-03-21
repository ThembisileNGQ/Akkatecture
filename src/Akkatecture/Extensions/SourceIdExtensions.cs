using System;
using System.Collections.Generic;
using System.Text;
using Akkatecture.Core;

namespace Akkatecture.Extensions
{
    public static class SourceIdExtensions
    {
        public static bool IsNone(this ISourceId sourceId)
        {
            return string.IsNullOrEmpty(sourceId?.Value);
        }
    }
}
