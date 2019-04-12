﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AgendaTech.WebAPI.Models
{
    public class Product
    {
        public string productName { get; set; }
        public int manufacturingYear { get; set; }
        public string brandName { get; set; }
    }
}