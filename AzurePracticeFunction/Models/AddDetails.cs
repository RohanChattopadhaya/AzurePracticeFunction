using System;
using System.Collections.Generic;
using System.Text;

namespace CosmosFunction.CosmosService
{
    public class AddDetails
    {
        public String id { get; set; }
        public string UniqueId { get; set; }
        public string UniqueName { get; set; }
        public string EmailID { get; set; }
        public List<Items> items { get; set; }
    }

    public class Items
    {
        public int ItemID { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public int UnitOfItem { get; set; }
    }
}
