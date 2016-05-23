using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersBase.Entities;

namespace UsersBase.LogicContracts
{
    public interface IAwardLogic
    {
        int CreateAward(string award);
        bool RewardUser(int userId, int awardId);
        bool DeleteUserAward(int userId, int awardId);
        IEnumerable<string> GetAll();
        
    }
}
