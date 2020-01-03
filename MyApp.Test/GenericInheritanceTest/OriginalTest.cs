using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MyApp.GenericInheritance;
using Xunit;

namespace MyApp.Test.GenericInheritanceTest
{
    public class OriginalTest
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
                var options = new DbContextOptionsBuilder<OriginalBlogContext>()
                    .EnableSensitiveDataLogging()
                    .UseSqlite(connection)
                    .Options;
                using var context = new OriginalBlogContext(options, isRequired);

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
                var options = new DbContextOptionsBuilder<OriginalUserContext>()
                    .EnableSensitiveDataLogging()
                    .UseSqlite(connection)
                    .Options;
                using var context = new OriginalUserContext(options, isRequired);

                context.Database.EnsureCreated();
            }
            finally
            {
                connection.Close();
            }
        }
    }
}