using System;
using System.Configuration;
using UsersBase.ExceptionLogger;
using UsersBase.Logic;
using UsersBase.LogicContracts;

namespace UsersBase.WebUI
{
    public class LogicManager
    {
        private static LogicManager _instance = new LogicManager();
        IUserLogic _userLogic;
        IAwardLogic _awardLogic;
        IAppUserLogic _appUserLogic;

        private LogicManager()
        {
            try
            {
                _userLogic = new UserLogic();
                _awardLogic = new AwardLogic();
                _appUserLogic = new AppUserLogic();
            }
            catch (ConfigurationErrorsException ex)
            {
                ExceptionLog.LogError($"PL-{ex.Source}", $"Error in configurations: {ex.Message}", DateTime.Now);
                throw;
            }
        }

        public static LogicManager Instance
        {
            get
            {
                return _instance;
            }
        }

        public IAwardLogic AwardLogic() => _awardLogic;

        public IUserLogic UserLogic() => _userLogic;

        public IAppUserLogic AppUserLogic() => _appUserLogic;
    }
}