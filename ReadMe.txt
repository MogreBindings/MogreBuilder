
--- Preparation ---

1.
You need Microsoft Visual Studio 2010, 2012 or 2013. 
If you don't have it, you can use the free express versions.
..... In this case install both: Visual C# Express AND Visual C++ Express.
..... http://www.microsoft.com/visualstudio/en-us/products/2010-editions/visual-csharp-express
..... http://www.microsoft.com/visualstudio/en-us/products/2010-editions/visual-cpp-express
VS2010 is supported by default.
For VS2012 / 2013 you need to use the command-line switch -vs2012 / -vs2013.

2.
Install TortoiseHg. 
http://tortoisehg.bitbucket.org/download/index.html

3.
Install Cmake.
Important: Choose the setup option "Add CMake to the system PATH".
http://www.cmake.org/cmake/resources/software.html   

4.
Install the DirectX SDK (June 2010)
http://www.microsoft.com/download/en/details.aspx?id=6812



--- Step by step ---

1. Download MogreBuilder source from:
   https://bitbucket.org/mogre/mogrebuilder

2. Compile MogreBuilder by use of file "MogreBuilder.sln".

3. Enter directory "bin_Debug", open file "RUN_example.bat",
   and type in your wanted target directory.
   (For use with VS2012 / 2013 open the *.bat file and add the command-line switch "-vs2012" / "-vs2013" to the end of the first line.)
 
4. Setup the config file if required. There is a Default.cfg as an example. 
   You can specify a different one using the command line options. e.g.
   MogreBuilder C:\Mogre -config Default.cfg

5. Call "RUN_example.bat".
   Alternatively you can call the MogreBuilder from command line.

The auto-builder should now ran successfully.
(Be patient during the repository cloning tasks at the first usage.)
After the downloads are finished, the build process needs about 1 or 2 hours.



--- Command-line options ---

Aside the config files the following command-line options are available:
-config <filename>          Use <filename> as config file
-vs2012                     Use Visual Studio 2012 to build (default is VS 2010)
-vs2013                     Use Visual Studio 2013 to build
-x64                        Build for x64 (default is x86)
-priority <priority>        Use specified process priority. Priorities are 'Idle', 'BelowNormal', 'AboveNormal'
-noboost                    Build Ogre without boost (doesn't work yet)
-noupdate                   Do not update repositories from remote source. Use latest local commit instead.
-skipcmake                  Do not run CMake (use existing files)
-development                Do not catch exceptions in MogreBuilder
-mogrenewt                  Build MogreNewt add-on (does preparations, but need some manual steps)
                            Instructions you find here: http://www.ogre3d.org/addonforums/viewtopic.php?p=100289#p100289
-mois                       Build MOIS add-on
-onlyaddons                 Skip building Mogre, only build enabled add-ons (only useful if Mogre was already built)



--- Feedback ---

Please give feedback in the MogreBuilder forum topic:
http://www.ogre3d.org/addonforums/viewtopic.php?f=8&t=29272

If there are problems, tell it. 
If everything is well, tell it too.

Note: To report problems, consider to use the "Snipping Tool" of Windows 7 for easy screenshot creation. 
      Just type "snipp" into the search field of the start menu. 



--- Notes for .NET 4.0 ---

Currently MogreBuilder only builds Mogre against .NET 4.0. 

This means: 
* You need Visual Studio 2010 or newer (Visual Studio 2008 isn't usable).
* Change your project settings to .NET 4.0.
* All of you dll files has to be replaced by .NET 4.0 builds, too.
* If not possible, you can try a mixture usage of .NET versions. 
  For this select a ".NET Client mode" in the project settings. 
  (Although this isn't recommended, because it can cause trouble.)

