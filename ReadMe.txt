
--- Preparation ---

Install Microsoft Visual C# 2010 Express AND Visual C++ Express. 
http://www.microsoft.com/visualstudio/en-us/products/2010-editions/visual-csharp-express
http://www.microsoft.com/visualstudio/en-us/products/2010-editions/visual-cpp-express
..... OR alternatively install Visual Studio. 
..... Visual Studio 2012 can also be used with the -vs2012 command-line switch.

Install TortoiseHg. 
http://tortoisehg.bitbucket.org/download/index.html

Install Cmake (add to system path, reboot). 
http://www.cmake.org/cmake/resources/software.html   

Install the DirectX SDK (June 2010)
http://www.microsoft.com/download/en/details.aspx?id=6812



=========== IMPORTANT === IMPORTANT === IMPORTANT =============
--->  
--->  There are problems with the new Mercurial version 2.3.
--->  
--->  So do NOT use TortoiseHG 2.4.3 (contains Mercurial 2.3)
--->  Instead   USE TortoiseHG 2.4.2 (contains Mercurial 2.2)
--->  
--->  If somebody find the reason/solution, please tell us. 
--->  
===============================================================



--- Step by step ---

1. Download MogreBuilder source from:
   https://bitbucket.org/mogre/mogrebuilder

2. Compile MogreBuilder by use of file "MogreBuilder.sln".

3. Enter directory "bin_Debug", open file "RUN_example.bat",
   and type in your wanted target directory.
 
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
-priority <priority>        Use specified process priority. Priorities are 'Idle', 'BelowNormal', 'AboveNormal'
-noboost                    Build Ogre without boost (doesn't work yet)
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

