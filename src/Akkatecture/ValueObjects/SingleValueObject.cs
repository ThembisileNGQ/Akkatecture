using System;
using System.Collections.Generic;
using Akkatecture.Extensions;

namespace Akkatecture.ValueObjects
{
    public abstract class SingleValueObject<T> : ValueObject, IComparable, ISingleValueObject
        where T : IComparable, IComparable<T>
    {
        public T Value { get; }

        protected SingleValueObject(T value)
        {
            Value = value;
        }

        public int CompareTo(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                throw new ArgumentNullException(nameof(obj));
            }

            var other = obj as SingleValueObject<T>;
            if (other == null)
            {
                throw new ArgumentException($"Cannot compare '{GetType().PrettyPrint()}' and '{obj.GetType().PrettyPrint()}'");
            }

            return Value.CompareTo(other.Value);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString()
        {
            return ReferenceEquals(Value, null)
                ? string.Empty
                : Value.ToString();
        }

        public object GetValue()
        {
            return Value;
        }
    }
}