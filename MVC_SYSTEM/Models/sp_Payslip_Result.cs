//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MVC_SYSTEM.Models
{
    using System;
    
    public partial class sp_Payslip_Result
    {
        public int fldID { get; set; }
        public string fldNopkj { get; set; }
        public string fldKodPkt { get; set; }
        public string fldKod { get; set; }
        public string fldKeterangan { get; set; }
        public Nullable<decimal> fldKuantiti { get; set; }
        public string fldUnit { get; set; }
        public Nullable<decimal> fldKadar { get; set; }
        public Nullable<int> fldGandaan { get; set; }
        public Nullable<decimal> fldJumlah { get; set; }
        public Nullable<int> fldBulan { get; set; }
        public Nullable<int> fldTahun { get; set; }
        public Nullable<int> fldNegaraID { get; set; }
        public Nullable<int> fldSyarikatID { get; set; }
        public Nullable<int> fldWilayahID { get; set; }
        public Nullable<int> fldLadangID { get; set; }
        public Nullable<int> fldFlag { get; set; }

        //added by syazana
        public string fldBasicIncomeType { get; set; }

        //added by Faeza on 23.11.2022
        public Nullable<int> fldFlagIncome { get; set; }
    }
}