using System;

namespace Akkatecture.Core
{
    public class Quid
    {
        public Guid Guid { get; }
        public string Value { get; }
        public static Quid New => NewQuid();

        private Quid(string quid)
        {
            Value = quid;
            Guid = Decode(quid);

            if (From(Guid).Value != Value)
            {
                throw new InvalidOperationException($"Provided quid value '{quid}' is not a valid quid.");
            }
        }

        private Quid(Guid guid)
        {
            Value = Encode(guid);
            Guid = guid;
        }

        public Quid(Quid quid)
            : this(quid.Guid)
        {
        }

        public static Quid NewQuid()
        {
            return new Quid(Guid.NewGuid());
        }

        public static Quid From(Guid guid)
        {
            return new Quid(guid);
        }

        public static Quid From(string quidValue)
        {
            try
            {
                return new Quid(quidValue);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Value provided '{quidValue}' is not a valid .", ex);
            }
        }

        public static string Encode(string value)
        {
            var guid = new Guid(value);
            return Encode(guid);
        }

        public static string Encode(Guid guid)
        {
            var encoded = Convert
                .ToBase64String(guid.ToByteArray())
                .Replace("/", "_")
                .Replace("+", "-")
                .Substring(0, 22);

            return encoded;
        }

        public static Guid Decode(string value)
        {
            byte[] buffer = Convert.FromBase64String(PrepareQuidValueToBase64(value));

            return new Guid(buffer);
        }

        public static bool TryParse(string value, out Quid quid)
        {
            quid = null;

            if (value == null)
            {
                return false;
            }

            if (value.Length != 22)
            {
                return false;
            }

            if (!IsBase64(PrepareQuidValueToBase64(value)))
            {
                return false;
            }

            try
            {
                quid = new Quid(value);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool CanParse(string value)
        {
            Quid quid;
            return TryParse(value, out quid);
        }

        public override bool Equals(object obj)
        {
            if (obj is Quid)
            {
                return Guid.Equals(((Quid)obj).Guid);
            }

            if (obj is Guid)
            {
                return Guid.Equals((Guid)obj);
            }

            return false;
        }

        public static bool operator ==(Quid x, Quid y)
        {
            if ((object)x == null)
            {
                return (object)y == null;
            }

            if ((object)y == null)
            {
                return (object)x == null;
            }

            return x.Guid == y.Guid;
        }

        public static bool operator !=(Quid x, Quid y)
        {
            return !(x == y);
        }

        public static implicit operator string(Quid quid)
        {
            return quid.Value;
        }

        public override int GetHashCode()
        {
            return Guid.GetHashCode();
        }

        public override string ToString()
        {
            return Value;
        }

        private static string PrepareQuidValueToBase64(string quidValue)
        {
            return quidValue.Replace("_", "/").Replace("-", "+") + "==";
        }

        private static bool IsBase64(string base64String)
        {
            if (base64String.Contains(" ") ||
                base64String.Contains("\t") ||
                base64String.Contains("\r") ||
                base64String.Contains("\n"))
            {
                return false;
            }

            var index = base64String.Length - 1;

            if (base64String[index] == '=')
            {
                index--;
            }

            if (base64String[index] == '=')
            {
                index--;
            }

            for (var i = 0; i <= index; i++)
            {
                if (IsInvalid(base64String[i]))
                {
                    return false;
                }
            }

            try
            {
                Convert.FromBase64String(base64String);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static bool IsInvalid(char value)
        {
            var intValue = (Int32)value;

            // 1 - 9
            if (intValue >= 48 && intValue <= 57)
            {
                return false;
            }

            // A - Z
            if (intValue >= 65 && intValue <= 90)
            {
                return false;
            }

            // a - z
            if (intValue >= 97 && intValue <= 122)
            {
                return false;
            }

            // + or /
            return intValue != 43 && intValue != 47;
        }
    }
}