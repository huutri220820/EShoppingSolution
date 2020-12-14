﻿using ModelAndRequest.Rating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelAndRequest.Book
{
    public class BookDetailViewModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public decimal price { get; set; }
        public decimal sale { get; set; }
        public string category { get; set; }
        public int categoryId { get; set; }
        public int available { get; set; }
        //base64 or url image
        public string image { get; set; }
        public string keyWord { get; set; }
        public string description { get; set; }
        public List<RatingViewModel> comments { get; set; }
    }
}
