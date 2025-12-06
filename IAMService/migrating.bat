@echo
dotnet ef migrations add Init --project Infrastructure --startup-project API

dotnet ef database update --project Infrastructure --startup-project API

dotnet ef database drop --project Infrastructure --startup-project API
