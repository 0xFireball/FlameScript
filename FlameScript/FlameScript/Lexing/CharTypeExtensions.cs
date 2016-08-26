using System;

namespace FlameScript.Lexing
{
    public static class CharTypeExtensions
    {
        /// <summary>
        /// Returns true, if ANY of the bits in flags is also set
        /// in this instance. (In contrast to HasFlag, which returns
        /// true if ALL bits are set).
        /// </summary>
        public static bool HasAnyFlag(this Enum e, Enum flag)
        {
            if (flag == null)
                throw new ArgumentNullException(nameof(flag));

            if (!e.GetType().IsEquivalentTo(flag.GetType()))
                throw new ArgumentException("The enum type does not match.", nameof(flag));

            return (Convert.ToUInt64(e) & Convert.ToUInt64(flag)) != 0;
        }
    }
}