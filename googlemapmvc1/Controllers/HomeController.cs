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
            var returnhits = new List<Typehits>();
           
           
            var returnPatrollist = new List<Patrollermodel>();
            var carTypesGuidList = new List<Guid>();
            var listsByPatrolId = new List<List<Carread>>();
            var days = new List<DateTime>();
            var lphsdays = new List<DateTime>();
            var typecontrols = new List<int>();
            var hittypes = new List<int>();

            string sqlstring = System.IO.File.ReadAllText(@"C:\Users\vanto\Desktop\sqlquerystat.sql");
            string typehitsquery = System.IO.File.ReadAllText(@"C:\Users\vanto\Desktop\typehitsquery.sql");


            string connectionString = "data source=ikt4ztoj17.database.windows.net;initial catalog=Q2CCloudMobile_OPC;user id=SQLAdmin;password=Q2C_141500";
            SqlConnection connection = new SqlConnection(connectionString);
            try
            {

                connection.Open();

                //Get vehcile list
               var fullList = connection.Query<Carread>("SELECT * FROM [MOBILEQueue_LOCT_F850F71B-CFB8-469A-A092-88D3E207CC28] ORDER BY HTQU_CreatedOn DESC");
                //Create 1st vehicle list

               var statlist = connection.Query<Controles>(sqlstring);

                Debug.WriteLine("aantal controles" + statlist.Count());
               

                returnstatlist = statlist.ToList();
                Debug.WriteLine(returnstatlist[5].typecontrol);

                var hits = connection.Query<Typehits>(typehitsquery);
                Debug.WriteLine("aantal hits"+ hits.Count());


                

                returnhits = hits.ToList();
               




                returnFullList = fullList.ToList();
               

                
                
                var daytypes = (from x in returnFullList select x.HTQU_CreatedOn.Date).Distinct();
                days = daytypes.ToList();


                //daytypes for lphs


                DateTime vandaag = DateTime.Today;
                DateTime gisteren = DateTime.Today.AddDays(-1);

                var lphsdaytypes = (from x in returnstatlist select x.LPHS_CreatedOn.Date).Distinct();
                lphsdays = lphsdaytypes.ToList();



                var lphstoday = from x in returnstatlist where x.LPHS_CreatedOn.Date== vandaag.Date select x;
                var lphsyesterday = from x in returnstatlist where (x.LPHS_CreatedOn.Date == gisteren.Date) select x;
                Debug.WriteLine(lphsdays.Count());


                //list of different scantypes (e.g. abonnement,parkingmonitor,parkeon,sms,..)
                var TypeControle = (from x in returnstatlist select x.typecontrol).Distinct();
                typecontrols = TypeControle.ToList();
                Debug.WriteLine("typecontroles_bart" + typecontrols.Count());

                //list of different hit types (0 : ongewerkte hit,1 : geannuleerde hit,2 :gevalideerde hit,4 : duplicate hit)
                var hittype = (from x in returnhits select x.typehit).Distinct();
                hittypes = hittype.ToList();



               
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

            string statlistjson = serializer.Serialize(returnstatlist);
            string lphsdaysjson = serializer.Serialize(lphsdays);

            string typecontrolsjson = serializer.Serialize(typecontrols);

            string hitsjson = serializer.Serialize(returnhits);
            string hittypesjson = serializer.Serialize(hittypes);

            ViewBag.Jsonlist = jsonlist;
            ViewBag.Patrollistjson = patrollistjson;
            ViewBag.Daysjson = daysjson;

            ViewBag.Statlistjson = statlistjson;
            ViewBag.Lphsdaysjson = lphsdaysjson;
            ViewBag.Typecontrolsjson = typecontrolsjson;

            ViewBag.Hitsjson = hitsjson;
            ViewBag.Hittypesjson = hittypesjson;


            return View();

         
        }
    }
}