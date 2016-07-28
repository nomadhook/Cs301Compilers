fpc SievePas.pas
echo FPC > results.txt
Timer.bat SievePas.exe >> results.txt
csharp SieveCS.cs
echo CS >> results.txt
Timer.bat SieveCS.exe >> results.txt
parva2tocsharp SievePav.pav
csharp SievePavp2c.cs
echo Convert >> results.txt
Timer.bat SievePavp2c.exe >> results.txt
BCC SieveC.c 
echo BCC C >> results.txt
Timer.bat SieveC.exe >> results.txt
BCC SieveCpp.cpp 
echo BCC CPP >> results.txt
Timer.bat Sievecpp.exe >> results.txt
CL SieveC.c
echo CL C >> results.txt 
Timer.bat SieveC.exe >> results.txt
CL SieveCpp.cpp 
echo CL CPP >> results.txt
Timer.bat SieveCpp.exe >> results.txt

fpc fiboPas.pas
echo FPC >> results.txt
Timer.bat fiboPas.exe >> results.txt
csharp fiboCS.cs
echo CS >> results.txt
Timer.bat fiboCS.exe >> results.txt
parva2tocsharp fiboPav.pav
csharp fiboPavp2c.cs
echo Convert >> results.txt
Timer.bat fiboPavp2c.exe >> results.txt
BCC fiboC.c 
echo BCC C >> results.txt
Timer.bat fiboC.exe >> results.txt
BCC fiboCpp.cpp 
echo BCC CPP >> results.txt
Timer.bat fibocpp.exe >> results.txt
CL fiboC.c
echo CL C >> results.txt 
Timer.bat fiboC.exe >> results.txt
CL fiboCpp.cpp 
echo CL CPP >> results.txt
Timer.bat fiboCpp.exe >> results.txt