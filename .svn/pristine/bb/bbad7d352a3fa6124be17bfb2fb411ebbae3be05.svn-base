using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ext.Net.MVC;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory.NHibernate.IoC;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Extensions.FO41.Services;

namespace Krista.FM.RIA.Extensions.FO41.Presentation.Controllers
{
    public class FO41CategoriesController : SchemeBoundController
    {
        private readonly CategoryTaxpayerService categoryRepository;
        private readonly IFO41Extension extension;

        public FO41CategoriesController(
            CategoryTaxpayerService categoryRepository,
            IFO41Extension extension)
        {
            this.extension = extension;
            this.categoryRepository = categoryRepository;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [Transaction]
        public RestResult Create(string data)
        {
            return new RestResult { Success = false, Message = string.Empty };
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public RestResult Read()
        {
            try
            {
                // для налогоплательщика возвращать только категории по его заявкам
                // для ОГВ должны отображаться те категории, у которых в классификаторе 
                // «Организации.Категория налогоплательщика по льготам (d.Org.CategoryTaxpayer)» 
                // в поле «Отвественный ОГВ (RefOGV)» стоит соответствующий ОГВ. 
                return new RestResult
                           {
                               Success = true, 
                               Data = 
                                    extension.ResponsOIV == null 
                                    ? null
                                    : (extension.ResponsOIV.Role.Equals("ДФ") 
                                        ? categoryRepository.GetQueryAll() 
                                        : categoryRepository.GetByOGV(extension.ResponsOIV.ID, null))
                           };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public RestResult ReadOne(int id)
        {
            try
            {
                return new RestResult { Success = true, Data = categoryRepository.Get(id) };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        public IList<D_Org_CategoryTaxpayer> GetCategories()
        {
            return categoryRepository.FindAll().ToList();
        }
    }
}
