using System;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MyApp.IsRequired;
using Xunit;

namespace MyApp.Test.IsRequiredTest
{
    public enum Pattern
    {
        NotNull,
        Nullable,
        Fk
    }

    public class IsRequiredTest
    {
        [InlineData("NotNull型int - IsRequiredが true の場合", Pattern.NotNull, true)]
        // [InlineData("NotNull型int - IsRequiredが false の場合", Pattern.NotNull, false)]
        // [InlineData("Nullable型int? - IsRequiredが true の場合", Pattern.Nullable, true)]
        // [InlineData("Nullable型int? - IsRequiredが false の場合", Pattern.Nullable, false)]
        // [InlineData("int/int?のFK - IsRequiredが true の場合", Pattern.Fk, true)]
        // [InlineData("int/int?のFK - IsRequiredが false の場合", Pattern.Fk, false)]
        [Theory]
        public void Nullable型やNotNull型のIsRequiredテスト(string title, Pattern pattern, bool isRequired)
        {
            Console.WriteLine($"{Environment.NewLine}==================={Environment.NewLine}" +
                              $"{title}{Environment.NewLine}" +
                              "===================");

            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            try
            {
                using DbContext context = pattern switch
                {
                    Pattern.NotNull => CreateContext<NotNullContext>(connection, isRequired),
                    Pattern.Nullable => CreateContext<NullableContext>(connection, isRequired),
                    Pattern.Fk => CreateContext<UserBlogContext>(connection, isRequired),
                    _ => throw new Exception()
                };
                context.Database.EnsureCreated();
            }
            finally
            {
                connection.Close();
            }
        }

        private T CreateContext<T>(SqliteConnection connection, bool isRequired)
            where T : DbContext
        {
            var options = new DbContextOptionsBuilder<T>()
                .EnableSensitiveDataLogging()
                .UseSqlite(connection)
                .Options;
            
            // Activator.CreateInstanceはパフォーマンス問題があることに注意
            // 参考：https://ufcpp.net/study/csharp/sp2_generics.html#new-constrants
            return (T) Activator.CreateInstance(typeof(T), options, isRequired);
        }
    }
}