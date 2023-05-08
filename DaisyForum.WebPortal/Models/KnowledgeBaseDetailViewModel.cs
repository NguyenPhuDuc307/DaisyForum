using DaisyForum.ViewModels.Contents;
using DaisyForum.ViewModels.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DaisyForum.WebPortal.Models
{
    public class KnowledgeBaseDetailViewModel
    {
        public CategoryViewModel? Category { set; get; }
        public KnowledgeBaseViewModel? Detail { get; set; }
        public List<LabelViewModel>? Labels { get; set; }
        public UserViewModel? CurrentUser { get; set; }
    }
}