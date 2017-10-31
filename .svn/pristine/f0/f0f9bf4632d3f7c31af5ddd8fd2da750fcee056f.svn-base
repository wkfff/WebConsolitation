using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

using Ext.Net;

using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Extensions;
using Krista.FM.RIA.Extensions.E86N.Utils;

namespace Krista.FM.RIA.Extensions.E86N.Services.DocService
{
    internal class DocService : IDocService
    {
        private readonly ILinqRepository<F_F_ParameterDoc> paramDocRepository;
        private readonly ILinqRepository<D_Doc_TypeDoc> typeDocRepository;
        private readonly ILinqRepository<F_Doc_Docum> docRepository;

        public DocService(
            ILinqRepository<F_Doc_Docum> docRepository,
            ILinqRepository<F_F_ParameterDoc> paramDocRepository,
            ILinqRepository<D_Doc_TypeDoc> typeDocRepository)
        {
            this.paramDocRepository = paramDocRepository;
            this.typeDocRepository = typeDocRepository;
            this.docRepository = docRepository;
        }

        #region IDocService Members

        public IEnumerable<F_Doc_Docum> GetItems(int? parentId)
        {
            if (parentId == null)
            {
                throw new InvalidDataException("DocService::GetItems: Не указан родительский документ");
            }

            /* Нужно проверить, существуют ли все необходимые документы и создать недостающие.
            /* Обязательные значения:
             * - F - учредительные документы;
             * - E - свидетельства о государственной регистрации;
             * - S - решения учредителя о назначении руководителя;
             * - L - положения о филиалах, представительствах;
             * - I - правовой акт о назначении членов наблюдательного совета;
             * Проверять наличие по коду.
             * 
             * Также допустимо:
             * - O - прочие документы.                         
             */

            //// новый фейковый тип при экспорте уходит в прочие с литерой "О"
            //// Решение учредителя о создании учреждения
            FX_FX_PartDoc docType = paramDocRepository.FindOne(parentId.Value).RefPartDoc;

            // осознаннно пишу говнокод)
            switch (docType.ID)
            {
                case FX_FX_PartDoc.StateTaskDocTypeID:
                    {
                        TestExistenseDocOfTypes(parentId.Value, new[] { "C" });
                        break;
                    }

                case FX_FX_PartDoc.PfhdDocTypeID:
                    {
                        TestExistenseDocOfTypes(parentId.Value, new[] { "P", "A" });
                        break;
                    }

                case FX_FX_PartDoc.SmetaDocTypeID:
                    {
                        TestExistenseDocOfTypes(parentId.Value, new[] { "B" });
                        break;
                    }

                case FX_FX_PartDoc.PassportDocTypeID:
                    {
                        TestExistenseDocOfTypes(parentId.Value, new[] { "F", "E", "S", "L", "I", "O", "X" });
                        break;
                    }

                case FX_FX_PartDoc.ResultsOfActivityDocTypeID:
                    {
                        TestExistenseDocOfTypes(parentId.Value, new[] { "D" });
                        break;
                    }

                case FX_FX_PartDoc.InfAboutControlActionsDocTypeID:
                    {
                        TestExistenseDocOfTypes(parentId.Value, new[] { "G", "H", "J" });
                        break;
                    }

                case FX_FX_PartDoc.AnnualBalanceF0503730Type:
                    {
                        TestExistenseDocOfTypes(parentId.Value, new[] { "K" });
                        break;
                    }

                case FX_FX_PartDoc.AnnualBalanceF0503121Type:
                    {
                        TestExistenseDocOfTypes(parentId.Value, new[] { "R" });
                        break;
                    }

                case FX_FX_PartDoc.AnnualBalanceF0503127Type:
                    {
                        TestExistenseDocOfTypes(parentId.Value, new[] { "T" });
                        break;
                    }

                case FX_FX_PartDoc.AnnualBalanceF0503130Type:
                    {
                        TestExistenseDocOfTypes(parentId.Value, new[] { "Q" });
                        break;
                    }

                case FX_FX_PartDoc.AnnualBalanceF0503137Type:
                    {
                        TestExistenseDocOfTypes(parentId.Value, new[] { "U" });
                        break;
                    }

                case FX_FX_PartDoc.AnnualBalanceF0503721Type:
                    {
                        TestExistenseDocOfTypes(parentId.Value, new[] { "M" });
                        break;
                    }

                case FX_FX_PartDoc.AnnualBalanceF0503737Type:
                    {
                        TestExistenseDocOfTypes(parentId.Value, new[] { "N", "2", "4", "5", "6", "7" });
                        break;
                    }
            }
            
            return from p in docRepository.FindAll()
                   where p.RefParametr.ID == parentId
                   select p;
        }

        public void Delete(int id)
        {
            try
            {
                F_Doc_Docum doc = docRepository.FindOne(id);
                DeleteFile(doc);
                docRepository.Delete(doc);
            }
            catch (Exception e)
            {
                throw new Exception("RestService::Delete: Ошибка удаления записи: " + e.Message, e);
            }
        }

        public void DeleteFile(F_Doc_Docum doc)
        {
            string fileName = FindFile(doc);
            if (File.Exists(fileName))
            {
                File.Delete(fileName); // note: Есть вероятность накопления пустых папок.
            }

            // else //По идее если файл не найден, то можно тихо ничего не делать.
            // note: Есть вероятность накопления файлов, о которых нет данных в БД.
            doc.Url = "НетФайла";
            doc.FileSize = null;
            docRepository.Save(doc);
        }

        public string FindFile(F_Doc_Docum doc)
        {
            // Новые файлы хранятся по иерархическому принципу.
            string fileNameHierarchy = DocHelpers.GetLocalFilePath(doc) + doc.Url;
            if (File.Exists(fileNameHierarchy))
            {
                return fileNameHierarchy;
            }

            // Файл в иерархии не нашли, значит он может лежать просто по пути хранения без иерархии.
            string fileNameFlat =
                ConfigurationManager.AppSettings["DocFilesSavePath"]
                + "\\" + doc.Url;
            if (File.Exists(fileNameFlat))
            {
                return fileNameFlat;
            }

            // Файла нет нигде...
            return string.Empty;
        }

        #endregion

        private void TestExistenseDocOfTypes(int parentId, string[] docTypeCode)
        {
            if (docTypeCode != null)
            {
                IQueryable<F_Doc_Docum> data = from p in docRepository.FindAll()
                                               where p.RefParametr != null && p.RefParametr.ID == parentId
                                               select p;

                List<string> list2 = data.Select(x => x.RefTypeDoc.Code).ToList();

                docTypeCode.Except(list2).Distinct().Each(x => FillNew(Convert.ToInt32(parentId), x));
            }
        }

        private void FillNew(int parentId, string docTypeCode)
        {
            try
            {
                F_F_ParameterDoc refParameter = JsonUtils.ParseRepositoryId(
                                                                            paramDocRepository,
                                                                            new JsonObject { { "ID", parentId } },
                                                                            "ID",
                                                                            "Не указан родительский документ");
                IQueryable<D_Doc_TypeDoc> refTypeDocList =
                    typeDocRepository.FindAll().Where(x => (x.Code == docTypeCode));
                if (refTypeDocList.Count() != 1)
                {
                    throw new InvalidDataException("Не найден тип документа-приложения: '" + docTypeCode + "'");
                }

                D_Doc_TypeDoc refTypeDoc = refTypeDocList.First();

                var name = refTypeDoc.Name.Length > 126 ? refTypeDoc.Name.Remove(126) : refTypeDoc.Name;

                var doc =
                    new F_Doc_Docum
                        {
                            SourceID = 0,
                            TaskID = 0,
                            RefParametr = refParameter,
                            RefTypeDoc = refTypeDoc,
                            Name = "(" + name + ")",
                            Url = "НетФайла",
                            DocDate = DateTime.Now,
                            NumberNPA = "НеУказан",
                    };
                docRepository.Save(doc);
            }
            catch (Exception e)
            {
                throw new InvalidDataException(
                    "DocService::FillNew: Ошибка заполнения нового документа-приложения: " + e.Message, e);
            }
        }
    }
}