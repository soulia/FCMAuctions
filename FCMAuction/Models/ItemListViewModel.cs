using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FCMAuction.Models
{
    public class ItemListViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        [DisplayFormat(DataFormatString = "{0:c}")]
        public int Value { get; set; }
        [Display(Name = "Minimum")]
        [DisplayFormat(DataFormatString = "{0:c}")]
        public int MinimumBid { get; set; }
        [DisplayFormat(DataFormatString = "{0:c}")]
        public int NewBid { get; set; }
        [Display(Name = "Highest")]
        [DisplayFormat(DataFormatString = "{0:c}")]
        public int HighestBid { get; set; }
    }
}