cd src\Host
dotnet ef database update -c PersistedGrantDbContext
dotnet ef database update -c ConfigurationDbContext
cd ..\..
