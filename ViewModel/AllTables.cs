using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Library.Models;
namespace Library.ViewModel
{
    public class AllTables
    {
        public IEnumerable<book> books { get; set; }
        public IEnumerable<user> users { get; set; }
        public IEnumerable<author> authors { get; set; }
        public IEnumerable<publication> publications { get; set; }
        public IEnumerable<subcategory> subcategories { get; set; }
        public IEnumerable<category> categories { get; set; }
        public IEnumerable<comment> comments { get; set; }
        public IEnumerable<Log> logs { get; set; }
    }
}