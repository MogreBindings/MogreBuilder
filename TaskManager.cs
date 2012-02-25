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
            var taskList = new List<Task>();
            var msBuildManager = new MsBuildManager(outputManager);
                        
            taskList.Add(new CloneMogreSource(inputManager, outputManager));

            taskList.Add(new CheckTargetDir(inputManager, outputManager));            
            taskList.Add(new CheckEnvironment(inputManager, outputManager));
            
            taskList.Add(new CloneOgreSource(inputManager, outputManager));

            // patch
            taskList.Add(new PatchOgreCode(inputManager, outputManager));
            taskList.Add(new OgreDependencies(inputManager, outputManager, msBuildManager));
            taskList.Add(new OgreCmake(inputManager, outputManager));

            // Auto-wrapping
            taskList.Add(new AutoWrap(inputManager, outputManager, msBuildManager));
            taskList.Add(new AddClrClassesToOgre(inputManager, outputManager));

            // Building
            taskList.Add(new BuildOgreWithoutMogreLinking(inputManager, outputManager, msBuildManager));
            taskList.Add(new UpdateMogreVersion(inputManager, outputManager));
            taskList.Add(new BuildMogre(inputManager, outputManager, msBuildManager));
            taskList.Add(new BuildOgreWithMogreLinking(inputManager, outputManager, msBuildManager));

            // Organizing the result
            taskList.Add(new AssembleBinaryFiles(inputManager, outputManager));

            return taskList;
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
