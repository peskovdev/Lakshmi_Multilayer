using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lakshmi.WEB.Models
{
    public class LikeViewModel
    {
        public bool Value { get; set; }
        public int Count { get; set; }
        public int IdPhoto { get; set; }
        public string IdName { get; set; }
    }
}