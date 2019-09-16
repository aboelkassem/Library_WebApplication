using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Library.Common
{
    public class FileSizeAttribute : ValidationAttribute , IClientValidatable
    {
        public int MaxSize { get; set; }    // MB

        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return true;
            }
            else
            {
                HttpPostedFileBase file = value as HttpPostedFileBase;
                
                return file.ContentLength <= MaxSize * 1048576;
            }
        }
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            ModelClientValidationRule rule = new ModelClientValidationRule();
            rule.ErrorMessage = base.ErrorMessage;
            rule.ValidationType = "fileszie";
            rule.ValidationParameters.Add("maxsize", MaxSize);
            return new ModelClientValidationRule[] { rule };
        }

    }
}