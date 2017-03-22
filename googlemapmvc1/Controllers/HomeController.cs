using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static googlemapmvc1.Models.Bigcarmodel;
using System.Data.SqlClient;
using Dapper;
using System.Diagnostics;


namespace googlemapmvc1.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
           
            IEnumerable<Carread> carList1 = null;
            IEnumerable<Carread> carList2 = null;
           
            var returnFullList = new List<Carread>();
            var returnPatrollist = new List<Patrollermodel>();
            var carTypesGuidList = new List<Guid>();
            var listsByPatrolId = new List<List<Carread>>();
            var days = new List<DateTime>();
           




                 string connectionString = "data source=ikt4ztoj17.database.windows.net;initial catalog=Q2CCloudMobile_OPC;user id=SQLAdmin;password=Q2C_141500";
            SqlConnection connection = new SqlConnection(connectionString);
            try
            {

                connection.Open();

                //Get vehcile list
                var fullList = connection.Query<Carread>("SELECT TOP 10000 * FROM [MOBILEQueue_LOCT_F850F71B-CFB8-469A-A092-88D3E207CC28] ORDER BY HTQU_CreatedOn DESC");
                //Create 1st vehicle list
                

                

               
                returnFullList=fullList.ToList();
               

                var cartypes = (from x in returnFullList
                                orderby x.HTQU_PatrollerMOBI_ID
                                select x.HTQU_PatrollerMOBI_ID).Distinct();


                var daytypes = (from x in returnFullList select x.HTQU_CreatedOn.Date).Distinct();

                days = daytypes.ToList();

                Debug.WriteLine("bart" + days.Count());

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
            serializer.MaxJsonLength = Int32.MaxValue;
            string jsonlist = serializer.Serialize(returnFullList);
            string patrollistjson = serializer.Serialize(carTypesGuidList);
            string daysjson = serializer.Serialize(days);

            ViewBag.Jsonlist = jsonlist;
            ViewBag.Patrollistjson = patrollistjson;
            ViewBag.Daysjson = daysjson;

            return View();

         
        }
    }
}