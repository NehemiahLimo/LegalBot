using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace LegalBotTest.Models
{
    public class SurveyContext : DbContext
    {
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<SurveyData> SurveyData { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=blogging.db");
            
    }

}