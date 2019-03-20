using System;

namespace QNH.Overheid.KernRegister.Business.Utility
{
    public class InschrijvingRecord : IEquatable<InschrijvingRecord>
    {
        public string kvknummer { get; set; }

        public virtual bool Equals(InschrijvingRecord other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return
                string.Equals(kvknummer, other.kvknummer);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != this.GetType())
            {
                return false;
            }
            return Equals((InschrijvingRecord)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = kvknummer?.GetHashCode() ?? 0;
                return hashCode;
            }
        }
    }
}