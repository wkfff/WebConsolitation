using System;
using System.Diagnostics;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Extensions.EO15ExcCostsAIP.Services;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.EO15ExcCostsAIP
{
    public class EO15ExcCostsAIPExtension : IEO15ExcCostsAIPExtension
    {
        private readonly IClientService clientsRepository;
        private readonly IScheme scheme;
        private readonly IRepository<Users> userRepository;
        private readonly IRepository<DataSources> sourceRepository;

        public EO15ExcCostsAIPExtension(
            IScheme scheme, 
            IClientService clientsRepository, 
            IRepository<Users> userRepository,
            IRepository<DataSources> sourceRepository)
        {
            this.scheme = scheme;
            this.clientsRepository = clientsRepository;
            this.userRepository = userRepository;
            this.sourceRepository = sourceRepository;
        }

        public D_ExcCosts_Clients Client { get; private set; }

        public DataSources DataSource { get; private set; }

        public string OKTMO { get; private set; }

        public string UserGroup { get; private set; }

        public bool Initialize()
        {
            try
            {
                var currentUserId = scheme.UsersManager.GetCurrentUserID();
                var user = userRepository.Get(currentUserId);
                
                UserGroup = String.Empty;
                var groups = scheme.UsersManager.GetGroupsForUser(currentUserId);

                foreach (System.Data.DataRow row in groups.Rows)
                {
                    if (row.ItemArray[2].Equals(true))
                    {
                        var groupName = row.ItemArray[1].ToString();
                        if (groupName.Equals(AIPRoles.Coordinator))
                        {
                            UserGroup = AIPRoles.Coordinator;
                        }
                        else
                        {
                            if (groupName.Equals(AIPRoles.GovClient))
                            {
                                UserGroup = AIPRoles.GovClient;
                            }
                            else
                            {
                                if (groupName.Equals(AIPRoles.MOClient))
                                {
                                    UserGroup = AIPRoles.MOClient;
                                }
                                else
                                {
                                    if (groupName.Equals(AIPRoles.User))
                                    {
                                        UserGroup = AIPRoles.User;
                                    }
                                }
                            }
                        }
                    }
                }
                
                // по логину ищем заказчика или null, если такого нет
                Client = clientsRepository.GetAll().FirstOrDefault(x => user.Name.Equals(x.Login));

                // Источник ЭО 0053 Сбор АИП
                DataSource = sourceRepository.GetAll()
                    .FirstOrDefault(ds => ds.SupplierCode == "ЭО" && ds.DataCode == 53);
                
                OKTMO = Convert.ToString(scheme.GlobalConstsManager.Consts["OKTMO"].Value);

                if (DataSource == null)
                {
                    throw new Exception("Отсутствует источник ЭО 0053 Сбор АИП.");
                }

                return true;
            }
            catch (Exception e)
            {
                Trace.TraceError("Ошибка инициализации модуля: {0}", e.Message);

                return false;
            }
        }
    }
}
