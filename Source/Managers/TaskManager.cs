﻿using System;
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
        


        /// <summary>
        /// Create a list of all needed tasks (related to the choosed options).
        /// </summary>
        private static List<Task> BuildTaskList(InputManager inputManager, IOutputManager outputManager)
        {
            List<Task> taskList = new List<Task>();
            MsBuildManager msBuildManager = new MsBuildManager(outputManager);

            if (inputManager.Option_OnlyAddons == false)
            {
                //--- tasks to build Ogre/Mogre ---

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

                // apply cygon patch
                taskList.Add(new ApplyCygonPatch(inputManager, outputManager));

                // Building
                taskList.Add(new BuildOgreWithoutMogreLinking(inputManager, outputManager, msBuildManager));
                taskList.Add(new UpdateMogreVersion(inputManager, outputManager));
                taskList.Add(new BuildMogre(inputManager, outputManager, msBuildManager));
                taskList.Add(new BuildOgreWithMogreLinking(inputManager, outputManager, msBuildManager));

                // Organizing the result
                taskList.Add(new AssembleBinaryFiles(inputManager, outputManager));

            }


            //--- optional add-on tasks ---

            // clone official Mogre-addons repository   (if one of the included add-ons is needed)
            if (inputManager.Option_MogreNewt || inputManager.Option_Hikari || inputManager.Option_Makuri ||
                inputManager.Option_MogreDesignSupport || inputManager.Option_MogreFreeSL)
            {
                taskList.Add(new CloneAddonsRepository(inputManager, outputManager));
            }


            // build MogreNewt
            if (inputManager.Option_MogreNewt)
            {
                taskList.Add(new NewtonLibraryDownload(inputManager, outputManager));
                taskList.Add(new NewtonPrepatation(inputManager, outputManager));

            }


            return taskList;
        } // BuildTaskList()




        /// <summary>
        /// Process the task.
        /// </summary>
        /// <returns>Returns false if aborted by an exception.</returns>
        public bool Run()
        {
            try
            {
                List<Task> tasksToRun = BuildTaskList(_inputManager, _outputManager);

                foreach (Task task in tasksToRun)
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
        } // Run()


    }
}