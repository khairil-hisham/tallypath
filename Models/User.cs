using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Tallypath.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required] public string Username { get; set; } = "";
        [Required] public string PasswordHash { get; set; } = "";
        public string Fullname { get; set; } = "";
        [EmailAddress] public string Email { get; set; } = "";
        [Phone] public string Mobile { get; set; } = "";
        public string Dob { get; set; } = "";

        public ICollection<GroupMember> GroupMemberships { get; set; } = new List<GroupMember>();
        public ICollection<Expense> Expenses { get; set; } = new List<Expense>();

    }
    public class UserDevice
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        public string DeviceId { get; set; } = null!;
        public string Platform { get; set; } = null!; // android / ios / web
        public string FcmToken { get; set; } = null!;

        public bool IsActive { get; set; } = true;
        public DateTime LastSeenAt { get; set; } = DateTime.UtcNow;
    }


}
