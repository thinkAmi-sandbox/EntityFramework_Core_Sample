using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApp.IsRequired
{
    public class NotNullEntity
    {
        public int Id { get; set; }
        public int NotNullField { get; set; }
    }
    
    public class NullableEntity
    {
        public int Id { get; set; }
        public int? NullableField { get; set; }
    }

    
    public class User
    {
        public int Id { get; set; }

        public List<Blog> AuthoredBlogs { get; set; }
        public List<Blog> ContributedToBlogs { get; set; }
    }
    public class Blog
    {
        public int Id { get; set; }
        
        [ForeignKey("Author")]
        public int AuthorFk { get; set; }
        public User Author { get; set; }
        
        [ForeignKey("Contributor")]
        public int? ContributorFk { get; set; }
        public User Contributor { get; set; }
    }
}