using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsersBase.Entities
{
    public class AwardedUser : User
    {
        public HashSet<string> Awards { get; set; }

        public AwardedUser(User user, HashSet<string> awards)
        {
            Id = user.Id;
            Name = user.Name;
            BirthDate = user.BirthDate;
            ImageType = user.ImageType;
            Awards = new HashSet<string>(awards);

        }
    }
}
