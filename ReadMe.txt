Install Microsoft Visual C# 2010 Express. 
http://www.microsoft.com/visualstudio/en-us/products/2010-editions/visual-csharp-express

Install TortoiseHg. 
http://tortoisehg.bitbucket.org/download/index.html

Install Cmake (add to system path, reboot). 
http://www.cmake.org/cmake/resources/software.html   


1. Download MogreBuilder source and compile.
https://bitbucket.org/mogre/mogrebuilder

2. Create an empty working directory and set the command line parameter in project properties. e.g. 
C:\Mogre

3. Setup the config file if required. There is a Default.cfg as an example. 
You can specify a different one using the command line options. e.g.
MogreBuilder C:\Mogre -config Default.cfg

The auto-builder should now ran successfully.
(please be patient during the repository cloning tasks)
