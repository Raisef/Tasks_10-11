using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersBase.Entities;

namespace UsersBase.Interfaces
{
    public interface IAwardDao
    {
        bool CreateAward(Award award);
        bool RewardUser(int userId, int awardId);
        bool DeleteUserAward(int userId, int awardId);
        IEnumerable<Award> GetAllAwards();
    }
}
