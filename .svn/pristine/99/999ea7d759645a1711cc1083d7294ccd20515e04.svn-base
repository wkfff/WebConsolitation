using System;
using System.Collections.Generic;
using System.Linq;
using bus.gov.ru.external.Item1;
using bus.gov.ru.types.Item1;
using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Extensions.E86N.Auth.Services;
using GlobalConsts = Krista.FM.RIA.Extensions.E86N.Utils.GlobalConsts;

namespace Krista.FM.RIA.Extensions.E86N.Services.Export
{
    public static class ExportInspectionActivityService
    {
        public static byte[] Serialize(IAuthService authService, F_F_ParameterDoc header)
        {
            D_Org_Structure target = header.RefUchr;
            D_Org_UserProfile placerProfile = authService.Profile;
            if (placerProfile == null)
            {
                throw new InvalidOperationException(GlobalConsts.NullProfile);
            }

            List<F_Doc_Docum> documents = header.Documents.ToList();
            int year = header.RefYearForm.ID;

            var position =
                new inspectionActivityType
                    {
                        positionId = Guid.NewGuid().ToString(),
                        changeDate = DateTime.Now,
                        placer = ExportServiceHelper.RefNsiOgsExtendedType(placerProfile.RefUchr),
                        initiator = target.ID != placerProfile.RefUchr.ID
                                        ? ExportServiceHelper.RefNsiOgsExtendedType(target)
                                        : null,
                        period = year,
                        document = ExportServiceHelper.Documents(documents),
                        inspectionEvent = header.InspectionEvent.Select(
                            inspectionEvent => new inspectionActivityType.inspectionEventLocalType
                                                   {
                                                       eventBegin = inspectionEvent.EventBegin,
                                                       eventEnd = inspectionEvent.EventEnd,
                                                       supervisor = new refNsiConsRegSoftType
                                                                        {
                                                                            fullName = inspectionEvent.Supervisor
                                                                        },
                                                       topic = inspectionEvent.Topic
                                                   }
                                                   .Do(
                                                       type => inspectionEvent.ResultActivity
                                                                   .If(s => s.IsNotNullOrEmpty())
                                                                   .Do(s => type.resultActivity = s))
                                                   .Do(
                                                       type => inspectionEvent.Violation
                                                                   .If(s => s.IsNotNullOrEmpty())
                                                                   .Do(s => type.violation = s)))
                            .ToList()
                    };

            return ExportServiceHelper.Serialize(
                new inspectionActivity
                    {
                        header = ExportServiceHelper.HeaderType(),
                        body = new inspectionActivity.bodyLocalType { position = position }
                    }.Save);
        }
    }
}
