namespace Mogre.Builder.Tasks
{
    abstract class MsBuildTask : Task
    {
        protected MsBuildManager msBuildMgr;



        public MsBuildTask(OutputManager outputMgr, MsBuildManager msBuildMgr)
            : base(outputMgr)
        {
            this.msBuildMgr = msBuildMgr;
        }
    }
}