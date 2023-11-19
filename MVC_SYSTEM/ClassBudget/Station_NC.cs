using MVC_SYSTEM.ModelsBudget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ClassBudget
{
    public class Station_NC
    {
        public List<bgt_Station_NC> GetStations()
        {
            using (var db = new MVC_SYSTEM_ModelsBudget())
            {
                return db.bgt_Station_NC.OrderBy(s => s.Code_LL).ToList();
            }
        }

        public bgt_Station_NC GetStationByCode(string code)
        {
            using (var db = new MVC_SYSTEM_ModelsBudget())
            {
                var station = db.bgt_Station_NC.FirstOrDefault(s => s.Code_LL.ToLower().Equals(code.ToLower()));
                if (station != null)
                {
                    return station;
                }
                return new bgt_Station_NC();
            }
        }
    }
}