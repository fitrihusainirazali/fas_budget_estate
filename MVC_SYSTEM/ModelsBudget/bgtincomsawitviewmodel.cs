using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsBudget
{
    public class bgtincomesawitviewmodel
    {
        public int abio_id { get; set; }

        [StringLength(15)]
        public string abio_cost_center { get; set; }

        [StringLength(100)]
        public string abio_cost_center_desc { get; set; }

        public decimal? abio_grand_total_quantity { get; set; }

        public decimal? abio_grand_total_amount { get; set; }

        [StringLength(100)]
        public string abio_product_name { get; set; }

        public int? abio_budgeting_year { get; set; }
    }
}