using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wsds.DAL.Repository.Abstract;

namespace Wsds.DAL.Infrastructure.Facade
{
    public class AccountUserFacade
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IClientRepository _cliRepo;
        private readonly IAppLocalizationRepository _appLocalization;

        public AccountUserFacade(IUserRepository userRepository,
                                 IRoleRepository roleRepository,
                                 IClientRepository clientRepository,
                                 IAppLocalizationRepository appLocalization)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _cliRepo = clientRepository;
            _appLocalization = appLocalization;

        }

        public IUserRepository Users => _userRepository;
        public IRoleRepository Roles => _roleRepository;
        public IClientRepository Clients => _cliRepo;
        public IAppLocalizationRepository Locale => _appLocalization;
    }
}
