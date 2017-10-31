using System.Collections.Generic;
using System.Linq;
using Ext.Net;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory.NHibernate;

namespace Krista.FM.RIA.Extensions.FO41.Services
{
    public class CategoryTaxpayerService : NHibernateLinqRepository<D_Org_CategoryTaxpayer>, ICategoryTaxpayerService
    {
        public IQueryable GetQueryAll()
        {
            return from r in FindAll()
                   where r.ID != -1
                   orderby r.ShortName
                   select new
                   {
                       r.ID,
                       r.Name,
                       r.RowType,
                       RefTypeTaxpayerId = r.RefTypeTaxpayer.ID,
                       RefTypeTaxpayerName = r.RefTypeTaxpayer.Name,
                       r.ShortName,
                       RefOGVID = r.RefOGV.ID,
                       RefOGVName = r.RefOGV.Name,
                       r.CorrectIndex
                   };
        }

        /// <summary>
        /// Возвращает список категорий,  по которым указанный ОГВ отвественный за исключением указанных
        /// </summary>
        /// <param name="ogv">Идентификатор ОГВ</param>
        /// <param name="exceptIDs">Список идентификаторов категорий, которые не нужно показывать или null</param>
        /// <returns>Список категорий</returns>
        public IQueryable GetByOGV(int ogv, string exceptIDs)
        {
            var excepts = exceptIDs == null ? null : JSON.Deserialize<List<int>>(exceptIDs);
            if (excepts == null)
            {
                return from r in FindAll()
                       where r.ID != -1 && (r.RefOGV.ID == ogv || ogv == -1)
                       orderby r.ShortName
                       select new
                                  {
                                      r.ID,
                                      r.Name,
                                      r.RowType,
                                      RefTypeTaxpayerId = r.RefTypeTaxpayer.ID,
                                      RefTypeTaxpayerName = r.RefTypeTaxpayer.Name,
                                      r.ShortName,
                                      RefOGVID = r.RefOGV.ID,
                                      RefOGVName = r.RefOGV.Name,
                                      r.CorrectIndex
                                  };
            }

            return from r in FindAll()
                   where r.ID != -1 && (r.RefOGV.ID == ogv && !excepts.Contains(r.ID))
                   orderby r.ShortName
                   select new
                   {
                       r.ID,
                       r.Name,
                       r.RowType,
                       RefTypeTaxpayerId = r.RefTypeTaxpayer.ID,
                       RefTypeTaxpayerName = r.RefTypeTaxpayer.Name,
                       r.ShortName,
                       RefOGVID = r.RefOGV.ID,
                       RefOGVName = r.RefOGV.Name,
                       r.CorrectIndex
                   };
        }

        /// <summary>
        /// Возвращает список категорий за исключением указанных
        /// </summary>
        /// <param name="exceptIDs">Список идентификаторов категорий, которые не нужно показывать или null</param>
        /// <returns>Список категорий</returns>
        public IQueryable Get(string exceptIDs)
        {
            var excepts = exceptIDs == null ? null : JSON.Deserialize<List<int>>(exceptIDs);
            if (excepts == null)
            {
                return from r in FindAll()
                       where r.ID != -1
                       orderby r.ShortName
                       select new
                       {
                           r.ID,
                           r.Name,
                           r.RowType,
                           RefTypeTaxpayerId = r.RefTypeTaxpayer.ID,
                           RefTypeTaxpayerName = r.RefTypeTaxpayer.Name,
                           r.ShortName,
                           RefOGVID = r.RefOGV.ID,
                           RefOGVName = r.RefOGV.Name,
                           r.CorrectIndex
                       };
            }

            return from r in FindAll()
                   where r.ID != -1 && !excepts.Contains(r.ID)
                   orderby r.ShortName
                   select new
                   {
                       r.ID,
                       r.Name,
                       r.RowType,
                       RefTypeTaxpayerId = r.RefTypeTaxpayer.ID,
                       RefTypeTaxpayerName = r.RefTypeTaxpayer.Name,
                       r.ShortName,
                       RefOGVID = r.RefOGV.ID,
                       RefOGVName = r.RefOGV.Name,
                       r.CorrectIndex
                   };
        }

        /// <summary>
        /// Возвращает категорию по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор категории</param>
        /// <returns>Объект категории</returns>
        public D_Org_CategoryTaxpayer GetQueryOne(int id)
        {
            return FindOne(id);
        }

        public IList<D_Org_CategoryTaxpayer> GetByTax(int taxId)
        {
            return FindAll().Where(r => r.ID != -1 && r.RefTypeTax.ID == taxId).ToList();
        }
    }
}
