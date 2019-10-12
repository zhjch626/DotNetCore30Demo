using System;

namespace DotNetCore30Demo.Model
{
    public class CommentModel
    {
        public Guid Id { get; set; }

        public string Email { get; set; }

        public string Content { get; set; }

        public DateTime CommentDate { get; set; }
    }
}