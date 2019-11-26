using System;
using System.Collections.Generic;

namespace WorkManagement.Models
{
    public partial class User
    {
        public int Id { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Tagname { get; set; }

        public string tokenRegister { get; set; }

        public string Status { get; set; }

        public DateTime tokenRegisterDate { get; set; }

        public string tokenResetPassword { get; set; }
        public DateTime tokenResetPasswordDate { get; set; }
        public string statusResetPassword { get; set; }
    }
}
