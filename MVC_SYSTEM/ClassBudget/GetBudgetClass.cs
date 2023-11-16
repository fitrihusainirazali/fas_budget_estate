using MVC_SYSTEM.ModelsBudget;
using MVC_SYSTEM.MasterModels;
using System;
using System.Collections.Generic;
using System.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using System.Data.Entity;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using MVC_SYSTEM.ModelsBudget.ViewModels;

namespace MVC_SYSTEM.ClassBudget
{
    public class GetBudgetClass
    {
        MVC_SYSTEM_ModelsBudget db = new MVC_SYSTEM_ModelsBudget();
        MVC_SYSTEM_MasterModels dbc = new MVC_SYSTEM_MasterModels();
        MVC_SYSTEM_ModelsBudgetEst dbe = new MVC_SYSTEM_ModelsBudgetEst();

        public string MyNameFullName(int? userid)
        {
            tblUser User;
            string Name;
            Name = "";
            User = dbc.tblUsers.Where(u => u.fldUserID == userid).FirstOrDefault();
            if (User != null)
            {
                Name = User.fldUserFullName.ToString();
            }

            return Name;
        }
        public int GetBudgetYear()
        {
            int year = DateTime.Now.Year + 1;
            return year;
        }
        public tbl_Syarikat GetSyarikatBySyarikatId(int SyarikatID)
        {
            var syarikat = dbc.tbl_Syarikat.FirstOrDefault(x => x.fld_SyarikatID == SyarikatID);
            return syarikat;
        }
        public List<bgt_FeldaGrp> GetFeldaGrp(int SyarikatID)
        {
            int BudgetYear = GetBudgetYear();
            var result = db.bgt_FeldaGrp.Where(x => x.Co_Active == true && x.PriceYear == BudgetYear && x.fld_SyarikatID == SyarikatID).ToList();
            return result;
        }
        public List<bgt_income_sawit> GetCompanyList(string costcenter, int? year, int? NegaraID, int? SyarikatID)
        {
            var result = dbe.bgt_income_sawit.Where(x => x.abio_cost_center == costcenter && x.abio_budgeting_year == year && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).ToList();
            return result;
        }

        public DateTime GetDateTime()
        {
            DateTime serverTime = DateTime.Now;
            DateTime _localTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(serverTime, TimeZoneInfo.Local.Id, "Singapore Standard Time");
            return _localTime;
        }

        public string GetScreenName(string ScreenCode)
        {
            bgt_Screen Screen;
            string value = "";
            Screen = db.bgt_Screens.Where(x => x.ScrCode == ScreenCode).FirstOrDefault();
            if (Screen != null)
            {
                value = Screen.ScrNameLongDesc.ToString();
            }
            return value;
        }

        public string GetScreenID(string ScreenCode)
        {
            bgt_Screen Screen;
            string value = "";
            Screen = db.bgt_Screens.Where(x => x.ScrCode == ScreenCode).FirstOrDefault();
            if (Screen != null)
            {
                value = Screen.ScrID.ToString();
            }
            return value;
        }

        //atun tambah - susutnilai
        public List<MasterModels.tbl_SAPGLPUP> GetScreenGL(int id)
        {
            return dbc.tbl_SAPGLPUP.Where(x => x.bgt_ScreenGLs.Any(s => s.fld_ScrID == id)).OrderBy(x => x.fld_GLCode).ToList();

        }

        public int GetCountGL(int id)
        {
            var dataList = db.bgt_ScreenGLs.Where(x => x.fld_ScrID == id).ToList();
            int CountGL = dataList.Count();

            return CountGL;
        }

        public List<ModelsBudget.bgt_expenses_Depreciation> GetScreenDelGL(int year, string costcenter)
        {
            return dbe.bgt_expenses_Depreciation
                .Where(x => x.abed_budgeting_year == year && x.abed_cost_center == costcenter)
                .OrderBy(x => x.abed_gl_code)
                .ToList();
        }

        public List<bgt_expenses_Depreciation> GetExpDepRpt(string costcenter, int? year, int? NegaraID, int? SyarikatID)
        {
            var result = dbe.bgt_expenses_Depreciation.Where(x => x.abed_cost_center == costcenter && x.abed_budgeting_year == year && x.abed_NegaraID == NegaraID && x.abed_SyarikatID == SyarikatID && x.abed_Deleted == false).ToList();
            return result;
        }

        //atun tambah - gaji kakitangan
        public List<MasterModels.tbl_SAPGLPUP> GetScreenSalaryGL(int id)
        {
            return dbc.tbl_SAPGLPUP.Where(x => x.bgt_ScreenGLs.Any(s => s.fld_ScrID == id)).OrderBy(x => x.fld_GLCode).ToList();

        }

        public int GetCountSalaryGL(int id)
        {
            var dataList = db.bgt_ScreenGLs.Where(x => x.fld_ScrID == id).ToList();
            int CountGL = dataList.Count();

            return CountGL;
        }

        //atun tambah - Insurans
        public List<MasterModels.tbl_SAPGLPUP> GetScreenInsuransGL(int id)
        {
            return dbc.tbl_SAPGLPUP.Where(x => x.bgt_ScreenGLs.Any(s => s.fld_ScrID == id)).OrderBy(x => x.fld_GLCode).ToList();

        }

        public int GetCountInsuransGL(int id)
        {
            var dataList = db.bgt_ScreenGLs.Where(x => x.fld_ScrID == id).ToList();
            int CountGL = dataList.Count();

            return CountGL;
        }

        //atun tambah - IT
        public List<MasterModels.tbl_SAPGLPUP> GetScreenITGL(int id)
        {
            return dbc.tbl_SAPGLPUP.Where(x => x.bgt_ScreenGLs.Any(s => s.fld_ScrID == id)).OrderBy(x => x.fld_GLCode).ToList();

        }

        public int GetCountITGL(int id)
        {
            var dataList = db.bgt_ScreenGLs.Where(x => x.fld_ScrID == id).ToList();
            int CountGL = dataList.Count();

            return CountGL;
        }

        //atun tambah - CtrlHQ
        public List<MasterModels.tbl_SAPGLPUP> GetScreenChqGL(int id)
        {
            return dbc.tbl_SAPGLPUP.Where(x => x.bgt_ScreenGLs.Any(s => s.fld_ScrID == id)).OrderBy(x => x.fld_GLCode).ToList();

        }

        public int GetCountChqGL(int id)
        {
            var dataList = db.bgt_ScreenGLs.Where(x => x.fld_ScrID == id).ToList();
            int CountGL = dataList.Count();

            return CountGL;
        }
        //atun tambah - CorpServ
        public List<MasterModels.tbl_SAPGLPUP> GetScreenCorpSGL(int id)
        {
            return dbc.tbl_SAPGLPUP.Where(x => x.bgt_ScreenGLs.Any(s => s.fld_ScrID == id)).OrderBy(x => x.fld_GLCode).ToList();

        }

        public int GetCountCorpSGL(int id)
        {
            var dataList = db.bgt_ScreenGLs.Where(x => x.fld_ScrID == id).ToList();
            int CountGL = dataList.Count();

            return CountGL;
        }

        //atun tambah - Medical
        public List<MasterModels.tbl_SAPGLPUP> GetScreenMedGL(int id)
        {
            return dbc.tbl_SAPGLPUP.Where(x => x.bgt_ScreenGLs.Any(s => s.fld_ScrID == id)).OrderBy(x => x.fld_GLCode).ToList();

        }

        public int GetCountMedGL(int id)
        {
            var dataList = db.bgt_ScreenGLs.Where(x => x.fld_ScrID == id).ToList();
            int CountGL = dataList.Count();

            return CountGL;
        }

        //atun tambah - Windfall tax
        public List<MasterModels.tbl_SAPGLPUP> GetScreenWindGL(int id)
        {
            return dbc.tbl_SAPGLPUP.Where(x => x.bgt_ScreenGLs.Any(s => s.fld_ScrID == id)).OrderBy(x => x.fld_GLCode).ToList();

        }

        public int GetCountWindGL(int id)
        {
            var dataList = db.bgt_ScreenGLs.Where(x => x.fld_ScrID == id).ToList();
            int CountGL = dataList.Count();

            return CountGL;
        }

        public string GetWilayahName(int? wilayahId = null)//new sept
        {
            using (var db = new MVC_SYSTEM_MasterModels())
            {
                tbl_Wilayah wilayahdata;
                string wilayahname = "";
                wilayahdata = db.tbl_Wilayah.Where(x => x.fld_ID == wilayahId).FirstOrDefault();
                if (wilayahdata != null)
                {
                    wilayahname = wilayahdata.fld_WlyhName.ToString();
                }
                return wilayahname;
            }
        }

        public bool IsVehicleRegistered(string CostCenter, int BudgetYear)//new oct
        {
            bool lockdelete = false;
            var expvehdata = dbe.bgt_expenses_vehicle
                               .Where(w => w.abev_budgeting_year == BudgetYear && w.abev_cost_center_code.Equals(CostCenter))
                               .FirstOrDefault();

            if (expvehdata != null)
            {
                lockdelete = true;
            }
            return lockdelete;
        }

        public string GetStationDesc(string Code)
        {
            bgt_Station_NC StationNC;
            string value = "";
            StationNC = db.bgt_Station_NC.Where(x => x.Code_LL == Code).FirstOrDefault();
            if (StationNC != null)
            {
                value = StationNC.Station_Desc.ToString();
            }
            return value;
        }
        public decimal GetTotalIncome(string CostCenter, string year, string ScrCode)
        {
            decimal value = 0;

            var listincomesawit = GetIncomeSawitRecords(CostCenter, year).Where(w => w.ScreenCode == ScrCode);   //1
            var listincomebb = GetIncomeBijiBenihRecords(CostCenter, year).Where(w => w.ScreenCode == ScrCode);  //2
            var listincomeprd = GetIncomeProductRecords(CostCenter, year).Where(w => w.ScreenCode == ScrCode);   //3

            var sawit_benih = from sawit in listincomesawit
                              join benih in listincomebb
                                on sawit.CostCenter equals benih.CostCenter into sawitbenih
                              from sb in sawitbenih.DefaultIfEmpty()
                              select new
                              {
                                  AmountSawit = sawit?.AmountSawit ?? 0,
                                  AmountBijiBenih = sb?.AmountBijiBenih ?? 0,
                                  cc = sawit.CostCenter
                              };

            var benih_sawit = from benih in listincomebb
                              join sawit in listincomesawit
                                on benih.CostCenter equals sawit.CostCenter into benihsawit
                              from bs in benihsawit.DefaultIfEmpty()
                              select new
                              {
                                  AmountSawit = bs?.AmountSawit ?? 0,
                                  AmountBijiBenih = benih?.AmountBijiBenih ?? 0,
                                  cc = benih.CostCenter
                              };

            var listsawitbenih = sawit_benih.Union(benih_sawit);

            var prd_join_sawitbenih = from prd in listincomeprd
                                      join sb in listsawitbenih
                                        on prd.CostCenter equals sb.cc into prdsawitbenih
                                      from psb in prdsawitbenih.DefaultIfEmpty()
                                      select new
                                      {
                                          AmountSawit = psb?.AmountSawit ?? 0,
                                          AmountBijiBenih = psb?.AmountBijiBenih ?? 0,
                                          AmountProduk = prd?.AmountProduk ?? 0
                                      };

            var sawitbenih_join_prd = from sb in listsawitbenih
                                      join prd in listincomeprd
                                        on sb.cc equals prd.CostCenter into sawitbenihprd
                                      from sbp in sawitbenihprd.DefaultIfEmpty()
                                      select new
                                      {
                                          AmountSawit = sb?.AmountSawit ?? 0,
                                          AmountBijiBenih = sb?.AmountBijiBenih ?? 0,
                                          AmountProduk = sbp?.AmountProduk ?? 0
                                      };

            var alljoin = prd_join_sawitbenih.Union(sawitbenih_join_prd);

            foreach (var val in alljoin)
            {
                var sawit = val.AmountSawit;
                var bijibenih = val.AmountBijiBenih;
                var produk = val.AmountProduk;

                value = sawit + bijibenih + produk;
            }
            return value;
        }

        private List<IncomeViewModel> GetIncomeSawitRecords(string CostCenter, string BudgetYear)
        {
            var query = from p in dbe.bgt_income_sawit
                        where p.fld_Deleted == false && p.abio_cost_center == CostCenter && p.abio_budgeting_year.ToString() == BudgetYear
                        group p by new { p.abio_budgeting_year, p.abio_cost_center } into g
                        select new
                        {
                            cc = g.FirstOrDefault().abio_cost_center,
                            yr = g.FirstOrDefault().abio_budgeting_year,
                            TotalAmountSawit = g.Sum(s => s.abio_grand_total_amount)
                        };

            var query2 = query.AsQueryable();

            query2 = query2
                .OrderByDescending(q => q.yr)
                .ThenBy(q => q.cc);

            var query3 = (from q in query2.AsEnumerable()
                          select new IncomeViewModel
                          {
                              CostCenter = q.cc,
                              Year = q.yr,
                              AmountSawit = (decimal)q.TotalAmountSawit,
                              ScreenCode = "I1"
                          }).Distinct();

            return query3.ToList();
        }
        private List<IncomeViewModel> GetIncomeBijiBenihRecords(string CostCenter, string BudgetYear)
        {
            var query = from p in dbe.bgt_income_bijibenih
                        where p.fld_Deleted == false && p.abis_cost_center == CostCenter && p.abis_budgeting_year.ToString() == BudgetYear
                        group p by new { p.abis_budgeting_year, p.abis_cost_center } into g
                        select new
                        {
                            cc = g.FirstOrDefault().abis_cost_center,
                            yr = g.FirstOrDefault().abis_budgeting_year,
                            TotalAmountBijiBenih = g.Sum(s => s.abis_grand_total_amount)
                        };

            var query2 = query.AsQueryable();

            query2 = query2
                .OrderByDescending(q => q.yr)
                .ThenBy(q => q.cc);

            var query3 = (from q in query2.AsEnumerable()
                          select new IncomeViewModel
                          {
                              CostCenter = q.cc,
                              Year = q.yr,
                              AmountBijiBenih = (decimal)q.TotalAmountBijiBenih,
                              ScreenCode = "I2"
                          }).Distinct();

            return query3.ToList();
        }
        private List<IncomeViewModel> GetIncomeProductRecords(string CostCenter, string BudgetYear)
        {
            var query = from p in dbe.bgt_income_product
                        where p.fld_Deleted == false && p.abip_cost_center == CostCenter && p.abip_budgeting_year.ToString() == BudgetYear
                        group p by new { p.abip_budgeting_year, p.abip_cost_center } into g
                        select new
                        {
                            cc = g.FirstOrDefault().abip_cost_center,
                            yr = g.FirstOrDefault().abip_budgeting_year,
                            TotalAmountProduk = g.Sum(s => s.abip_grand_total_amount)
                        };

            var query2 = query.AsQueryable();

            query2 = query2
                .OrderByDescending(q => q.yr)
                .ThenBy(q => q.cc);

            var query3 = (from q in query2.AsEnumerable()
                          select new IncomeViewModel
                          {
                              CostCenter = q.cc,
                              Year = q.yr,
                              AmountProduk = (decimal)q.TotalAmountProduk,
                              ScreenCode = "I3"
                          }).Distinct();

            return query3.ToList();
        }


        //==================EXPENSES=================//

        public decimal GetTotalExpenses(string CostCenter, string year, string ScrCode)
        {
            decimal value = 0;

            var listexpcapital = GetExpCapital(CostCenter, year).Where(w => w.ScreenCode == ScrCode);
            var listexpctrlhq = GetExpCtrlHQRecords(CostCenter, year).Where(w => w.ScreenCode == ScrCode);
            var listexpdepreciation = GetExpDepreciation(CostCenter, year).Where(w => w.ScreenCode == ScrCode);
            var listexpgaji = GetExpGaji(CostCenter, year).Where(w => w.ScreenCode == ScrCode);
            var listexpinsurance = GetExpInsuranceRecords(CostCenter, year).Where(w => w.ScreenCode == ScrCode);
            var listexpit = GetExpIT(CostCenter, year).Where(w => w.ScreenCode == ScrCode);
            var listexpkhidcorp = GetExpKhidCorp(CostCenter, year).Where(w => w.ScreenCode == ScrCode);
            var listexplevi = GetExpLevi(CostCenter, year).Where(w => w.ScreenCode == ScrCode);
            var listexpmedical = GetExpMedical(CostCenter, year).Where(w => w.ScreenCode == ScrCode);
            var listexpothers = GetExpOthers(CostCenter, year).Where(w => w.ScreenCode == ScrCode);
            var listexppau = GetExpPau(CostCenter, year).Where(w => w.ScreenCode == ScrCode);
            var listexpsalary = GetExpSalary(CostCenter, year).Where(w => w.ScreenCode == ScrCode);
            var listexpvehicle = GetExpVehicle(CostCenter, year).Where(w => w.ScreenCode == ScrCode);
            var listexpwindfall = GetExpWindfall(CostCenter, year).Where(w => w.ScreenCode == ScrCode);

            var capital_hq = from capital in listexpcapital
                             join hq in listexpctrlhq
                               on capital.CostCenter equals hq.CostCenter into capitalhq
                             from ch in capitalhq.DefaultIfEmpty()
                             select new
                             {
                                 AmountExpCapital = capital?.AmountExpCapital ?? 0,
                                 AmountExpCtrlHQRecords = ch?.AmountExpCtrlHQRecords ?? 0,
                                 cc = capital.CostCenter
                             };

            var hq_capital = from hq in listexpctrlhq
                             join capital in listexpcapital
                               on hq.CostCenter equals capital.CostCenter into hqcapital
                             from hc in hqcapital.DefaultIfEmpty()
                             select new
                             {
                                 AmountExpCapital = hc?.AmountExpCapital ?? 0,
                                 AmountExpCtrlHQRecords = hq?.AmountExpCtrlHQRecords ?? 0,
                                 cc = hq.CostCenter
                             };
            var listcapitalhq = capital_hq.Union(hq_capital);//1

            var dep_gajis = from dep in listexpdepreciation
                            join gaji in listexpgaji
                              on dep.CostCenter equals gaji.CostCenter into depgajis
                            from dg in depgajis.DefaultIfEmpty()
                            select new
                            {
                                AmountExpDepreciation = dep?.AmountExpDepreciation ?? 0,
                                AmountExpGaji = dg?.AmountExpGaji ?? 0,
                                cc = dep.CostCenter
                            };

            var gajis_dep = from gaji in listexpgaji
                            join dep in listexpdepreciation
                              on gaji.CostCenter equals dep.CostCenter into gajisdep
                            from gd in gajisdep.DefaultIfEmpty()
                            select new
                            {
                                AmountExpDepreciation = gd?.AmountExpDepreciation ?? 0,
                                AmountExpGaji = gaji?.AmountExpGaji ?? 0,
                                cc = gaji.CostCenter
                            };

            var listdepgajis = dep_gajis.Union(gajis_dep);//2

            var ins_it = from ins in listexpinsurance
                         join it in listexpit
                           on ins.CostCenter equals it.CostCenter into insit
                         from ii in insit.DefaultIfEmpty()
                         select new
                         {

                             AmountExpInsuran = ins?.AmountExpInsuranceRecords ?? 0,
                             AmountExpIT = ii?.AmountExpIT ?? 0,
                             cc = ins.CostCenter
                         };

            var it_ins = from it in listexpit
                         join ins in listexpinsurance
                              on it.CostCenter equals ins.CostCenter into itins
                         from ii in itins.DefaultIfEmpty()
                         select new
                         {
                             AmountExpInsuran = ii?.AmountExpInsuranceRecords ?? 0,
                             AmountExpIT = it?.AmountExpIT ?? 0,
                             cc = it.CostCenter
                         };

            var listinsit = ins_it.Union(it_ins);//3

            var khid_levi = from khid in listexpkhidcorp
                            join levi in listexplevi
                              on khid.CostCenter equals levi.CostCenter into a
                            from b in a.DefaultIfEmpty()
                            select new
                            {
                                AmountExpKhidCorp = khid?.AmountExpKhidCorp ?? 0,
                                AmountExpLevi = b?.AmountExpLevi ?? 0,
                                cc = khid.CostCenter
                            };

            var levi_khid = from levi in listexplevi
                            join khid in listexpkhidcorp
                              on levi.CostCenter equals khid.CostCenter into a
                            from b in a.DefaultIfEmpty()
                            select new
                            {
                                AmountExpKhidCorp = b?.AmountExpKhidCorp ?? 0,
                                AmountExpLevi = levi?.AmountExpLevi ?? 0,
                                cc = levi.CostCenter
                            };

            var listkhidlevi = khid_levi.Union(levi_khid);//4

            var med_oth = from med in listexpmedical
                          join oth in listexpothers
                            on med.CostCenter equals oth.CostCenter into a
                          from b in a.DefaultIfEmpty()
                          select new
                          {
                              AmountExpMedical = med?.AmountExpMedical ?? 0,
                              AmountExpOthers = b?.AmountExpOthers ?? 0,
                              cc = med.CostCenter
                          };

            var oth_med = from oth in listexpothers
                          join med in listexpmedical
                              on oth.CostCenter equals med.CostCenter into a
                          from b in a.DefaultIfEmpty()
                          select new
                          {
                              AmountExpMedical = b?.AmountExpMedical ?? 0,
                              AmountExpOthers = oth?.AmountExpOthers ?? 0,
                              cc = oth.CostCenter
                          };

            var listmedoth = med_oth.Union(oth_med);//5

            var pau_sal = from pau in listexppau
                          join sal in listexpsalary
                            on pau.CostCenter equals sal.CostCenter into a
                          from b in a.DefaultIfEmpty()
                          select new
                          {
                              AmountExpPau = pau?.AmountExpPau ?? 0,
                              AmountGetExpSalary = b?.AmountGetExpSalary ?? 0,
                              cc = pau.CostCenter
                          };

            var sal_pau = from sal in listexpsalary
                          join pau in listexppau
                              on sal.CostCenter equals pau.CostCenter into a
                          from b in a.DefaultIfEmpty()
                          select new
                          {
                              AmountExpPau = b?.AmountExpPau ?? 0,
                              AmountGetExpSalary = sal?.AmountGetExpSalary ?? 0,
                              cc = sal.CostCenter
                          };

            var listpausal = pau_sal.Union(sal_pau);//6

            var veh_wind = from veh in listexpvehicle
                           join wind in listexpwindfall
                             on veh.CostCenter equals wind.CostCenter into a
                           from b in a.DefaultIfEmpty()
                           select new
                           {
                               AmountGetExpVehicle = veh?.AmountGetExpVehicle ?? 0,
                               AmountGetExpWindfall = b?.AmountGetExpWindfall ?? 0,
                               cc = veh.CostCenter
                           };

            var wind_veh = from wind in listexpwindfall
                           join veh in listexpvehicle
                              on wind.CostCenter equals veh.CostCenter into a
                           from b in a.DefaultIfEmpty()
                           select new
                           {
                               AmountGetExpVehicle = b?.AmountGetExpVehicle ?? 0,
                               AmountGetExpWindfall = wind?.AmountGetExpWindfall ?? 0,
                               cc = wind.CostCenter
                           };

            var listvehwind = veh_wind.Union(wind_veh);//7

            //1-2
            var list_capitalhq = from capitalhq in listcapitalhq
                                 join depgajis in listdepgajis
                                   on capitalhq.cc equals depgajis.cc into a
                                 from b in a.DefaultIfEmpty()
                                 select new
                                 {
                                     AmountExpCapital = capitalhq?.AmountExpCapital ?? 0,
                                     AmountExpCtrlHQRecords = capitalhq?.AmountExpCtrlHQRecords ?? 0,
                                     AmountExpDepreciation = b?.AmountExpDepreciation ?? 0,
                                     AmountExpGaji = b?.AmountExpGaji ?? 0,
                                     cc = capitalhq.cc
                                 };
            var list_depgajis = from depgajis in listdepgajis
                                join capitalhq in listcapitalhq
                                  on depgajis.cc equals capitalhq.cc into a
                                from b in a.DefaultIfEmpty()
                                select new
                                {
                                    AmountExpCapital = b?.AmountExpCapital ?? 0,
                                    AmountExpCtrlHQRecords = b?.AmountExpCtrlHQRecords ?? 0,
                                    AmountExpDepreciation = depgajis?.AmountExpDepreciation ?? 0,
                                    AmountExpGaji = depgajis?.AmountExpGaji ?? 0,
                                    cc = depgajis.cc
                                };

            var capitalhqdepgajis = list_capitalhq.Union(list_depgajis);//1-2

            //3-4
            var list_insit = from insit in listinsit
                             join khidlevi in listkhidlevi
                               on insit.cc equals khidlevi.cc into a
                             from b in a.DefaultIfEmpty()
                             select new
                             {
                                 AmountExpInsuran = insit?.AmountExpInsuran ?? 0,
                                 AmountExpIT = insit?.AmountExpIT ?? 0,
                                 AmountExpKhidCorp = b?.AmountExpKhidCorp ?? 0,
                                 AmountExpLevi = b?.AmountExpLevi ?? 0,
                                 cc = insit.cc
                             };
            var list_khidlevi = from khidlevi in listkhidlevi
                                join insit in listinsit
                                  on khidlevi.cc equals insit.cc into a
                                from b in a.DefaultIfEmpty()
                                select new
                                {
                                    AmountExpInsuran = b?.AmountExpInsuran ?? 0,
                                    AmountExpIT = b?.AmountExpIT ?? 0,
                                    AmountExpKhidCorp = khidlevi?.AmountExpKhidCorp ?? 0,
                                    AmountExpLevi = khidlevi?.AmountExpLevi ?? 0,
                                    cc = khidlevi.cc
                                };

            var insitkhidlevi = list_insit.Union(list_khidlevi);//3-4

            //5-6
            var list_medoth = from medoth in listmedoth
                              join pausal in listpausal
                                on medoth.cc equals pausal.cc into a
                              from b in a.DefaultIfEmpty()
                              select new
                              {
                                  AmountExpMedical = medoth?.AmountExpMedical ?? 0,
                                  AmountExpOthers = medoth?.AmountExpOthers ?? 0,
                                  AmountExpPau = b?.AmountExpPau ?? 0,
                                  AmountGetExpSalary = b?.AmountGetExpSalary ?? 0,
                                  cc = medoth.cc
                              };
            var list_pausal = from pausal in listpausal
                              join medoth in listmedoth
                                  on pausal.cc equals medoth.cc into a
                              from b in a.DefaultIfEmpty()
                              select new
                              {
                                  AmountExpMedical = b?.AmountExpMedical ?? 0,
                                  AmountExpOthers = b?.AmountExpOthers ?? 0,
                                  AmountExpPau = pausal?.AmountExpPau ?? 0,
                                  AmountGetExpSalary = pausal?.AmountGetExpSalary ?? 0,
                                  cc = pausal.cc
                              };

            var medothpausal = list_medoth.Union(list_pausal);//5-6

            //1-2-3-4
            var capitalhq_depgajis = from chdg in capitalhqdepgajis
                                     join iikl in insitkhidlevi
                                       on chdg.cc equals iikl.cc into a
                                     from b in a.DefaultIfEmpty()
                                     select new
                                     {
                                         AmountExpCapital = chdg?.AmountExpCapital ?? 0,
                                         AmountExpCtrlHQRecords = chdg?.AmountExpCtrlHQRecords ?? 0,
                                         AmountExpDepreciation = chdg?.AmountExpDepreciation ?? 0,
                                         AmountExpGaji = chdg?.AmountExpGaji ?? 0,
                                         AmountExpInsuran = b?.AmountExpInsuran ?? 0,
                                         AmountExpIT = b?.AmountExpIT ?? 0,
                                         AmountExpKhidCorp = b?.AmountExpKhidCorp ?? 0,
                                         AmountExpLevi = b?.AmountExpLevi ?? 0,
                                         cc = chdg.cc
                                     };

            var insit_khidlevi = from iikl in insitkhidlevi
                                 join chdg in capitalhqdepgajis
                                  on iikl.cc equals chdg.cc into a
                                 from b in a.DefaultIfEmpty()
                                 select new
                                 {
                                     AmountExpCapital = b?.AmountExpCapital ?? 0,
                                     AmountExpCtrlHQRecords = b?.AmountExpCtrlHQRecords ?? 0,
                                     AmountExpDepreciation = b?.AmountExpDepreciation ?? 0,
                                     AmountExpGaji = b?.AmountExpGaji ?? 0,
                                     AmountExpInsuran = iikl?.AmountExpInsuran ?? 0,
                                     AmountExpIT = iikl?.AmountExpIT ?? 0,
                                     AmountExpKhidCorp = iikl?.AmountExpKhidCorp ?? 0,
                                     AmountExpLevi = iikl?.AmountExpLevi ?? 0,
                                     cc = iikl.cc
                                 };

            var capitalhq_depgajis_insit_khidlevi = capitalhq_depgajis.Union(insit_khidlevi);//1-2-3-4

            //5-6-7
            var medoth_pausal = from mops in medothpausal
                                join vw in listvehwind
                                       on mops.cc equals vw.cc into a
                                from b in a.DefaultIfEmpty()
                                select new
                                {
                                    AmountExpMedical = mops?.AmountExpMedical ?? 0,
                                    AmountExpOthers = mops?.AmountExpOthers ?? 0,
                                    AmountExpPau = mops?.AmountExpPau ?? 0,
                                    AmountGetExpSalary = mops?.AmountGetExpSalary ?? 0,
                                    AmountGetExpVehicle = b?.AmountGetExpVehicle ?? 0,
                                    AmountGetExpWindfall = b?.AmountGetExpWindfall ?? 0,
                                    cc = mops.cc
                                };

            var list_vehwind = from vw in listvehwind
                               join mops in medothpausal
                                on vw.cc equals mops.cc into a
                               from b in a.DefaultIfEmpty()
                               select new
                               {
                                   AmountExpMedical = b?.AmountExpMedical ?? 0,
                                   AmountExpOthers = b?.AmountExpOthers ?? 0,
                                   AmountExpPau = b?.AmountExpPau ?? 0,
                                   AmountGetExpSalary = b?.AmountGetExpSalary ?? 0,
                                   AmountGetExpVehicle = vw?.AmountGetExpVehicle ?? 0,
                                   AmountGetExpWindfall = vw?.AmountGetExpWindfall ?? 0,
                                   cc = vw.cc
                               };

            var medoth_pausal_vehwind = medoth_pausal.Union(list_vehwind);//5-6-7

            //1-2-3-4_5-6-7
            var leftjoin = from left in capitalhq_depgajis_insit_khidlevi
                           join right in medoth_pausal_vehwind
                             on left.cc equals right.cc into a
                           from b in a.DefaultIfEmpty()
                           select new
                           {
                               AmountExpCapital = left?.AmountExpCapital ?? 0,
                               AmountExpCtrlHQRecords = left?.AmountExpCtrlHQRecords ?? 0,
                               AmountExpDepreciation = left?.AmountExpDepreciation ?? 0,
                               AmountExpGaji = left?.AmountExpGaji ?? 0,
                               AmountExpInsuran = left?.AmountExpInsuran ?? 0,
                               AmountExpIT = left?.AmountExpIT ?? 0,
                               AmountExpKhidCorp = left?.AmountExpKhidCorp ?? 0,
                               AmountExpLevi = left?.AmountExpLevi ?? 0,
                               AmountExpMedical = b?.AmountExpMedical ?? 0,
                               AmountExpOthers = b?.AmountExpOthers ?? 0,
                               AmountExpPau = b?.AmountExpPau ?? 0,
                               AmountGetExpSalary = b?.AmountGetExpSalary ?? 0,
                               AmountGetExpVehicle = b?.AmountGetExpVehicle ?? 0,
                               AmountGetExpWindfall = b?.AmountGetExpWindfall ?? 0
                           };

            var rightjoin = from right in medoth_pausal_vehwind
                            join left in capitalhq_depgajis_insit_khidlevi
                                  on right.cc equals left.cc into a
                            from b in a.DefaultIfEmpty()
                            select new
                            {
                                AmountExpCapital = b?.AmountExpCapital ?? 0,
                                AmountExpCtrlHQRecords = b?.AmountExpCtrlHQRecords ?? 0,
                                AmountExpDepreciation = b?.AmountExpDepreciation ?? 0,
                                AmountExpGaji = b?.AmountExpGaji ?? 0,
                                AmountExpInsuran = b?.AmountExpInsuran ?? 0,
                                AmountExpIT = b?.AmountExpIT ?? 0,
                                AmountExpKhidCorp = b?.AmountExpKhidCorp ?? 0,
                                AmountExpLevi = b?.AmountExpLevi ?? 0,
                                AmountExpMedical = right?.AmountExpMedical ?? 0,
                                AmountExpOthers = right?.AmountExpOthers ?? 0,
                                AmountExpPau = right?.AmountExpPau ?? 0,
                                AmountGetExpSalary = right?.AmountGetExpSalary ?? 0,
                                AmountGetExpVehicle = right?.AmountGetExpVehicle ?? 0,
                                AmountGetExpWindfall = right?.AmountGetExpWindfall ?? 0
                            };

            var outerjoin = leftjoin.Union(rightjoin);//1-2-3-4_5-6-7


            foreach (var val in outerjoin)
            {
                var capital = val.AmountExpCapital;
                var hq = val.AmountExpCtrlHQRecords;
                var dep = val.AmountExpDepreciation;
                var gaji = val.AmountExpGaji;
                var ins = val.AmountExpInsuran;
                var it = val.AmountExpIT;
                var khid = val.AmountExpKhidCorp;
                var levi = val.AmountExpLevi;
                var med = val.AmountExpMedical;
                var oth = val.AmountExpOthers;
                var pau = val.AmountExpPau;
                var sal = val.AmountGetExpSalary;
                var veh = val.AmountGetExpVehicle;
                var wind = val.AmountGetExpWindfall;

                value = capital + hq + dep + gaji + ins + it + khid + levi + med + oth + pau + sal + veh + wind;
            }
            return value;
        }
        private List<ExpensesViewModel> GetExpCapital(string CostCenter, string BudgetYear)
        {
            var query = from p in dbe.bgt_expenses_capitals
                        where p.abce_deleted == false && p.abce_cost_center_code == CostCenter && p.abce_budgeting_year.ToString() == BudgetYear
                        select p;

            var query2 = query.AsQueryable();

            query2 = query2
                .OrderByDescending(q => q.abce_budgeting_year)
                .ThenBy(q => q.abce_cost_center_code);

            var query3 = (from q in query2.AsEnumerable()
                          select new ExpensesViewModel
                          {
                              AmountExpCapital = (decimal)q.abce_total,
                              CostCenter = q.abce_cost_center_code,
                              ScreenCode = "E9"
                          }).Distinct();

            return query3.ToList();
        }
        private List<ExpensesViewModel> GetExpCtrlHQRecords(string CostCenter, string BudgetYear)
        {
            var query = from p in dbe.bgt_expenses_CtrlHQ
                        where p.abhq_Deleted == false && p.abhq_cost_center == CostCenter && p.abhq_budgeting_year.ToString() == BudgetYear
                        group p by new { p.abhq_budgeting_year, p.abhq_cost_center } into g
                        select new
                        {
                            cc = g.FirstOrDefault().abhq_cost_center,
                            yr = g.FirstOrDefault().abhq_budgeting_year,
                            TotalAmount = g.Sum(s => s.abhq_jumlah_RM),
                        };

            var query2 = query.AsQueryable();

            query2 = query2
                .OrderByDescending(q => q.yr)
                .ThenBy(q => q.cc);

            var query3 = (from q in query2.AsEnumerable()
                          select new ExpensesViewModel
                          {
                              AmountExpCtrlHQRecords = (decimal)q.TotalAmount,
                              CostCenter = q.cc,
                              ScreenCode = "E5"
                          }).Distinct();

            return query3.ToList();
        }
        private List<ExpensesViewModel> GetExpDepreciation(string CostCenter, string BudgetYear)
        {
            var query = from p in dbe.bgt_expenses_Depreciation
                        where p.abed_Deleted == false && p.abed_cost_center == CostCenter && p.abed_budgeting_year.ToString() == BudgetYear
                        group p by new { p.abed_budgeting_year, p.abed_cost_center } into g
                        select new
                        {
                            cc = g.FirstOrDefault().abed_cost_center,
                            yr = g.FirstOrDefault().abed_budgeting_year,
                            TotalAmount = g.Sum(s => s.abed_jumlah_RM)
                        };

            var query2 = query.AsQueryable();

            query2 = query2
                .OrderByDescending(q => q.yr)
                .ThenBy(q => q.cc);

            var query3 = (from q in query2.AsEnumerable()
                          select new ExpensesViewModel
                          {
                              AmountExpDepreciation = (decimal)q.TotalAmount,
                              CostCenter = q.cc,
                              ScreenCode = "E1"
                          }).Distinct();

            return query3.ToList();
        }
        private List<ExpensesViewModel> GetExpGaji(string CostCenter, string BudgetYear)
        {
            var query = from p in dbe.bgt_expenses_gajis
                        where p.abeg_Deleted == false && p.abeg_cost_center_code == CostCenter && p.abeg_budgeting_year.ToString() == BudgetYear
                        select p;
            
            var query2 = query.AsQueryable();

            query2 = query2
                .OrderByDescending(q => q.abeg_budgeting_year)
                .ThenBy(q => q.abeg_cost_center_code);

            var query3 = (from q in query2.AsEnumerable()
                          select new ExpensesViewModel
                          {
                              AmountExpGaji = (decimal)q.abeg_total,
                              CostCenter = q.abeg_cost_center_code,
                              ScreenCode = "E12"
                          }).Distinct();

            return query3.ToList();
        }
        private List<ExpensesViewModel> GetExpInsuranceRecords(string CostCenter, string BudgetYear)
        {
            var query = from p in dbe.bgt_expenses_Insurans
                        where p.abei_Deleted == false
                        group p by new { p.abei_budgeting_year, p.abei_cost_center } into g
                        select new
                        {
                            cc = g.FirstOrDefault().abei_cost_center,
                            yr = g.FirstOrDefault().abei_budgeting_year,
                            TotalAmount = g.Sum(s => s.abei_jumlah_RM)
                        };

            if (!string.IsNullOrEmpty(CostCenter))
                query = query.Where(q => q.cc.ToString() == CostCenter);
            if (!string.IsNullOrEmpty(BudgetYear))
                query = query.Where(q => q.yr.ToString() == BudgetYear);

            var query2 = query.AsQueryable();

            query2 = query2
                .OrderByDescending(q => q.yr)
                .ThenBy(q => q.cc);

            var query3 = (from q in query2.AsEnumerable()
                          select new ExpensesViewModel
                          {
                              AmountExpInsuranceRecords = (decimal)q.TotalAmount,
                              CostCenter = q.cc,
                              ScreenCode = "E3"
                          }).Distinct();

            return query3.ToList();
        }
        private List<ExpensesViewModel> GetExpIT(string CostCenter, string BudgetYear)
        {
            var query = from p in dbe.bgt_expenses_IT
                        where p.abet_Deleted == false
                        group p by new { p.abet_budgeting_year, p.abet_cost_center } into g
                        select new
                        {
                            cc = g.FirstOrDefault().abet_cost_center,
                            yr = g.FirstOrDefault().abet_budgeting_year,
                            TotalAmount = g.Sum(s => s.abet_jumlah_RM)
                        };

            if (!string.IsNullOrEmpty(CostCenter))
                query = query.Where(q => q.cc.ToString() == CostCenter);
            if (!string.IsNullOrEmpty(BudgetYear))
                query = query.Where(q => q.yr.ToString() == BudgetYear);

            var query2 = query.AsQueryable();

            query2 = query2
                .OrderByDescending(q => q.yr)
                .ThenBy(q => q.cc);

            var query3 = (from q in query2.AsEnumerable()
                          select new ExpensesViewModel
                          {
                              AmountExpIT = (decimal)q.TotalAmount,
                              CostCenter = q.cc,
                              ScreenCode = "E4"
                          }).Distinct();

            return query3.ToList();
        }
        private List<ExpensesViewModel> GetExpKhidCorp(string CostCenter, string BudgetYear)
        {
            var query = from p in dbe.bgt_expenses_KhidCor
                        where p.abcs_Deleted == false
                        group p by new { p.abcs_budgeting_year, p.abcs_cost_center } into g
                        select new
                        {
                            cc = g.FirstOrDefault().abcs_cost_center,
                            yr = g.FirstOrDefault().abcs_budgeting_year,
                            TotalAmount = g.Sum(s => s.abcs_jumlah_RM)
                        };

            if (!string.IsNullOrEmpty(CostCenter))
                query = query.Where(q => q.cc.ToString() == CostCenter);
            if (!string.IsNullOrEmpty(BudgetYear))
                query = query.Where(q => q.yr.ToString() == BudgetYear);

            var query2 = query.AsQueryable();

            query2 = query2
                .OrderByDescending(q => q.yr)
                .ThenBy(q => q.cc);

            var query3 = (from q in query2.AsEnumerable()
                          select new ExpensesViewModel
                          {
                              AmountExpKhidCorp = (decimal)q.TotalAmount,
                              CostCenter = q.cc,
                              ScreenCode = "E6"
                          }).Distinct();

            return query3.ToList();
        }
        private List<ExpensesViewModel> GetExpLevi(string CostCenter, string BudgetYear)
        {
            var query = from p in dbe.bgt_expenses_levi
                        where p.fld_Deleted == false
                        group p by new { p.abel_budgeting_year, p.abel_cost_center_code } into g
                        select new
                        {
                            cc = g.FirstOrDefault().abel_cost_center_code,
                            yr = g.FirstOrDefault().abel_budgeting_year,
                            TotalAmount = g.Sum(s => s.abel_grand_total_amount)
                        };

            if (!string.IsNullOrEmpty(CostCenter))
                query = query.Where(q => q.cc.ToString() == CostCenter);
            if (!string.IsNullOrEmpty(BudgetYear))
                query = query.Where(q => q.yr.ToString() == BudgetYear);

            var query2 = query.AsQueryable();

            query2 = query2
                .OrderByDescending(q => q.yr)
                .ThenBy(q => q.cc);

            var query3 = (from q in query2.AsEnumerable()
                          select new ExpensesViewModel
                          {
                              AmountExpLevi = (decimal)q.TotalAmount,
                              CostCenter = q.cc,
                              ScreenCode = "E11"
                          }).Distinct();

            return query3.ToList();
        }
        private List<ExpensesViewModel> GetExpMedical(string CostCenter, string BudgetYear)
        {
            var query = from p in dbe.bgt_expenses_Medical
                        where p.abem_Deleted == false
                        group p by new { p.abem_budgeting_year, p.abem_cost_center } into g
                        select new
                        {
                            cc = g.FirstOrDefault().abem_cost_center,
                            yr = g.FirstOrDefault().abem_budgeting_year,
                            TotalAmount = g.Sum(s => s.abem_jumlah_RM)
                        };

            if (!string.IsNullOrEmpty(CostCenter))
                query = query.Where(q => q.cc.ToString() == CostCenter);
            if (!string.IsNullOrEmpty(BudgetYear))
                query = query.Where(q => q.yr.ToString() == BudgetYear);

            var query2 = query.AsQueryable();

            query2 = query2
                .OrderByDescending(q => q.yr)
                .ThenBy(q => q.cc);

            var query3 = (from q in query2.AsEnumerable()
                          select new ExpensesViewModel
                          {
                              AmountExpMedical = (decimal)q.TotalAmount,
                              CostCenter = q.cc,
                              ScreenCode = "E7"
                          }).Distinct();

            return query3.ToList();
        }
        private List<ExpensesViewModel> GetExpOthers(string CostCenter, string BudgetYear)
        {
            var query = from p in dbe.bgt_expenses_others
                        where p.abeo_deleted == false && p.abeo_cost_center_code == CostCenter && p.abeo_budgeting_year.ToString() == BudgetYear
                        select p;

            var query2 = query.AsQueryable();

            query2 = query2
                .OrderByDescending(q => q.abeo_budgeting_year)
                .ThenBy(q => q.abeo_cost_center_code);

            var query3 = (from q in query2.AsEnumerable()
                          select new ExpensesViewModel
                          {
                              AmountExpOthers = (decimal)q.abeo_total,
                              CostCenter = q.abeo_cost_center_code,
                              ScreenCode = "E15"
                          }).Distinct();

            return query3.ToList();
        }
        private List<ExpensesViewModel> GetExpPau(string CostCenter, string BudgetYear)
        {
            var query = from p in dbe.bgt_expenses_paus
                        where p.abep_deleted == false && p.abep_cost_center_code == CostCenter && p.abep_budgeting_year.ToString() == BudgetYear
                        select p;

            var query2 = query.AsQueryable();

            query2 = query2
                .OrderByDescending(q => q.abep_budgeting_year)
                .ThenBy(q => q.abep_cost_center_code);

            var query3 = (from q in query2.AsEnumerable()
                          select new ExpensesViewModel
                          {
                              AmountExpPau = (decimal)q.abep_total,
                              CostCenter = q.abep_cost_center_code,
                              ScreenCode = "E10"
                          }).Distinct();

            return query3.ToList();
        }
        private List<ExpensesViewModel> GetExpSalary(string CostCenter, string BudgetYear)
        {
            var query = from p in dbe.bgt_expenses_Salary
                        where p.abes_Deleted == false
                        group p by new { p.abes_budgeting_year, p.abes_cost_center } into g
                        select new
                        {
                            cc = g.FirstOrDefault().abes_cost_center,
                            yr = g.FirstOrDefault().abes_budgeting_year,
                            TotalAmount = g.Sum(s => s.abes_jumlah_RM)
                        };

            if (!string.IsNullOrEmpty(CostCenter))
                query = query.Where(q => q.cc.ToString() == CostCenter);
            if (!string.IsNullOrEmpty(BudgetYear))
                query = query.Where(q => q.yr.ToString() == BudgetYear);

            var query2 = query.AsQueryable();

            query2 = query2
                .OrderByDescending(q => q.yr)
                .ThenBy(q => q.cc);

            var query3 = (from q in query2.AsEnumerable()
                          select new ExpensesViewModel
                          {
                              AmountGetExpSalary = (decimal)q.TotalAmount,
                              CostCenter = q.cc,
                              ScreenCode = "E2"
                          }).Distinct();

            return query3.ToList();
        }
        private List<ExpensesViewModel> GetExpVehicle(string CostCenter, string BudgetYear)
        {
            var query = from p in dbe.bgt_expenses_vehicle
                        where p.abev_deleted == false && p.abev_cost_center_code == CostCenter && p.abev_budgeting_year.ToString() == BudgetYear
                        select p;

            var query2 = query.AsQueryable();

            query2 = query2
                .OrderByDescending(q => q.abev_budgeting_year)
                .ThenBy(q => q.abev_cost_center_code);

            var query3 = (from q in query2.AsEnumerable()
                          select new ExpensesViewModel
                          {
                              AmountGetExpVehicle = (decimal)q.abev_total,
                              CostCenter = q.abev_cost_center_code,
                              ScreenCode = "E14"
                          }).Distinct();

            return query3.ToList();
        }
        private List<ExpensesViewModel> GetExpWindfall(string CostCenter, string BudgetYear)
        {
            var query = from p in dbe.bgt_expenses_Windfall
                        where p.abew_Deleted == false
                        group p by new { p.abew_budgeting_year, p.abew_cost_center } into g
                        select new
                        {
                            cc = g.FirstOrDefault().abew_cost_center,
                            yr = g.FirstOrDefault().abew_budgeting_year,
                            TotalAmount = g.Sum(s => s.abew_jumlah_RM)
                        };

            if (!string.IsNullOrEmpty(CostCenter))
                query = query.Where(q => q.cc.ToString() == CostCenter);
            if (!string.IsNullOrEmpty(BudgetYear))
                query = query.Where(q => q.yr.ToString() == BudgetYear);

            var query2 = query.AsQueryable();

            query2 = query2
                .OrderByDescending(q => q.yr)
                .ThenBy(q => q.cc);

            var query3 = (from q in query2.AsEnumerable()
                          select new ExpensesViewModel
                          {
                              AmountGetExpWindfall = (decimal)q.TotalAmount,
                              CostCenter = q.cc,
                              ScreenCode = "E8"
                          }).Distinct();

            return query3.ToList();
        }
    }
}