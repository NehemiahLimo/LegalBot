using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LegalBotTest.Models
{
    public class UserProfile
    {
        public int Id { get; set; }
        public string  Name { get; set; }
        public string County{ get; set; }
        public string SubCounty { get; set; }
        public string Ward { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }


    }
}
