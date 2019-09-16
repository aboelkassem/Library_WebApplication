using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Library.Common;

namespace Library.Models
{
    [MetadataType(typeof(CustomMetaDataBooks))]
    public partial class book
    {
    }
    public class CustomMetaDataBooks
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        [Display(Name = "Book Title")]
        public string title { get; set; }
        [Required]
        [Display(Name = "Edition")]
        [StringLength(50, MinimumLength = 0)]
        [Range(0, 99)]
        public string edition { get; set; }
        [Required]
        [Display(Name = "Price")]
        [DataType(DataType.Currency)]
        public int price { get; set; }
        [Display(Name = "Photo")]
        public string photo { get; set; }
        [Range(0, 3000)]
        [Display(Name = "Pages Number")]
        public int pages { get; set; }
        [StringLength(1500, MinimumLength = 30)]
        [Display(Name = "Description")]
        public string description { get; set; }
        [StringLength(50, MinimumLength = 0)]
        [Display(Name = "Added Date")]
        [DataType(DataType.DateTime)]
        public string add_date { get; set; }
        [Range(0, 5)]
        public Nullable<byte> rate { get; set; }
        [Range(0, 2)]
        public Nullable<byte> availabilty { get; set; }


        [FileTypes(ErrorMessage = "Only Image with extensions jpg,jpeg,png,gif allowed ", AllowedExtensions = "jpg,jpeg,png,gif,webp,bmp,tif,tiff")]
        [FileSize(MaxSize = 15, ErrorMessage = "File is Too big to Upload , Image Must be less than or Equal 15 MB ")]
        public HttpPostedFileBase uploadedImage { get; set; }

        [FileTypes(ErrorMessage = "Only pdf files allowed", AllowedExtensions ="pdf,doc,docx")]
        [FileSize(MaxSize = 100,ErrorMessage = "File is Too big to Upload , PDF Must be less than or Equal 100 MB")]
        public HttpPostedFileBase uploadedFile { get; set; }
        public int Category_Id { get; set; }
    }
}