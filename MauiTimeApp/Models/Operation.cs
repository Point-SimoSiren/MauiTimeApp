using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiTimeApp.Models
{
    class Operation
    {
        public int EmployeeID { get; set; }
        public int WorkAssignmentID { get; set; }
        public string OperationType { get; set; }
        public string Comment { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }
}
