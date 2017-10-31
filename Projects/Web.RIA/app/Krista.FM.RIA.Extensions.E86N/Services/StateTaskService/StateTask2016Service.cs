using System;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;

namespace Krista.FM.RIA.Extensions.E86N.Services.StateTaskService
{
    public class StateTask2016Service : NewRestService, IStateTask2016Service
    {
        public virtual void DeleteDoc(int id)
        {
            try
            {
                GetItems<F_F_GosZadanie2016>().Where(p => p.RefParameter.ID == id).Each(p =>
                {
                    DeleteDetails(p.ID);
                    Delete(p);
                });

                GetItems<F_F_OrderControl2016>().Where(p => p.RefFactGZ.ID == id).Each(Delete);
                GetItems<F_F_RequestAccount2016>().Where(p => p.RefFactGZ.ID == id).Each(Delete);
                GetItems<F_F_BaseTermination2016>().Where(p => p.RefFactGZ.ID == id).Each(Delete);
                GetItems<F_F_OtherInfo>().Where(p => p.RefFactGZ.ID == id).Each(Delete);
                GetItems<F_F_Reports>().Where(p => p.RefFactGZ.ID == id).Each(Delete);

                GetItems<T_F_ExtHeader>().Where(p => p.RefParametr.ID == id).Each(Delete);
            }
            catch (Exception e)
            {
                throw new Exception("Ошибка удаления документа ГЗ: " + e.Message, e);
            }
        }

        /// <summary>
        /// Удаление данных документа при проставлениее признака "Не доводить ГЗ"
        /// </summary>
        public virtual void DeleteDocForNotBring(int id)
        {
            try
            {
                GetItems<F_F_GosZadanie2016>().Where(p => p.RefParameter.ID == id).Each(p =>
                {
                    DeleteDetails(p.ID);
                    Delete(p);
                });

                GetItems<F_F_OrderControl2016>().Where(p => p.RefFactGZ.ID == id).Each(Delete);
                GetItems<F_F_RequestAccount2016>().Where(p => p.RefFactGZ.ID == id).Each(Delete);
                GetItems<F_F_BaseTermination2016>().Where(p => p.RefFactGZ.ID == id).Each(Delete);
                GetItems<F_F_OtherInfo>().Where(p => p.RefFactGZ.ID == id).Each(Delete);
                GetItems<F_F_Reports>().Where(p => p.RefFactGZ.ID == id).Each(Delete);
            }
            catch (Exception e)
            {
                throw new Exception("Ошибка удаления документа ГЗ: " + e.Message, e);
            }
        }

        public virtual void DeleteDetails(int id)
        {
            try
            {
                GetItems<F_F_GZYslPotr2016>().Where(p => p.RefFactGZ.ID == id).Each(Delete);
                DeleteIndexes(id);
                GetItems<F_F_NPACena2016>().Where(p => p.RefFactGZ.ID == id).Each(Delete);
                GetItems<F_F_NPARenderOrder2016>().Where(p => p.RefFactGZ.ID == id).Each(Delete);
                GetItems<F_F_InfoProcedure2016>().Where(p => p.RefFactGZ.ID == id).Each(Delete);
            }
            catch (Exception e)
            {
                throw new Exception("Ошибка удаления детализаций для услуг\\работ: " + e.Message, e);
            }
        }

        private void DeleteIndexes(int id)
        {
            GetItems<F_F_AveragePrice>().Where(p => p.RefVolumeIndex.RefFactGZ.ID == id).Each(Delete);
            GetItems<F_F_PNRZnach2016>().Where(p => p.RefFactGZ.ID == id).Each(Delete);
        }
    }
}