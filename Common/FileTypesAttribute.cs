using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace Library.Common
{
    public class FileTypesAttribute : ValidationAttribute
    {
        public string AllowedExtensions { get; set; }

        public override bool IsValid(object value)
        {
            if (value == null){return true;}    // if don't change the photo,file

            HttpPostedFileBase file = value as HttpPostedFileBase;
            string ext = Path.GetExtension(file.FileName);

            ext = ext.TrimStart('.').ToLower();

            return AllowedExtensions.Contains(ext);
        }
    }
}