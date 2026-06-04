using Microsoft.AspNetCore.Identity;

namespace QLSNT.Areas.Admin.ViewModel
{
    public class UserManagementVM
    {
        public string UserId { get; set; }

        public string CCCD { get; set; }

        public string HoTen { get; set; }

        public string Role { get; set; }

        public bool IsLocked { get; set; }
    }
}