using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersBase.DalContracts;
using UsersBase.DBDal;
using UsersBase.FileDal;
using UsersBase.MemoryDal;

namespace UsersBase.DaoManager
{
    public class ManagerDao
    {
        private static ManagerDao _instance = new ManagerDao();
        private IAwardDao _awardDao;
        private IUserDao _userDao;
        private IAppUserDao _appUserDao;

        private ManagerDao()
        {
            string dalType = ConfigurationManager.AppSettings["DalType"];
            switch (dalType.ToLower())
            {
                case "files":
                    _awardDao = new FileAwardDao();
                    _userDao = new FileUserDao();
                    break;

                case "memory":
                    _awardDao = MemoryAwardDao.Instance;
                    _userDao = MemoryUserDao.Instance;
                    break;

                case "db":
                    var connectionString = ConfigurationManager.ConnectionStrings["default"].ConnectionString;
                    if (string.IsNullOrEmpty(connectionString))
                    {
                        throw new ConfigurationErrorsException("Invalid ConnectionString");
                    }
                    _awardDao = new DBAwardDao(connectionString);
                    _userDao = new DBUserDao(connectionString);
                    _appUserDao = new DBAppUserDao(connectionString);
                    break;

                default:
                    throw new ConfigurationErrorsException($"Invalid dalType:{dalType}");
            }
        }
        public static ManagerDao Instance
        {
            get
            {
                return _instance;
            }
        }
        public IAwardDao AwardDao() => _awardDao;
        public IUserDao UserDao() => _userDao;
        public IAppUserDao AppUserDao() => _appUserDao;
    }
}
