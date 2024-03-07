@echo
cd "C:\Windows\Microsoft.NET\Framework\v4.0.30319"
RegAsm.exe C:\CompilationZone\MPago\InterfaceComNetCore.dll /codebase /tlb
Regsvr32.exe /s C:\CompilationZone\MPago\WrapperMercadoPagoAPI.comhost.dll 
pause

