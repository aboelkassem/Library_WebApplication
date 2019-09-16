using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Library.Models
{
    [MetadataType(typeof(CustomMetaDataComments))]
    public partial class comment
    {
    }

    public class CustomMetaDataComments
    {
        [Display(Name ="Comment Data")]
        [DataType(DataType.DateTime)]
        public string comment_data { get; set; }
        [Required]
        [StringLength(2000, MinimumLength = 5)]
        public string comments { get; set; }
    }
}