using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Cronos;

namespace ScriptEngine.BuildIn.ShareCode.Extensions
{
    public static class CronosExtensions
    {
        public static TimeEnumerable GetOccurrencesLowGc(this CronExpression expression, DateTime from, DateTime to,
            bool fromInclusive = true, bool toInclusive = false)
        {
            return new TimeEnumerable(from, to, fromInclusive, toInclusive, expression);
        }
    }

    public readonly struct TimeEnumerable : IEnumerable<DateTime>
    {
        //follow the value of CronExpression.MaxYear
        private const int MaxYear = 2499;
        private readonly CronExpression expression;
        private readonly DateTime fromUtc;
        private readonly DateTime toUtc;
        private readonly bool fromInclusive;
        private readonly bool toInclusive;

        internal TimeEnumerable(DateTime fromUtc, DateTime toUtc, bool fromInclusive, bool toInclusive,
            CronExpression expression)
        {
            this.fromUtc = fromUtc;
            this.toUtc = toUtc;
            this.fromInclusive = fromInclusive;
            this.toInclusive = toInclusive;
            this.expression = expression;
            if (fromUtc > toUtc) ThrowFromShouldBeLessThanToException(nameof(fromUtc), nameof(toUtc));
            if (fromUtc.Year > MaxYear) ThrowDateTimeExceedsMaxException(nameof(fromUtc));
            if (toUtc.Year > MaxYear) ThrowDateTimeExceedsMaxException(nameof(toUtc));
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ThrowDateTimeExceedsMaxException(string paramName)
        {
            throw new ArgumentException($"The supplied DateTime is after the supported year of {MaxYear}.", paramName);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ThrowFromShouldBeLessThanToException(string fromName, string toName)
        {
            throw new ArgumentException(
                $"The value of the {fromName} argument should be less than the value of the {toName} argument.",
                fromName);
        }

        public TimeEnumerator GetEnumerator()
        {
            return new TimeEnumerator(expression, fromUtc, toUtc, fromInclusive, toInclusive);
        }

        IEnumerator<DateTime> IEnumerable<DateTime>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public struct TimeEnumerator : IEnumerator<DateTime>
    {
        private readonly CronExpression expression;
        private readonly DateTime fromUtc;
        private readonly DateTime toUtc;
        private readonly bool fromInclusive;
        private readonly bool toInclusive;
        private DateTime? current;
        private bool first;

        internal TimeEnumerator(CronExpression expression, DateTime fromUtc, DateTime toUtc, bool fromInclusive,
            bool toInclusive)
        {
            this.expression = expression;
            this.fromUtc = fromUtc;
            this.toUtc = toUtc;
            this.fromInclusive = fromInclusive;
            this.toInclusive = toInclusive;
            current = fromUtc;
            first = false;
        }

        public bool MoveNext()
        {
            if (current < toUtc || current == toUtc && toInclusive)
            {
                if (!first)
                {
                    current = expression.GetNextOccurrence(fromUtc, fromInclusive);
                    first = true;
                }
                else
                {
                    current = expression.GetNextOccurrence(current!.Value, inclusive: false);
                }

                return current < toUtc || current == toUtc && toInclusive;
            }
            else return false;
        }

        public void Reset()
        {
            throw new NotSupportedException("Reset is not supported.");
        }

        public DateTime Current
        {
            get
            {
                if (current == null) throw new InvalidOperationException("Can not be call this time");
                return current.Value;
            }
        }

        object IEnumerator.Current => Current;

        void System.IDisposable.Dispose()
        {
            //do nothing
        }
    }
}