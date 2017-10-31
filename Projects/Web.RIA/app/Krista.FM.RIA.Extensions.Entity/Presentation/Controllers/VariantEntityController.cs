using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ext.Net.MVC;
using Krista.FM.Common;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.Entity
{
    public class VariantEntityController : SchemeBoundController
    {
        /// <summary>
        /// Копирование варианта.
        /// </summary>
        /// <param name="objectKey">Ключ классификатора вариантов.</param>
        /// <param name="variantId">Id копируемого варианта.</param>
        public AjaxResult Copy(string objectKey, int variantId)
        {
            IEntity entity = Scheme.RootPackage.FindEntityByName(objectKey);
            if (entity == null)
                return new AjaxResult("Классификатор вариантов не найден.");

            if (!(entity is IVariantDataClassifier))
                return new AjaxResult("Классификатор не является классификатором вариантов.");

            RemoteMetod metod = new RemoteMetod();
            int id = ((IVariantDataClassifier)entity).CopyVariant(variantId);

            return new AjaxResult(id);
        }
    }
}
