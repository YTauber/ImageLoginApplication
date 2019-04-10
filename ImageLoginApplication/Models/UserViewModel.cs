using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ClassLibrary1;

namespace ImageLoginApplication.Models
{
    public class UserViewModel
    {
        public IEnumerable<Image> Images { get; set; }
        public string id { get; set; }
        public string password { get; set; }

    }

    public class ViewImageModel
    {
        public string Message { get; set; }
        public bool HasPermission { get; set; }
        public Image image { get; set; }
    }

    public class LogInViewModel
    {
        public string Message { get; set; }
    }
}