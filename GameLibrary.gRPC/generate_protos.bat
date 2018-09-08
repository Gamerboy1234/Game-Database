
@rem Generate the C# code for .proto files

setlocal

@rem enter this directory
cd /d %~dp0

set TOOLS_PATH=C:\Users\rajjj\.nuget\packages\grpc.tools\1.14.2\tools\windows_x64

%TOOLS_PATH%\protoc.exe -I"../GameLibrary.gRPC/protos" --csharp_out "../GameLibrary.gRPC/generated"  "../GameLibrary.gRPC/protos/gamelibrary.proto" --grpc_out "../GameLibrary.gRPC/generated" --plugin=protoc-gen-grpc=%TOOLS_PATH%\grpc_csharp_plugin.exe

endlocal
