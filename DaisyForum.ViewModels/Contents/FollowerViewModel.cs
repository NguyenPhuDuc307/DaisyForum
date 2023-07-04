using DaisyForum.ViewModels.Systems;

namespace DaisyForum.ViewModels.Contents
{
    public class FollowerViewModel
    {
        public string? OwnerUserId { get; set; }
        public UserViewModel? Owner { get; set; }
        public string? FollowerId { get; set; }
        public UserViewModel? Follower { get; set; }
        public DateTime CreateDate { get; set; }
        public bool Notification { get; set; }
    }
}