
--- Preparation ---

Install Microsoft Visual C# 2010 Express and Visual C++ Express. 
http://www.microsoft.com/visualstudio/en-us/products/2010-editions/visual-csharp-express
http://www.microsoft.com/visualstudio/en-us/products/2010-editions/visual-cpp-express
..... Or alternatively install Visual Stutio. 

Install TortoiseHg. 
http://tortoisehg.bitbucket.org/download/index.html

Install Cmake (add to system path, reboot). 
http://www.cmake.org/cmake/resources/software.html   

Install the DirectX SDK (June 2010)
http://www.microsoft.com/download/en/details.aspx?id=6812

Note: Some Ogre depencies will be downloaded automatically. 



--- Step by step ---

1. Download MogreBuilder source and compile.
   https://bitbucket.org/mogre/mogrebuilder

2. Enter directory "bin_Debug", open file "GO_example.bat",
   and type in your wanted target directory.
 
3. Setup the config file if required. There is a Default.cfg as an example. 
   You can specify a different one using the command line options. e.g.
   MogreBuilder C:\Mogre -config Default.cfg

4. Call "GO_example.bat".
   Alternatively you can call the MogreBuilder from command line.

The auto-builder should now ran successfully.
(Be patient during the repository cloning tasks at the first usage.)
After the downloads are finished, it needs about 1 hour for building Mogre.



--- Feedback ---

Please give feedback in the MogreBuilder forum topic:
http://www.ogre3d.org/addonforums/viewtopic.php?f=8&t=29272

If there are problems, tell it. 
If everything is well, tell it too.

Note: To report problems, consider to use the "Snipping Tool" of Windows 7 for easy screenshot creation. 
      Just type "snipp" into the search field of the start menu. 


