using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsersBase.Entities
{
    public class AppUser
    {
        public string Login { get; set; }
        public string NickName { get; set; }
        public string Role { get; set; }
        public string ImageType { get; set; }
    }
}
