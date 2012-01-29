Assumptions:
Install Microsoft Visual C# 2010 Express. http://www.microsoft.com/visualstudio/en-us/products/2010-editions/visual-csharp-express
Install TortoiseHg. http://tortoisehg.bitbucket.org/download/index.html
Install Cmake (add to system path, reboot). http://www.cmake.org/cmake/resources/software.html   

1. Download MogreBuilder source and compile. 
https://bitbucket.org/mogre/mogrebuilder

2. Create working directory and set the command line parameter in project properties.
C:\Mogre

3. Downloaded Mogre source and extract into working directory.
https://bitbucket.org/mogre/mogre/overview

4. Get Ogre source using the command line: (run from the C:\Mogre\Main\OgreSrc directory)
hg clone http://bitbucket.org/sinbad/ogre/ -u v1-7   

5. If the patch fails for file OgreCodec.h you can I manually patched it using the reject file.

The auto-builder should ran successfully.
