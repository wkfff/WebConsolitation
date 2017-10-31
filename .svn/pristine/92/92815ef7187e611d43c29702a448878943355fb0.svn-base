using System;
using System.Linq;
using Krista.FM.Domain.Reporitory.NHibernate;

namespace Krista.FM.Domain.Services.FinSourceDebtorBook
{
    public class VariantProtocolService : IVariantProtocolService
    {
        private readonly NHibernateLinqRepository<D_Variant_Schuldbuch> variantRepository;
        private readonly NHibernateLinqRepository<D_Regions_Analysis> regionRepository;
        private readonly NHibernateLinqRepository<FX_S_StatusSchb> statusSchbRepository;
        private readonly NHibernateLinqRepository<T_S_ProtocolTransfer> repository;

        public VariantProtocolService()
        {
            statusSchbRepository = new NHibernateLinqRepository<FX_S_StatusSchb>();
            variantRepository = new NHibernateLinqRepository<D_Variant_Schuldbuch>();
            regionRepository = new NHibernateLinqRepository<D_Regions_Analysis>();
            repository = new NHibernateLinqRepository<T_S_ProtocolTransfer>();
        }

        public T_S_ProtocolTransfer GetStatus(int variantId, int regionId)
        {
            var status = repository.FindAll()
                .Where(x => x.RefVariant.ID == variantId && x.RefRegion.ID == regionId)
                .OrderByDescending(x => x.ID).FirstOrDefault();

            if (status == null)
            {
                status = new T_S_ProtocolTransfer {
                    RefStatusSchb = statusSchbRepository.FindOne(1),
                    RefVariant = variantRepository.FindOne(variantId),
                    RefRegion = regionRepository.FindOne(regionId)
                };
            }
            
            return status;
        }

        /// <summary>
        /// Отправить для района его вариант на проверку в субъект.
        /// </summary>
        /// <param name="variantId"></param>
        /// <param name="regionId"></param>
        /// <param name="comment"></param>
        public T_S_ProtocolTransfer ToTest(int variantId, int regionId, string comment)
        {
            var status = GetStatus(variantId, regionId);
            if (status.RefStatusSchb.ID != 1)
            {
                throw new VariantProtocolException("Невозможно отправить изменения на рассмотрение.");
            }

            var protocolStatus = new T_S_ProtocolTransfer
            {
                CheckTransfer = false,
                DataTransfer = String.Format("{0:dd.MM.yyyy hh:mm:ss}", DateTime.Now),
                Commentary = comment,
                RefStatusSchb = statusSchbRepository.FindOne(2), // На рассмотрении
                RefVariant = variantRepository.FindOne(variantId),
                RefRegion = regionRepository.FindOne(regionId)
            };

            repository.Save(protocolStatus);
            
            return protocolStatus;
        }

        public void Reject(int variantId, int regionId, string comment)
        {
            var status = GetStatus(variantId, regionId);
            if (status.RefStatusSchb.ID == 1)
            {
                throw new VariantProtocolException("Невозможно отклонить изменения.");
            }

            var protocolStatus = new T_S_ProtocolTransfer
            {
                CheckTransfer = false,
                DataTransfer = String.Format("{0:dd.MM.yyyy hh:mm:ss}", DateTime.Now),
                Commentary = comment,
                RefStatusSchb = statusSchbRepository.FindOne(1), // На редактировании МО
                RefVariant = variantRepository.FindOne(variantId),
                RefRegion = regionRepository.FindOne(regionId)
            };

            repository.Save(protocolStatus);
        }

        public void Accept(int variantId, int regionId, string comment)
        {
            var status = GetStatus(variantId, regionId);
            if (status.RefStatusSchb.ID != 2)
            {
                throw new VariantProtocolException("Невозможно утвердить изменения.");
            }

            var protocolStatus = new T_S_ProtocolTransfer
            {
                CheckTransfer = false,
                DataTransfer = String.Format("{0:dd.MM.yyyy hh:mm:ss}", DateTime.Now),
                Commentary = comment,
                RefStatusSchb = statusSchbRepository.FindOne(3), // Утвержден
                RefVariant = variantRepository.FindOne(variantId),
                RefRegion = regionRepository.FindOne(regionId)
            };

            repository.Save(protocolStatus);
        }
    }
}
