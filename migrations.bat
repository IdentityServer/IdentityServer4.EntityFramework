cd src\Host
rmdir /S /Q Migrations
dotnet ef migrations add Grants -c PersistedGrantDbContext -o Migrations/IdentityServer/PersistedGrantDb
dotnet ef migrations add Config -c ConfigurationDbContext -o Migrations/IdentityServer/ConfigurationDb
cd ..\..
