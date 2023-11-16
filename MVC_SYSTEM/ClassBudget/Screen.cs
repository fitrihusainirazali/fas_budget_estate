using MVC_SYSTEM.ModelsBudget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ClassBudget
{
    public class Screen
    {
        public List<bgt_ScrHdr> GetScrHdrs()
        {
            using (var db = new MVC_SYSTEM_ModelsBudget())
            {
                return db.bgt_ScrHdrs.ToList();
            }
        }

        public bgt_ScrHdr GetScrHdr(string scrHdrCode)
        {
            using (var db = new MVC_SYSTEM_ModelsBudget())
            {
                var scrHdr = db.bgt_ScrHdrs.FirstOrDefault(s => s.ScrHdrCode.Contains(scrHdrCode));
                if (scrHdr != null)
                    return scrHdr;
                else
                    return new bgt_ScrHdr();
            }
        }

        public List<bgt_Screen> GetScreens()
        {
            using (var db = new MVC_SYSTEM_ModelsBudget())
            {
                return db.bgt_Screens.ToList();
            }
        }

        public bgt_Screen GetScreen(string screenCode)
        {
            using (var db = new MVC_SYSTEM_ModelsBudget())
            {
                var screen = db.bgt_Screens.FirstOrDefault(s => s.ScrCode.Equals(screenCode));
                if (screen != null)
                    return screen;
                else
                    return new bgt_Screen();
            }
        }
    }
}