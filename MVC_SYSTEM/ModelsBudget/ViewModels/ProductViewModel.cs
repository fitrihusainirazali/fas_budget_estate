using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsBudget.ViewModels
{
    public class ProductListViewModel
    {
        public int Year { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public int? LocCode { get; set; }

        public decimal? PricePAU { get; set; }
    }
}