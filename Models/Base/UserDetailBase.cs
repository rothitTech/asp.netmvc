﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EDS.Models
{
    public abstract class UserDetailBase
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
    }
}