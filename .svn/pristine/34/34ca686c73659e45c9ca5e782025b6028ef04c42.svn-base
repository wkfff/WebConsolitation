using System.Linq;

namespace Krista.FM.RIA.Extensions.Consolidation
{
    public class TaskPermisionSettings
    {
        public TaskPermisionSettings(IUserSessionState sessionState, TaskViewModel taskViewModel)
        {
            var isParentSubject = sessionState.Subjects.Any(x => x.ID == taskViewModel.ParentSubjectId);
            var isOwnerSubject = sessionState.Subjects.Any(x => x.ID == taskViewModel.SubjectId);

            CanChangeDeadline = isParentSubject;
            CanSetVise = isParentSubject;
            CanEditTask = isOwnerSubject || isParentSubject;
            CanViewTask = sessionState.User != null;
            CanPumpReport = isParentSubject;
        }

        public bool CanChangeDeadline { get; private set; }

        public bool CanSetVise { get; private set; }

        public bool CanEditTask { get; private set; }

        public bool CanViewTask { get; private set; }

        public bool CanPumpReport { get; private set; }
    }
}
