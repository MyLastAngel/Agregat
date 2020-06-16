using System.Windows.Input;

namespace ProjectControlSystem
{
    public static class CustomCommands
    {
        public static readonly RoutedUICommand CreateProject;

        static CustomCommands()
        {
            CreateProject = new RoutedUICommand("Новый проект", "CreateProject", typeof(CustomCommands));
        }
    }
}
