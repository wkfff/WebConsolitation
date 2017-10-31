using System;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;

namespace Krista.FM.RIA.Extensions.E86N.Services.StateTaskService
{
    public class StateTaskService : NewRestService, IStateTaskService
    {
        #region IStateTaskService Members

        public virtual void DeleteDoc(int id)
        {
            try
            {
               GetItems<F_F_GosZadanie>().Where(p => p.RefParametr.ID == id).Each(p =>
                                                                                {
                                                                                    DeleteDetails(p.ID);
                                                                                    Delete(p);
                                                                                });

               GetItems<F_F_OrderControl>().Where(p => p.RefFactGZ.ID == id).Each(Delete);

               GetItems<F_F_RequestAccount>().Where(p => p.RefFactGZ.ID == id).Each(Delete);

               GetItems<F_F_BaseTermination>().Where(p => p.RefFactGZ.ID == id).Each(Delete);

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
                GetItems<F_F_GosZadanie>().Where(p => p.RefParametr.ID == id).Each(p =>
                {
                    DeleteDetails(p.ID);
                    Delete(p);
                });

                GetItems<F_F_OrderControl>().Where(p => p.RefFactGZ.ID == id).Each(Delete);

                GetItems<F_F_RequestAccount>().Where(p => p.RefFactGZ.ID == id).Each(Delete);

                GetItems<F_F_BaseTermination>().Where(p => p.RefFactGZ.ID == id).Each(Delete);
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
                GetItems<F_F_GZYslPotr>().Where(p => p.RefFactGZ.ID == id).Each(Delete);

                GetItems<F_F_PNRZnach>().Where(p => p.RefFactGZ.ID == id).Each(Delete);

                GetItems<F_F_NPACena>().Where(p => p.RefGZPr.ID == id).Each(Delete);

                GetItems<F_F_NPARenderOrder>().Where(p => p.RefFactGZ.ID == id).Each(Delete);

                GetItems<F_F_LimitPrice>().Where(p => p.RefFactGZ.ID == id).Each(Delete);

                GetItems<F_F_InfoProcedure>().Where(p => p.RefFactGZ.ID == id).Each(Delete);
            }
            catch (Exception e)
            {
                throw new Exception("Ошибка удаления детализаций для услуг\\работ: " + e.Message, e);
            }
        }

        #endregion
    }
}