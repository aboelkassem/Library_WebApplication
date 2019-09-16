using Library.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Library.Models
{
    [MetadataType(typeof(CustomMetaDataUsers))]
    public partial class user
    {
    }
    public class CustomMetaDataUsers
    {
        [RegularExpression(@"^(([A-za-z]+[\s]{1}[A-za-z]+)|([A-Za-z]+))$", ErrorMessage ="please enter upper and lower case alphabets only, don't write spaces or something like this")]
        [Display(Name = "First Name")]
        [StringLength(50, MinimumLength = 3)]
        public string first_name { get; set; }
        [RegularExpression(@"^(([A-za-z]+[\s]{1}[A-za-z]+)|([A-Za-z]+))$", ErrorMessage = "please enter upper and lower case alphabets only, don't write spaces or something like this")]
        [StringLength(50, MinimumLength = 3)]
        [Display(Name = "Last Name")]
        public string last_name { get; set; }
        [Display(Name = "Join Data")]
        [DataType(DataType.DateTime)]
        public string join_data { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 4)]
        [Remote("IsUerNameAvailable", "users", ErrorMessage = "User Name Is Already exist , please Write different Name")]
        public string UserName { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string PasswordHash { get; set; }
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"^[\w-\._\+%]+@(?:[\w-]+\.)+[\w]{2,6}$", ErrorMessage = "Please enter a valid email address")]
        [StringLength(50, MinimumLength = 4)]
        [Remote("IsEmailAvailable", "users", ErrorMessage = "Email Is Already exist , please Write different Email")]
        public string Email { get; set; }
        [DataType(DataType.PhoneNumber)]
        [StringLength(50, MinimumLength = 0)]
        [RegularExpression(@"(201)[0-9]{9}", ErrorMessage = "Please enter a valid Phone Starting With 201")]
        [Remote("IsPhoneAvailable", "users", ErrorMessage = "Phone Number Is Already exist , please Write different Phone Number")]
        public string PhoneNumber { get; set; }
        [Display(Name = "Email Verfiy")]
        public bool EmailConfirmed { get; set; }
        [Display(Name = "Security Stamp")]
        public string SecurityStamp { get; set; }
        [Display(Name = "Phone Verfiy")]
        public bool PhoneNumberConfirmed { get; set; }

        [DataType(DataType.Upload)]
        [FileTypes(ErrorMessage = "Only Image with extensions jpg,jpeg,png,gif allowed ", AllowedExtensions = "jpg,jpeg,png,gif,webp,bmp,tif,tiff")]
        [FileSize(MaxSize = 15, ErrorMessage = "File is Too big to Upload , Image Must be less than or Equal 15 MB ")]
        public HttpPostedFileBase uploadedUserImage { get; set; }
    }
}