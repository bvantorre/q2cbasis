using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static googlemapmvc1.Models.Bigcarmodel;
using static googlemapmvc1.Models.CustomRange;
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

            var controlsvandaag = new List<Controles>();
            var controlsgisteren = new List<Controles>();
            var controlsdezeweek = new List<Controles>();
            var controlsvorigeweek = new List<Controles>();

            var controlsdezemaand = new List<Controles>();
            var controlsvorigemaand = new List<Controles>();

            var hitsvandaag = new List<Typehits>();
            var hitsgisteren= new List<Typehits>();
            var hitsdezeweek = new List<Typehits>();
            var hitsvorigeweek= new List<Typehits>();

            var hitsdezemaand = new List<Typehits>();
            var hitsvorigemaand = new List<Typehits>();




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


                //code for filter controls 
                
                DateTime vandaag = DateTime.Today;
                DateTime gisteren = DateTime.Today.AddDays(-1);
                             
                

               

               
                

               

                //this week+last week
                DayOfWeek weekStart = DayOfWeek.Monday;
                DateTime startingDate = DateTime.Today;

                while (startingDate.DayOfWeek != weekStart)
                    startingDate = startingDate.AddDays(-1);

                DateTime previousWeekStart = startingDate.AddDays(-7);
                DateTime previousWeekEnd = startingDate.AddDays(-1);

                var controlstoday = from x in returnstatlist where x.LPHS_CreatedOn.Date == vandaag.Date select x;
                var controlsyesterday = from x in returnstatlist where (x.LPHS_CreatedOn.Date == gisteren.Date) select x;

                var controlsthisweek = from x in returnstatlist where (startingDate.Date <= x.LPHS_CreatedOn.Date 
                                       && x.LPHS_CreatedOn.Date <= vandaag.Date) select x;
                var controlslastweek = from x in returnstatlist
                                       where (previousWeekStart.Date <= x.LPHS_CreatedOn.Date &&
                                       x.LPHS_CreatedOn.Date <= previousWeekEnd.Date)
                                       select x;
                var controlsthismonth = from x in returnstatlist
                                        where ((x.LPHS_CreatedOn.Month==vandaag.Month) && (x.LPHS_CreatedOn.Year == vandaag.Year))
                                        select x;
                var controlslastmonth = from x in returnstatlist
                                        where ((x.LPHS_CreatedOn.Month == (vandaag.Month-1)) && (x.LPHS_CreatedOn.Year == vandaag.Year ||
                                        x.LPHS_CreatedOn.Year==(vandaag.Year-1)))
                                        select x;

               




                var hitstoday = from x in returnhits where x.HTQU_CreatedOn.Date == vandaag.Date select x;

                var hitsyesterday = from x in returnhits where (x.HTQU_CreatedOn.Date == gisteren.Date) select x;
                
                var hitsthisweek = from x in returnhits where (startingDate.Date <= x.HTQU_CreatedOn.Date
                                   && x.HTQU_CreatedOn.Date <= vandaag.Date) select x;

                var hitslastweek= from x in returnhits
                                  where (previousWeekStart.Date <= x.HTQU_CreatedOn.Date &&
                                  x.HTQU_CreatedOn.Date <= previousWeekEnd.Date)
                                  select x;

                var hitsthismonth= from x in returnhits
                                   where ((x.HTQU_CreatedOn.Month == vandaag.Month) && (x.HTQU_CreatedOn.Year == vandaag.Year))
                                   select x;
                var hitslastmonth = from x in returnhits
                                    where ((x.HTQU_CreatedOn.Month == (vandaag.Month - 1)) && (x.HTQU_CreatedOn.Year == vandaag.Year ||
                                    x.HTQU_CreatedOn.Year == (vandaag.Year - 1)))
                                    select x;


                controlsvandaag = controlstoday.ToList();
                controlsgisteren = controlsyesterday.ToList();
                controlsdezeweek = controlsthisweek.ToList();
                controlsvorigeweek = controlslastweek.ToList();
                controlsdezemaand = controlsthismonth.ToList();
                controlsvorigemaand = controlslastmonth.ToList();

                hitsdezeweek = hitsthisweek.ToList();
                hitsvorigeweek = hitslastweek.ToList();
                hitsvandaag = hitstoday.ToList();
                hitsgisteren = hitsyesterday.ToList();
                hitsdezemaand = hitsthismonth.ToList();
                hitsvorigemaand = hitslastmonth.ToList();



                Debug.WriteLine(lphsdays.Count());


                //list of different scantypes (e.g. abonnement,parkingmonitor,parkeon,sms,..)
                var TypeControle = (from x in returnstatlist select x.typecontrol).Distinct();
                typecontrols = TypeControle.ToList();
                Debug.WriteLine("typecontroles_bart" + typecontrols.Count());

                //list of different hit types (0 : ongewerkte hit,1 : geannuleerde hit,2 :gevalideerde hit,4 : duplicate hit)
                var hittype = (from x in returnhits select x.typehit).Distinct();
                hittypes = hittype.ToList();

            }
            catch (Exception ex)
            {
                Console.WriteLine("error:  {0}", ex);
                connection.Close();

            }

            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;
          

            string typecontrolsjson = serializer.Serialize(typecontrols);

            string controlsvandaagjson = serializer.Serialize(controlsvandaag);
            string controlsgisterenjson = serializer.Serialize(controlsgisteren);
            string controlsdezeweekjson = serializer.Serialize(controlsdezeweek);
            string controlsvorigeweekjson = serializer.Serialize(controlsvorigeweek);
            string controlsdezemaandjson = serializer.Serialize(controlsdezemaand);
            string controlsvorigemaandjson = serializer.Serialize(controlsvorigemaand);

            string hitsvandaagjson = serializer.Serialize(hitsvandaag);
            string hitsgisterenjson = serializer.Serialize(hitsgisteren);
            string hitsdezeweekjson= serializer.Serialize(hitsdezeweek);
            string hitsvorigeweekjson = serializer.Serialize(hitsvorigeweek);
            string hitsdezemaandjson = serializer.Serialize(hitsdezemaand);
            string hitsvorigemaandjson = serializer.Serialize(hitsvorigemaand);


            string hitsjson = serializer.Serialize(returnhits);
            string hittypesjson = serializer.Serialize(hittypes);

           
            ViewBag.Typecontrolsjson = typecontrolsjson;

            ViewBag.Controlsvandaagjson = controlsvandaagjson;
            ViewBag.Controlsgisterenjson = controlsgisterenjson;
            ViewBag.Controlsdezeweekjson = controlsdezeweekjson;
            ViewBag.Controlsvorigeweekjson = controlsvorigeweekjson;
            ViewBag.Controlsdezemaandjson = controlsdezemaandjson;
            ViewBag.Controlsvorigemaandjson = controlsvorigemaandjson;

            ViewBag.Hitsvandaagjson = hitsvandaagjson;
            ViewBag.Hitsgisterenjson = hitsgisterenjson;
            ViewBag.Hitsdezeweekjson = hitsdezeweekjson;
            ViewBag.Hitsvorigeweekjson = hitsvorigeweekjson;
            ViewBag.Hitsdezemaandjson = hitsdezemaandjson;
            ViewBag.Hitsvorigemaandjson = hitsvorigemaandjson;


            ViewBag.Hittypesjson = hittypesjson;


            var model = new Models.CustomRange{
                Customenddate = DateTime.Now,
                Customstartdate = DateTime.Now
            };


            return View(model);

         
        }


        [HttpPost]
        public ActionResult customrangemethod(Models.CustomRange customRange)
        {
            var returnFullList = new List<Carread>();
            var returnstatlist = new List<Controles>();
            var returnhits = new List<Typehits>();

            var controlsvandaag = new List<Controles>();
            var controlsgisteren = new List<Controles>();
            var controlsdezeweek = new List<Controles>();
            var controlsvorigeweek = new List<Controles>();

            var controlsdezemaand = new List<Controles>();
            var controlsvorigemaand = new List<Controles>();

            var hitsvandaag = new List<Typehits>();
            var hitsgisteren = new List<Typehits>();
            var hitsdezeweek = new List<Typehits>();
            var hitsvorigeweek = new List<Typehits>();

            var hitsdezemaand = new List<Typehits>();
            var hitsvorigemaand = new List<Typehits>();




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

                Debug.WriteLine("aantal controles " + statlist.Count());


                returnstatlist = statlist.ToList();
                Debug.WriteLine(returnstatlist[5].typecontrol);

                var hits = connection.Query<Typehits>(typehitsquery);
                Debug.WriteLine("aantal hits " + hits.Count());


                returnhits = hits.ToList();
                returnFullList = fullList.ToList();



                var daytypes = (from x in returnFullList select x.HTQU_CreatedOn.Date).Distinct();
                days = daytypes.ToList();


                //code for filter controls 

                DateTime vandaag = DateTime.Today;
                DateTime gisteren = DateTime.Today.AddDays(-1);










                //this week+last week
                DayOfWeek weekStart = DayOfWeek.Monday;
                DateTime startingDate = DateTime.Today;

                while (startingDate.DayOfWeek != weekStart)
                    startingDate = startingDate.AddDays(-1);

                DateTime previousWeekStart = startingDate.AddDays(-7);
                DateTime previousWeekEnd = startingDate.AddDays(-1);

                var controlstoday = from x in returnstatlist where x.LPHS_CreatedOn.Date == vandaag.Date select x;
                var controlsyesterday = from x in returnstatlist where (x.LPHS_CreatedOn.Date == gisteren.Date) select x;

                var controlsthisweek = from x in returnstatlist
                                       where (startingDate.Date <= x.LPHS_CreatedOn.Date
              && x.LPHS_CreatedOn.Date <= vandaag.Date)
                                       select x;
                var controlslastweek = from x in returnstatlist
                                       where (previousWeekStart.Date <= x.LPHS_CreatedOn.Date &&
                                       x.LPHS_CreatedOn.Date <= previousWeekEnd.Date)
                                       select x;
                var controlsthismonth = from x in returnstatlist
                                        where ((x.LPHS_CreatedOn.Month == vandaag.Month) && (x.LPHS_CreatedOn.Year == vandaag.Year))
                                        select x;
                var controlslastmonth = from x in returnstatlist
                                        where ((x.LPHS_CreatedOn.Month == (vandaag.Month - 1)) && (x.LPHS_CreatedOn.Year == vandaag.Year ||
                                        x.LPHS_CreatedOn.Year == (vandaag.Year - 1)))
                                        select x;






                var hitstoday = from x in returnhits where x.HTQU_CreatedOn.Date == vandaag.Date select x;

                var hitsyesterday = from x in returnhits where (x.HTQU_CreatedOn.Date == gisteren.Date) select x;

                var hitsthisweek = from x in returnhits
                                   where (startingDate.Date <= x.HTQU_CreatedOn.Date
              && x.HTQU_CreatedOn.Date <= vandaag.Date)
                                   select x;

                var hitslastweek = from x in returnhits
                                   where (previousWeekStart.Date <= x.HTQU_CreatedOn.Date &&
                                   x.HTQU_CreatedOn.Date <= previousWeekEnd.Date)
                                   select x;

                var hitsthismonth = from x in returnhits
                                    where ((x.HTQU_CreatedOn.Month == vandaag.Month) && (x.HTQU_CreatedOn.Year == vandaag.Year))
                                    select x;
                var hitslastmonth = from x in returnhits
                                    where ((x.HTQU_CreatedOn.Month == (vandaag.Month - 1)) && (x.HTQU_CreatedOn.Year == vandaag.Year ||
                                    x.HTQU_CreatedOn.Year == (vandaag.Year - 1)))
                                    select x;


                controlsvandaag = controlstoday.ToList();
                controlsgisteren = controlsyesterday.ToList();
                controlsdezeweek = controlsthisweek.ToList();
                controlsvorigeweek = controlslastweek.ToList();
                controlsdezemaand = controlsthismonth.ToList();
                controlsvorigemaand = controlslastmonth.ToList();

                hitsdezeweek = hitsthisweek.ToList();
                hitsvorigeweek = hitslastweek.ToList();
                hitsvandaag = hitstoday.ToList();
                hitsgisteren = hitsyesterday.ToList();
                hitsdezemaand = hitsthismonth.ToList();
                hitsvorigemaand = hitslastmonth.ToList();



                Debug.WriteLine(lphsdays.Count());


                //list of different scantypes (e.g. abonnement,parkingmonitor,parkeon,sms,..)
                var TypeControle = (from x in returnstatlist select x.typecontrol).Distinct();
                typecontrols = TypeControle.ToList();
                Debug.WriteLine("typecontroles_bart" + typecontrols.Count());

                //list of different hit types (0 : ongewerkte hit,1 : geannuleerde hit,2 :gevalideerde hit,4 : duplicate hit)
                var hittype = (from x in returnhits select x.typehit).Distinct();
                hittypes = hittype.ToList();

            }
            catch (Exception ex)
            {
                Console.WriteLine("error:  {0}", ex);
                connection.Close();

            }

            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;


            string typecontrolsjson = serializer.Serialize(typecontrols);

            string controlsvandaagjson = serializer.Serialize(controlsvandaag);
            string controlsgisterenjson = serializer.Serialize(controlsgisteren);
            string controlsdezeweekjson = serializer.Serialize(controlsdezeweek);
            string controlsvorigeweekjson = serializer.Serialize(controlsvorigeweek);
            string controlsdezemaandjson = serializer.Serialize(controlsdezemaand);
            string controlsvorigemaandjson = serializer.Serialize(controlsvorigemaand);

            string hitsvandaagjson = serializer.Serialize(hitsvandaag);
            string hitsgisterenjson = serializer.Serialize(hitsgisteren);
            string hitsdezeweekjson = serializer.Serialize(hitsdezeweek);
            string hitsvorigeweekjson = serializer.Serialize(hitsvorigeweek);
            string hitsdezemaandjson = serializer.Serialize(hitsdezemaand);
            string hitsvorigemaandjson = serializer.Serialize(hitsvorigemaand);


            string hitsjson = serializer.Serialize(returnhits);
            string hittypesjson = serializer.Serialize(hittypes);


            ViewBag.Typecontrolsjson = typecontrolsjson;

            ViewBag.Controlsvandaagjson = controlsvandaagjson;
            ViewBag.Controlsgisterenjson = controlsgisterenjson;
            ViewBag.Controlsdezeweekjson = controlsdezeweekjson;
            ViewBag.Controlsvorigeweekjson = controlsvorigeweekjson;
            ViewBag.Controlsdezemaandjson = controlsdezemaandjson;
            ViewBag.Controlsvorigemaandjson = controlsvorigemaandjson;

            ViewBag.Hitsvandaagjson = hitsvandaagjson;
            ViewBag.Hitsgisterenjson = hitsgisterenjson;
            ViewBag.Hitsdezeweekjson = hitsdezeweekjson;
            ViewBag.Hitsvorigeweekjson = hitsvorigeweekjson;
            ViewBag.Hitsdezemaandjson = hitsdezemaandjson;
            ViewBag.Hitsvorigemaandjson = hitsvorigemaandjson;


            ViewBag.Hittypesjson = hittypesjson;


            return View("Index");


        }
    }
}