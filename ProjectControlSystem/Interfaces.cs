using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectControlSystem
{
    public interface IProjectViewControl
    {
        ProjectViewMode Mode { get; }
        string Header { get; }
        Project SelectedProject { get; }

        void Init();
        void Unload();

        void Select(int projectId);
    }

    public interface IFilterChecker
    {
        bool IsFilterPassed(Project p);
    }

}
