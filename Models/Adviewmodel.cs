﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eMarketing_project.Models
{
    public class Adviewmodel
    {
        public int pro_id { get; set; }
        public string pro_name { get; set; }
        public string pro_image { get; set; }
        public string pro_des { get; set; }
        public Nullable<int> pro_price { get; set; }
        public Nullable<int> pro_fk_cat { get; set; }
        public Nullable<int> pro_fk_user { get; set; }
        public Nullable<bool> pro_status { get; set; }
        public int cat_id { get; set; }
        public string cat_name { get; set; }
        public string u_name { get; set; }
        public string u_image { get; set; }
        public string u_contact { get; set; }
        public int cart_id { get; set; }
        public Nullable<int> user_id { get; set; }

        public Nullable<int> Quantity { get; set; }
        public string AddedOn { get; set; }

        public virtual tbl_product tbl_product { get; set; }
        public virtual tbl_user tbl_user { get; set; }
    }
}