using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static googlemapmvc1.Models.Bigcarmodel;
using System.Data.SqlClient;
using Dapper;
using System.Diagnostics;
using System.IO;



namespace googlemapmvc1.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
           
           
            
            var returnFullList = new List<Carread>();
            var returnstatlist = new List<Controles>();
           
            var returnPatrollist = new List<Patrollermodel>();
            var carTypesGuidList = new List<Guid>();
            var listsByPatrolId = new List<List<Carread>>();
            var days = new List<DateTime>();
            var lphsdays = new List<DateTime>();
            var typecontrols = new List<int>();

            string sqlstring = System.IO.File.ReadAllText(@"C:\Users\vanto\Downloads\sqlquerystat.sql");



                 string connectionString = "data source=ikt4ztoj17.database.windows.net;initial catalog=Q2CCloudMobile_OPC;user id=SQLAdmin;password=Q2C_141500";
            SqlConnection connection = new SqlConnection(connectionString);
            try
            {

                connection.Open();

                //Get vehcile list
               var fullList = connection.Query<Carread>("SELECT TOP 4000 * FROM [MOBILEQueue_LOCT_F850F71B-CFB8-469A-A092-88D3E207CC28] ORDER BY HTQU_CreatedOn DESC");
                //Create 1st vehicle list

                var statlist = connection.Query<Controles>(sqlstring);
                Debug.WriteLine(statlist.Count());
                var teststatlist = statlist.Take(1000);
                returnstatlist = teststatlist.ToList();
               




                returnFullList = fullList.ToList();
               

                var cartypes = (from x in returnFullList
                                orderby x.HTQU_PatrollerMOBI_ID
                                select x.HTQU_PatrollerMOBI_ID).Distinct();

                
                var daytypes = (from x in returnFullList select x.HTQU_CreatedOn.Date).Distinct();
                days = daytypes.ToList();


                //daytypes for lphs
                var lphsdaytypes = (from x in returnstatlist select x.LPHS_CreatedOn.Date).Distinct();
                lphsdays = lphsdaytypes.ToList();

                //list of different scantypes (e.g. abonnement,parkingmonitor,parkeon,sms,..)
                var LHDT_TypeControleDis = (from x in returnstatlist select x.LHDT_TypeControle).Distinct();
                typecontrols = LHDT_TypeControleDis.ToList();





                //if more than one scancar
                carTypesGuidList = cartypes.ToList();

                listsByPatrolId = returnFullList.
                    GroupBy(x => x.HTQU_PatrollerMOBI_ID).Select(g => g.ToList()).ToList();


           
               

            }
            catch (Exception ex)
            {
                Console.WriteLine("error:  {0}", ex);
                connection.Close();

            }

            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            string jsonlist = serializer.Serialize(returnFullList);
            string patrollistjson = serializer.Serialize(carTypesGuidList);
            string daysjson = serializer.Serialize(days);
            string statlistjson = serializer.Serialize(returnstatlist);
            string lphsdaysjson = serializer.Serialize(lphsdays);
            string typecontrolsjson = serializer.Serialize(typecontrols);

            ViewBag.Jsonlist = jsonlist;
            ViewBag.Patrollistjson = patrollistjson;
            ViewBag.Daysjson = daysjson;

            ViewBag.Statlistjson = statlistjson;
            ViewBag.Lphsdaysjson = lphsdaysjson;
            ViewBag.Typecontrolsjson = typecontrolsjson;

            return View();

         
        }
    }
}