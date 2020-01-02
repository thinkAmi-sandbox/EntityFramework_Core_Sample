using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApp.ClientSetNull
{
    public interface IUser<T>
    {
        string Name { get; set; }
        List<T> AuthoredBlogs { get; set; }
    }
    public interface IBlog
    {
        int Id { get; set; }
        int? AuthorId { get; set; }
        
    }
    
    // User - Blog 間に、FKが1つの場合
    public class SingleFkUser : IUser<SingleFkBlog>
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<SingleFkBlog> AuthoredBlogs { get; set; }
    }
    
    public class SingleFkBlog: IBlog
    {
        public int Id { get; set; }
        public string Content { get; set; }
        
        public int? AuthorId { get; set; }
        public SingleFkUser Author { get; set; }
    }
    
    
    // User - Blog 間に、FKが2つの場合
    // multiple cascade pathsな関係
    public class MultiFkUser : IUser<MultiFkBlog>
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [InverseProperty("Author")]
        public List<MultiFkBlog> AuthoredBlogs { get; set; }
        
        [InverseProperty("Contributor")]
        public List<MultiFkBlog> ContributedToBlogs { get; set; }
        
    }
    
    public class MultiFkBlog: IBlog
    {
        public int Id { get; set; }
        public string Content { get; set; }
        
        public int? AuthorId { get; set; }
        public MultiFkUser Author { get; set; }
        
        public int? ContributorId { get; set; }
        public MultiFkUser Contributor { get; set; }
    }
}