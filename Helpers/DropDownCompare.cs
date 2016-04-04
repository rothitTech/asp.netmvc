using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using EDS.Models;

namespace EDS.Helpers
{
    public class DropDownCompare : IEqualityComparer<DropDownIdName>
    {
        public bool Equals(DropDownIdName x, DropDownIdName y)
        {
            if (x.Id == y.Id)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int GetHashCode(DropDownIdName obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}