using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MyApp.GenericInheritance;
using Xunit;

namespace MyApp.Test.GenericInheritanceTest
{
    public class GenericInheritanceTest
    {
        [InlineData("IsRequiredが true の場合", true)]
        // [InlineData("IsRequiredが false の場合", false)]
        [Theory]
        public void BlogのSQL確認(string title, bool isRequired)
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            try
            {
                var options = new DbContextOptionsBuilder<GenericInheritanceBlogContext>()
                    .EnableSensitiveDataLogging()
                    .UseSqlite(connection)
                    .Options;
                using var context = new GenericInheritanceBlogContext(options, isRequired);

                context.Database.EnsureCreated();
            }
            finally
            {
                connection.Close();
            }
        }
        
        // [InlineData("IsRequiredが true の場合", true)]
        // [InlineData("IsRequiredが false の場合", false)]
        // [Theory]
        public void UserのSQL確認(string title, bool isRequired)
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            try
            {
                var options = new DbContextOptionsBuilder<GenericInheritanceUserContext>()
                    .EnableSensitiveDataLogging()
                    .UseSqlite(connection)
                    .Options;
                using var context = new GenericInheritanceUserContext(options, isRequired);

                context.Database.EnsureCreated();
            }
            finally
            {
                connection.Close();
            }
        }
    }
}