using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Library.Models
{
    [MetadataType(typeof(CustomMetaDataCategories))]
    public partial class category
    {
    }
    public class CustomMetaDataCategories
    {
        [Required]
        [Display(Name = "Category Name")]
        [StringLength(50, MinimumLength = 4)]
        public string category_name { get; set; }
        [StringLength(2000, MinimumLength = 20)]
        public string description { get; set; }
    }
}