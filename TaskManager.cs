using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre.Builder.Tasks;

namespace Mogre.Builder
{
    public class TaskManager
    {
        private InputManager _inputManager;
        private IOutputManager _outputManager;

        public TaskManager(InputManager inputManager, IOutputManager outputManager)
        {
            _inputManager = inputManager;
            _outputManager = outputManager;
        }
        
        private static List<Task> BuildTaskList(InputManager inputManager, IOutputManager outputManager)
        {
            var tasks = new List<Task>(20);
            var msBuildManager = new MsBuildManager(outputManager);

            tasks.Add(new CloneMogreSource(inputManager, outputManager));

            tasks.Add(new CheckTargetDir(inputManager, outputManager));            
            tasks.Add(new CheckEnvironment(inputManager, outputManager));
            
            tasks.Add(new CloneOgreSource(inputManager, outputManager));
            // patch
            tasks.Add(new PatchOgreCode(inputManager, outputManager));
            tasks.Add(new OgreDependencies(inputManager, outputManager, msBuildManager));
            tasks.Add(new OgreCmake(inputManager, outputManager));
            // Auto-wrapping
            tasks.Add(new AutoWrap(inputManager, outputManager, msBuildManager));
            tasks.Add(new AddClrClassesToOgre(inputManager, outputManager));
            // Building
            tasks.Add(new BuildOgreWithoutMogreLinking(inputManager, outputManager, msBuildManager));
            tasks.Add(new BuildMogre(inputManager, outputManager, msBuildManager));
            tasks.Add(new BuildOgreWithMogreLinking(inputManager, outputManager, msBuildManager));
            // Organizing the result
            tasks.Add(new AssembleBinaryFiles(inputManager, outputManager));

            return tasks;
        }

        public bool Run()
        {
            try
            {
                var tasksToRun = BuildTaskList(_inputManager, _outputManager);

                foreach (var task in tasksToRun)
                {
                    _outputManager.Action(task.Name);
                    task.Run();
                }
            }
            catch (Exception ex)
            {
                _outputManager.Error(ex.Message);
                return false;
            }

            return true;
        }
    }
}
