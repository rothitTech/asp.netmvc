using EDS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EDS.Helpers
{
    public class DistinctItemComparer : IEqualityComparer<CheckBoxes>
    {

        public bool Equals(CheckBoxes x, CheckBoxes y)
        {
            return x.Id == y.Id &&
                x.Text == y.Text;
        }

        public int GetHashCode(CheckBoxes obj)
        {
            return obj.Id.GetHashCode() ^
                obj.Text.GetHashCode();
        }
    }
}