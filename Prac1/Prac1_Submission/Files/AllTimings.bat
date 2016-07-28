fpc SievePas.pas
echo FPC
Timer.bat SievePas.exe 

csharp SieveCS.cs
echo CS 
Timer.bat SieveCS.exe 

parva2tocsharp SievePav.pav
csharp SievePavp2c.cs
echo Convert 
Timer.bat SievePavp2c.exe 

BCC SieveC.c 
echo BCC C 
Timer.bat SieveC.exe 

BCC SieveCpp.cpp 
echo BCC CPP 
Timer.bat Sievecpp.exe 

CL SieveC.c
echo CL C  
Timer.bat SieveC.exe 

CL SieveCpp.cpp 
echo CL CPP 
Timer.bat SieveCpp.exe 

fpc fiboPas.pas
echo FPC 
Timer.bat fiboPas.exe 
csharp fiboCS.cs
echo CS 
Timer.bat fiboCS.exe 
parva2tocsharp fiboPav.pav
csharp fiboPavp2c.cs
echo Convert 
Timer.bat fiboPavp2c.exe 
BCC fiboC.c 
echo BCC C 
Timer.bat fiboC.exe 
BCC fiboCpp.cpp 
echo BCC CPP 
Timer.bat fibocpp.exe 
CL fiboC.c
echo CL C  
Timer.bat fiboC.exe 
CL fiboCpp.cpp 
echo CL CPP 
Timer.bat fiboCpp.exe 