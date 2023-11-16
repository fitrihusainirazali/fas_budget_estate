using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsBudget
{
    public class bgtincomleviviewmodel
    {
        public int abel_id { get; set; }

        [StringLength(15)]
        public string abel_cost_center_code { get; set; }

        [StringLength(100)]
        public string abel_cost_center_desc { get; set; }

        public decimal? abel_grand_total_quantity { get; set; }

        public decimal? abel_grand_total_amount { get; set; }

        [StringLength(100)]
        public string abel_product_name { get; set; }

        public int? abel_budgeting_year { get; set; }
    }
}