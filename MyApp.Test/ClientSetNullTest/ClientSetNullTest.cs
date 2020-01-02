using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MyApp.ClientSetNull;
using Xunit;

namespace MyApp.Test.ClientSetNullTest
{

    public enum DbEngine
    {
        MsSqlServer, PostgreSql
    }

    // SQLの発行タイミングを確認したいため、直列でテストを実行する
    // https://stackoverflow.com/questions/1408175/execute-unit-tests-serially-rather-than-in-parallel
    [Collection("ClientSetNull")]
    public class ClientSetNullTest
    {
        
        // xunit時にSQLを出力できるように設定したいが、今回は使わない
        // https://stackoverflow.com/a/43818769
        // private readonly ITestOutputHelper _output;
        //
        //
        // public ClientSetNullTest(ITestOutputHelper output)
        // {
        //     this._output = output;
        // }


        // Theoryですべてをテストすると、ON DELETEが一つのもの(例：NOT NULL)に制限されるので、1つずつアンコメントして試す
        // [InlineData("単一FK - 任意 - SetNull", DeleteBehavior.SetNull, false)]
        // [InlineData("単一FK - 任意 - ClientSetNull", DeleteBehavior.ClientSetNull, false)]
        // [InlineData("単一FK - 任意 - Cascade", DeleteBehavior.Cascade, false)]
        // [Theory]
        public void SingleForeignKeyTest(string title, DeleteBehavior deleteBehavior, bool isRequired)
        {
            // 初期データ作成
            CreateSingleData(deleteBehavior, isRequired);
            
            Console.WriteLine($"{Environment.NewLine}================= {title} =================");
            
            using var context = CreateSingleContext(deleteBehavior, isRequired);
        
            // Userを取得
            var user = context.SingleFkUsers.Include(u => u.AuthoredBlogs).First();
        
            // 検証
            Validate(context, user);
        
            // 検証後の確認
            using var afterContext = CreateSingleContext(deleteBehavior, isRequired);
            PrintSingleBlogs("STEP4", afterContext);
        }

        // Theoryですべてをテストすると、ON DELETEが一つのもの(例：NOT NULL)に制限されるので、1つずつアンコメントして試す
        // [InlineData("複数FK - SetNull - 任意 - Includeあり - MSSQLServer", 
        //     DeleteBehavior.SetNull, false, true, DbEngine.MsSqlServer)]
        // [InlineData("複数FK - Cascade - 任意 - Includeあり - MSSQLServer", 
        //     DeleteBehavior.Cascade, false, true, DbEngine.MsSqlServer)]
        // [InlineData("複数FK - ClientSetNull - 任意 - Includeあり - MSSQLServer", 
        //     DeleteBehavior.ClientSetNull, false, true, DbEngine.MsSqlServer)]
        // [InlineData("複数FK - ClientSetNull - 任意 - Include無し - MSSQLServer", 
        //     DeleteBehavior.ClientSetNull, false, false, DbEngine.MsSqlServer)]
        // [InlineData("複数FK - SetNull - 任意 - Includeあり - PostgreSQL", 
        //     DeleteBehavior.SetNull, false, true, DbEngine.PostgreSql)]
        [InlineData("複数FK - Cascade - 任意 - Include無し - PostgreSQL", 
            DeleteBehavior.Cascade, false, false, DbEngine.PostgreSql)]
        [Theory]
        public void MultiForeignKeyTest(string title, DeleteBehavior deleteBehavior,
            bool isRequired, bool withInclude, DbEngine dbEngine)
        {
            CreateMultiData(deleteBehavior, isRequired, dbEngine);
            Console.WriteLine($"{Environment.NewLine}================= {title} =================");
            
            using var context = CreateMultiContext(deleteBehavior, isRequired, dbEngine);

            // 複数のFKのうち、片方だけ指定すると、以下のエラーが発生して削除できない
            // SQL Server
            //   Microsoft.Data.SqlClient.SqlException (0x80131904):
            //   The DELETE statement conflicted with the REFERENCE constraint
            //   "FK_MultiFkBlogs_MultiFkUsers_ContributorId"
            // PostgreSQL
            //   Npgsql.PostgresException (0x80004005): 23503:
            //   update or delete on table "MultiFkUsers" violates foreign key constraint
            //   "FK_MultiFkBlogs_MultiFkUsers_ContributorId" on table "MultiFkBlogs"
            var user = withInclude
                // すべてのFKを含む
                ? context.MultiFkUsers
                    .Include(u => u.AuthoredBlogs)
                    .Include(u => u.ContributedToBlogs)
                    .First()
                // 一部のFKだけ含む
                : context.MultiFkUsers
                    .Include(u => u.AuthoredBlogs)
                    .First();

            Validate(context, user);

            using var afterContext = CreateMultiContext(deleteBehavior, isRequired, dbEngine);
            PrintMultiBlogs("STEP4", afterContext);
        }

        private void CreateSingleData(DeleteBehavior deleteBehavior, bool isRequired)
        {
            using var context = CreateSingleContext(deleteBehavior, isRequired);
            
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
                
            var user = new SingleFkUser { Name = "Foo"};
            context.SingleFkUsers.Add(user);

            var blog = new List<SingleFkBlog>
            {
                new SingleFkBlog
                {
                    Content = "Single User Blog 1",
                    Author = user,
                    AuthorId = user.Id
                },
                new SingleFkBlog
                {
                    Content = "Single User Blog 2",
                    Author = user,
                    AuthorId = user.Id
                }
            };

            context.SingleFkBlogs.AddRange(blog);
            context.SaveChanges();
        }
        
        private void CreateMultiData(DeleteBehavior deleteBehavior, bool isRequired, DbEngine dbEngine)
        {
            using var context = CreateMultiContext(deleteBehavior, isRequired, dbEngine);
            
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            var userTarget = new MultiFkUser {Name = "Target"};
            var userFoo = new MultiFkUser {Name = "Foo"};
            var userBar = new MultiFkUser {Name = "Bar"};
            context.MultiFkUsers.AddRange(userTarget, userFoo, userBar);

            var blogs = new List<MultiFkBlog>
            {
                // AuthorとContributorが同じで、削除対象
                new MultiFkBlog
                {
                    Content = "Same User Blog",
                    Author = userTarget,
                    AuthorId = userTarget.Id,
                    Contributor = userTarget,
                    ContributorId = userTarget.Id
                },
                // Authorだけ削除対象
                new MultiFkBlog
                {
                    Content = "Only Author User Blog",
                    Author = userTarget,
                    AuthorId = userTarget.Id,
                    Contributor = userFoo,
                    ContributorId = userFoo.Id
                },
                // Contributorだけ削除対象
                new MultiFkBlog
                {
                    Content = "Only Contributor User Blog",
                    Author = userFoo,
                    AuthorId = userFoo.Id,
                    Contributor = userTarget,
                    ContributorId = userTarget.Id
                },
                // Author、Contributorともに違う
                new MultiFkBlog
                {
                    Content = "Another User Blog",
                    Author = userFoo,
                    AuthorId = userFoo.Id,
                    Contributor = userBar,
                    ContributorId = userBar.Id
                }
            };

            context.MultiFkBlogs.AddRange(blogs);
            context.SaveChanges();
        }

        private SingleContext CreateSingleContext(DeleteBehavior deleteBehavior, bool isRequired)
        {
            var opt = new DbContextOptionsBuilder<SingleContext>()
                .EnableSensitiveDataLogging()
                .UseSqlServer(@"Server=tcp:.;Database=efcore;User=sa;Password=Your_password123;")
                .Options;

            return new SingleContext(opt, deleteBehavior, isRequired);
        }
        
        private MultiContext CreateMultiContext(DeleteBehavior deleteBehavior, bool isRequired, DbEngine dbEngine)
        {
            var opt = dbEngine switch
            {
                DbEngine.MsSqlServer => new DbContextOptionsBuilder<MultiContext>()
                    .EnableSensitiveDataLogging()
                    .UseSqlServer(@"Server=tcp:.;Database=efcore;User=sa;Password=Your_password123;")
                    .Options,
                DbEngine.PostgreSql => new DbContextOptionsBuilder<MultiContext>()
                    .EnableSensitiveDataLogging()
                    .UseNpgsql(@"Host=localhost;Database=efcore;Port=55432;Username=postgres;Password=Your_password123")
                    .Options,
                _ => throw new Exception()
                
            };
            return new MultiContext(opt, deleteBehavior, isRequired);
        }
        
        private void Validate<T>(DbContext context, IUser<T> user)
        {
            try {
                PrintEntities(context, "STEP-1", user);
                Console.WriteLine("==== SQL実行確認 ====");
    
                context.Remove(user);
                PrintEntities(context, "STEP-2", user);
                
                context.SaveChanges();
                PrintEntities(context, "STEP-3", user);
            }
            catch (Exception e)
            {
                Console.WriteLine("---- 例外発生 ----");
                Console.WriteLine(e.ToString());
            }
        }

        // userにはsingleとmultiがくるので、<T>で受け取っておく
        private void PrintEntities<T>(DbContext context, string step, IUser<T> user)
        {
            var authoredBlogs = user.AuthoredBlogs.ToList();
            
            Console.WriteLine(step);
            var userEntry = context.Entry(user);
            Console.WriteLine($"    User：状態 '{userEntry.State}'、" +
                              $"参照Author件数 '{authoredBlogs.Count}'");
            
            foreach (var blog in authoredBlogs)
            {
                var blogEntry = context.Entry(blog);
                
                // Tからキャストする
                var authorBlog = (IBlog) blog;
                
                // null条件演算子で分かりやすくする
                // https://www.atmarkit.co.jp/ait/articles/1607/07/news023.html
                var authorId = authorBlog.AuthorId?.ToString();
                Console.WriteLine(
                    $"    AuthoredBlog：状態 '{blogEntry.State}'、" +
                    $"Id '{authorBlog.Id}'、" +
                    $"外部キー(AuthorId) '{authorId ?? "null"}'");
            }
        }

        private void PrintSingleBlogs(string step, SingleContext context)
        {
            Console.WriteLine(step);
            
            var blogs = context.SingleFkBlogs.ToList();

            if (blogs.Count > 0)
            {
                foreach (var blog in blogs)
                {
                    Console.WriteLine(
                        $"Id: '{blog.Id}' / AuthorId: '{blog.AuthorId?.ToString() ?? "null"}'");
                }                
            }
            else
            {
                Console.WriteLine("Blogデータがありません");
            }
        }
        
        private void PrintMultiBlogs(string step, MultiContext context)
        {
            Console.WriteLine(step);
            
            var blogs = context.MultiFkBlogs.ToList();

            if (blogs.Count > 0)
            {
                foreach (var blog in blogs)
                {
                    Console.WriteLine(
                        $"Id: '{blog.Id}' / " +
                        $"AuthorId: '{blog.AuthorId?.ToString() ?? "null"}' /" +
                        $"ContributorId: '{blog.ContributorId?.ToString() ?? "null"}'");
                }                
            }
            else
            {
                Console.WriteLine("Blogデータがありません");
            }
        }
    }
}