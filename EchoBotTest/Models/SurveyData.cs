using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Threading.Tasks;

namespace LegalBotTest.Models
{
    public class SurveyData
    {
        public int Id { get; set; }
        public string Question { get; set; }
        public string ProvisionService { get; set; }
        public string Accesibility { get; set; }
        public string ImproveAccess { get; set; }
        public string Recommend { get; set; }
        public string OverallExperience { get; set; }
        public string ImproveExperience { get; set; }
    }
}
