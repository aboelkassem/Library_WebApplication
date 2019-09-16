using Library.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Library.Models
{
    [MetadataType(typeof(CustomMetaDataAuthors))]
    public partial class author
    {
    }

    public class CustomMetaDataAuthors
    {
        [Required]
        [RegularExpression(@"^(([A-za-z]+[\s]{1}[A-za-z]+)|([A-Za-z]+))$", ErrorMessage = "please enter upper and lower case alphabets only , don't write spaces or something like this")]
        [StringLength(50, MinimumLength = 3)]
        [Display(Name = "First Name")]
        public string first_name { get; set; }
        [Required]
        [RegularExpression(@"^(([A-za-z]+[\s]{1}[A-za-z]+)|([A-Za-z]+))$", ErrorMessage = "please enter upper and lower case alphabets only , don't write spaces or something like this")]
        [StringLength(50, MinimumLength = 3)]
        [Display(Name = "Last Name")]
        public string last_name { get; set; }
        [StringLength(1000, MinimumLength = 30)]
        [Display(Name = "Description")]
        public string description { get; set; }
        [StringLength(100, MinimumLength = 0)]
        public string nationality { get; set; }
        [FileTypes(ErrorMessage = "Only Image with extensions jpg,jpeg,png,gif allowed ", AllowedExtensions = "jpg,jpeg,png,gif,webp,bmp,tif,tiff")]
        [FileSize(MaxSize = 15, ErrorMessage = "File is Too big to Upload , Image Must be less than or Equal 15 MB ")]
        public HttpPostedFileBase uploadedAuthorImage { get; set; }
    }
}