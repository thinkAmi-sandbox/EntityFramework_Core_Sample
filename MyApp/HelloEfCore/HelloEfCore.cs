namespace MyApp.HelloEfCore
{
    public class HelloEfCore
    {
        public static void InsertMsSqlServer()
        {
            using var context = new HelloContext();
            
            context.HelloModels.Add(new HelloModel
            {
                Content = "Hello World!"
            });
            context.SaveChanges();
        }
    }
}