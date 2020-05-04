using System;
using System.Web;
using System.Web.Mvc;
using System.Linq;

namespace Lakshmi.WEB.Filters
{
    public class MineSelfAttribute : AuthorizeAttribute
    {
        private string[] allowedUsers = new string[] { };
        private string[] allowedRoles = new string[] { };

        public MineSelfAttribute()
        { }
    }
}