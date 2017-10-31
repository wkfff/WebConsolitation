using System;
using System.Collections.Generic;
using System.Data;
using Krista.FM.Common;
using Microsoft.Practices.ServiceLocation;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Domain.Services.FinSourceDebtorBook
{
    public class VariantService
    {
        private readonly IScheme scheme;

        public VariantService()
            : this(ServiceLocator.Current.GetInstance<IScheme>())
        { }

        public VariantService(IScheme scheme)
        {
            this.scheme = scheme;
        }

        /// <summary>
        /// ���������� ������� ������� �� ���������.
        /// </summary>
        public VariantDescriptor GetDefaultVariant()
        {
            IEntity entity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.d_Variant_Schuldbuch);
            using (IDataUpdater du = entity.GetDataUpdater())
            {
                DataSet dsVariant = new DataSet();
                du.Fill(ref dsVariant);
                foreach (DataRow variantRow in dsVariant.Tables[0].Rows)
                {
                    if (Convert.ToBoolean(variantRow["CurrentVariant"]))
                    {
                        return new VariantDescriptor(
                                       Convert.ToInt32(variantRow["ID"]),
                                       Convert.ToString(variantRow["CODE"]),
                                       Convert.ToString(variantRow["NAME"]),
                                       Convert.ToString(variantRow["VARIANTCOMMENT"]),
                                       Convert.ToInt32(variantRow["ACTUALYEAR"]),
                                       Convert.ToDateTime(variantRow["REPORTDATE"])
                                   );
                    }
                }
            }
            return new VariantDescriptor(
                -1,
                String.Empty,
                "������� �� ��������� �� ������",
                String.Empty,
                -1,
                DateTime.MinValue
            );
        }

        /// <summary>
        /// ���������� ������� �� ��� ID
        /// </summary>
        public VariantDescriptor GetVariant(int variantId)
        {
            IEntity entity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.d_Variant_Schuldbuch);
            using (IDataUpdater du = entity.GetDataUpdater("ID = ?", null, new DbParameterDescriptor("paramId", variantId)))
            {
                DataTable data = new DataTable();
                du.Fill(ref data);
                
                if (data.Rows.Count == 0 || data.Rows.Count > 1)
                {
                    throw new KeyNotFoundException("������� �� ������");
                }

                DataRow variantRow = data.Rows[0];
                {
                    return new VariantDescriptor(
                                       Convert.ToInt32(variantRow["ID"]),
                                       Convert.ToString(variantRow["CODE"]),
                                       Convert.ToString(variantRow["NAME"]),
                                       Convert.ToString(variantRow["VARIANTCOMMENT"]),
                                       Convert.ToInt32(variantRow["ACTUALYEAR"]),
                                       Convert.ToDateTime(variantRow["REPORTDATE"]));
                }
            }
        }
    }
}
