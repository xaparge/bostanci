﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Iksap.ItsmReporting.Web.Models
{
    public class project
    {
        public int id { get; set; }
        public string name { get; set; }
        public int parent_id { get; set; }
    }
}