using System;
using System.Collections;
using System.Collections.Generic;

namespace QNH.Overheid.KernRegister.Organization.Resources
{
    public class OrganizationResource : IEnumerable
    {
        public Dictionary<Organization, string> Values = new Dictionary<Organization, string>();

        public void Add(Organization org, string value)
        {
            Values.Add(org, value);
        }

        public string this[Organization org] => Values[org];

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Values.GetEnumerator();
        }
    }
}