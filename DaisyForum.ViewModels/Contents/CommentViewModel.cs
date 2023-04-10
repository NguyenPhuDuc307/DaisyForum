using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DaisyForum.ViewModels.Contents
{
    public class CommentViewModel
    {
        public int Id { get; set; }
        public string? Content { get; set; }
        public int KnowledgeBaseId { get; set; }
        public string? OwnerUserId { get; set; }
        public string? OwnerName { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}