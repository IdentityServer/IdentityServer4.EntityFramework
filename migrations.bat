cd src\Host
rmdir /S /Q Migrations
dotnet ef migrations add Grants -c PersistedGrantDbContext
dotnet ef migrations add Config -c ConfigurationDbContext
cd ..\..
