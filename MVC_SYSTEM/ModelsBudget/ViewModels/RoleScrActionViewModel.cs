using MVC_SYSTEM.App_LocalResources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using MVC_SYSTEM.ModelsBudget;

namespace MVC_SYSTEM.ModelsBudget.ViewModels
{
    public class RoleScrActionUpdate
    {
        public string roleid { get; set; }
        public string screencode { get; set; }
        public bool add { get; set; }
        public bool approve { get; set; }
        public bool check { get; set; }
        public bool delete { get; set; }
        public bool download { get; set; }
        public bool edit { get; set; }
        public bool print { get; set; }
        public bool reject { get; set; }
        public bool upload { get; set; }
        public bool view { get; set; }
    }
}