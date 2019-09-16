using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Library.Models
{
    [MetadataType(typeof(CustomMetaDataSubCategories))]
    public partial class subcategory
    {
    }
    public class CustomMetaDataSubCategories
    {
        [Required]
        [Display(Name = "Sub Category")]
        [StringLength(50, MinimumLength = 0)]
        public string subcategory_name { get; set; }
    }
}