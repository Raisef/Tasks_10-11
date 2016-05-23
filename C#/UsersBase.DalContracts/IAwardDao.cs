using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersBase.Entities;

namespace UsersBase.DalContracts
{
    public interface IAwardDao
    {
        int Create(Award award);
        bool AwardUser(KeyValuePair<int, string> user, int awardId);
        bool RemoveUserAward(int userId, int awardId);
        IEnumerable<Award> GetAll();
        Award Get(int awardId);
        bool Edit(int awardId, string awardName);
        bool Delete(int awardId);
        bool SetImage(int awardId, byte[] image, string imageType);
        bool RemoveImage(int awardId);
        byte[] GetImage(int awardId);
        string GetImageType(int awardId);
    }
}
