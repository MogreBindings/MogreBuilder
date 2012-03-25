namespace Mogre.Builder.Tasks
{
    abstract class MsBuildTask : Task
    {
        protected MsBuildManager msBuildManager;

        public MsBuildTask(InputManager inputManager, IOutputManager outputManager, MsBuildManager msBuildManager)
            : base(inputManager, outputManager)
        {
            this.msBuildManager = msBuildManager;
        }
    }
}