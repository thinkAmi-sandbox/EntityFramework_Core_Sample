namespace MyApp.GenericInheritance
{
#nullable enable
    public class Blog
    {
        public int Id { get; set; }
        public string? Url { get; set; }
    }

    public class User
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }
#nullable restore
}