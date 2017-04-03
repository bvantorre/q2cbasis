using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace googlemapmvc1.Models
{
    public class Bigcarmodel
    {
        public class Carread
        {
            public Guid HTQU_ID { get; set; }
            public DateTime HTQU_CreatedOn { get; set; }
            public double HTQU_Longitude { get; set; }
            public double HTQU_Latitude { get; set; }
            public Guid HTQU_PatrollerMOBI_ID { get; set; }
            public string Vehicle { get; set; }
        }

        public class Controles

        {

            public int LHDT_TypeControle { get; set; }
            public int typecontrole { get; set; }
            public Guid LPHS_ID { get; set; }
            public DateTime LPHS_CreatedOn { get; set; }
            public double LPHS_Latitude { get; set; }
            public double LPHS_Longitude { get; set; }
        }


        public class Patrollermodel
        {
            public string Vehicle { get; set; }
            public int ID { get; set; }
            public Guid HTQU_PatrollerMOBI_ID { get; set; }


        }


        public class BigCarReadModel
        {
            public List<Patrollermodel> PatrollerList;
            public int Patroller;
            public string Vehicle { get; set; }
            public List<Carread> Reads { get; set; }
            public List<List<Carread>> listoflists;
        }
    }
}