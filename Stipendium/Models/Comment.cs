using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Stipendium.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public virtual ApplicationUser Commenter { get; set; }
        public virtual Stiftelse Stiftelse { get; set; }
        public string Body { get; set; }
        public DateTime CommentDate { get; set; }
        public DateTime? EditDate { get; set; }
    }
}