﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PWS_3.Models
{
    public class HATEOAS
    {
        public string Href { get; set; }
        public string Rel { get; set; }

        public HATEOAS(string href, string rel)
        {
            Href = href;
            Rel = rel;
        }
    }
}