using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QNH.Overheid.KernRegister.Business.Utility
{
    public class ZipCodeRecord : IEquatable<ZipCodeRecord>
    {
        public string postcode { get; set; }

        public virtual bool Equals(ZipCodeRecord other)
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
                string.Equals(postcode, other.postcode);
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
            return Equals((ZipCodeRecord)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (postcode != null ? postcode.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
