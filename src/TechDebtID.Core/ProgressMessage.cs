namespace TechDebtID.Core
{
    public class ProgressMessage
    {
        public int ProjectsProcessed { get; set; }
        public int RootProjectsProcessed { get; set; }

        public ProgressMessage()
        {
            ProjectsProcessed = 0;
            RootProjectsProcessed = 0;
        }
    }
}
