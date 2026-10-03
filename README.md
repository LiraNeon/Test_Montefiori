# ExcelApp

Aplicación web para cargar archivos Excel y gestionar sus registros (crear, listar, editar y eliminar).

- **Backend:** ASP.NET Core 8 Web API + EF Core + SQLite + ClosedXML
- **Frontend:** Angular 19
- **Pruebas:** xUnit

## Requisitos

- .NET 8 SDK
- Node.js 18+ y npm

## Cómo ejecutar

**Backend** (http://localhost:5005, Swagger en `/swagger`):

```bash
dotnet run --project backend/ExcelApp.Api
```

La base de datos `excelapp.db` se crea automáticamente al iniciar.

**Frontend** (http://localhost:4200):

```bash
cd frontend
npm install
npm start
```

## Pruebas

```bash
dotnet test backend/ExcelApp.Tests
```

## Formato del Excel

La primera fila debe tener los encabezados. `Nombre` y `Email` son obligatorios; el resto es opcional.

| Nombre | Email | Telefono | Distrito | Sexo |
|---|---|---|---|---|
| Pepe | pepe@gmail.com | 999878797 | Ate | H |

Hay un archivo de ejemplo en [`samples/datos.xlsx`](samples/datos.xlsx).

## Endpoints

| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/registros` | Lista todos los registros |
| POST | `/api/registros/upload` | Carga un archivo `.xlsx` (campo `archivo`) |
| PUT | `/api/registros/{id}` | Actualiza un registro |
| DELETE | `/api/registros/{id}` | Elimina un registro |
