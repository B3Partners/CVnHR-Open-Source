using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QNH.Overheid.KernRegister.Business.Model.nHibernate
{
    public class IndexableAttribute : Attribute
    {
        public string Name { get; set; }

        public IndexableAttribute()
        { }

        public IndexableAttribute(string name) 
        { 
            Name = name; 
        }
    }
}
