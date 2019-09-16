using Library.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Library.Models
{
    [MetadataType(typeof(CustomMetaDataPublications))]
    public partial class publication
    {
    }
    public class CustomMetaDataPublications
    {
        [Required]
        [Display(Name = "Publication Name")]
        [StringLength(50, MinimumLength = 0)]
        public string name { get; set; }
        [StringLength(1000, MinimumLength = 10)]
        public string address { get; set; }
        [StringLength(100, MinimumLength = 0)]
        public string contact { get; set; }

        [FileTypes(ErrorMessage = "Only Image with extensions jpg,jpeg,png,gif allowed ", AllowedExtensions = "jpg,jpeg,png,gif,webp,bmp,tif,tiff")]
        [FileSize(MaxSize = 15, ErrorMessage = "File is Too big to Upload , Image Must be less than or Equal 15 MB ")]
        public HttpPostedFileBase uploadedPublicationLogo { get; set; }
    }
}