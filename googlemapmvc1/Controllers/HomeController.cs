﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static googlemapmvc1.Models.Bigcarmodel;
using System.Data.SqlClient;
using Dapper;


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
            
           




                 string connectionString = "data source=ikt4ztoj17.database.windows.net;initial catalog=Q2CCloudMobile_OPC;user id=SQLAdmin;password=Q2C_141500";
            SqlConnection connection = new SqlConnection(connectionString);
            try
            {

                connection.Open();

                //Get vehcile list
                var fullList = connection.Query<Carread>("SELECT TOP 50 * FROM [MOBILEQueue_LOCT_F850F71B-CFB8-469A-A092-88D3E207CC28] ORDER BY HTQU_CreatedOn DESC");
                //Create 1st vehicle list
                carList1 = fullList.Take(25);

                //Create 2nd vehicle list
                carList2 = fullList.Skip(25).Take(25);

                

                Guid newVehicleGuid = Guid.NewGuid();
                foreach (var item in carList2)
                {
                    item.HTQU_PatrollerMOBI_ID = newVehicleGuid;
                }
                //Combine 2 lists
                returnFullList.AddRange(carList1);
                returnFullList.AddRange(carList2);

                var cartypes = (from x in returnFullList
                                orderby x.HTQU_PatrollerMOBI_ID
                                select x.HTQU_PatrollerMOBI_ID).Distinct();

              
               
              


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
            
            ViewBag.Jsonlist = jsonlist;
            ViewBag.Patrollistjson = patrollistjson;

            return View();

         
        }
    }
}