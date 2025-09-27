# TooliRent API

TooliRent är ett RESTful API för att hantera uthyrning av verktyg. Systemet stödjer användarregistrering och inloggning, bokningar, kategorisering av verktyg samt hantering av tillgänglighet.

---

## Arkitektur

Projektet är byggt med **ASP.NET Core Web API** och följer en klassisk lagerindelning:

- **Auth** – hanterar autentisering och användarroller via JWT.
- **Bookings** – ansvarar för uthyrningar, återlämningar och bokningsstatusar.
- **Categories** – organiserar verktyg i kategorier.
- **Tools** – hanterar enskilda verktyg, deras status och tillgänglighet.
- **DTOs & Responses** – alla API-anrop returnerar standardiserade svar enligt `ApiResponse`-modellen.

Säkerhet implementeras med **JWT Bearer Authentication**.

---

## Kör lokalt

### Förutsättningar
- .NET 8
- SQL Server
  
### Steg
1. Klona repot och gå in i mappen:
   ```bash
   git clone https://github.com/HugoW5/TooliRent.git
   cd TooliRent
2. Hämta alla paket:
   ```bash
   dotnet restore
3. Kör migrations och skapa databasen:
   ```bash
   dotnet ef database update
4. Öppna Swagger i webbläsaren:
   ```bash
    https://localhost:7122/swagger



## Datamodeller

### ApplicationUser
- Ärver från `IdentityUser`.
- Egenskaper:
  - `IsActive` (bool): Anger om användaren är aktiv. Standardvärde: `true`.

### Booking
- Representerar en bokning av verktyg.
- Egenskaper:
  - `Id` (Guid): Unikt ID för bokningen.
  - `UserId` (string): FK till `ApplicationUser`.
  - `User` (ApplicationUser): Navigeringsegenskap till användaren.
  - `StartAt` (DateTime): Starttid för bokningen.
  - `EndAt` (DateTime): Sluttid för bokningen.
  - `Status` (BookingStatus): Status på bokningen. Standard: `Reserved`.
  - `BookingItems` (ICollection<BookingItem>): Lista med bokade verktyg.

### BookingItem
- Representerar ett enskilt verktyg i en bokning.
- Egenskaper:
  - `Id` (Guid): Unikt ID för bokningsposten.
  - `BookingId` (Guid): FK till `Booking`.
  - `Booking` (Booking): Navigeringsegenskap till bokningen.
  - `ToolId` (Guid): FK till `Tool`.
  - `Tool` (Tool): Navigeringsegenskap till verktyget.

### Category
- Representerar en kategori av verktyg.
- Egenskaper:
  - `Id` (Guid): Unikt ID för kategorin.
  - `Name` (string): Namn på kategorin.
  - `Description` (string, nullable): Beskrivning av kategorin.
  - `Tools` (ICollection<Tool>): Lista med verktyg i kategorin.

### RefreshToken
- Representerar en refresh-token för JWT-autentisering.
- Egenskaper:
  - `Id` (int, Key): Unikt ID för token.
  - `Token` (string): Själva tokensträngen.
  - `UserId` (string): ID på användaren token tillhör.
  - `ExpiryDate` (DateTime): När token går ut.
  - `IsUsed` (bool): Om token redan använts.
  - `IsRevoked` (bool): Om token har blivit återkallad.

### Tool
- Representerar ett verktyg som kan bokas.
- Egenskaper:
  - `Id` (Guid): Unikt ID för verktyget.
  - `Name` (string): Namn på verktyget.
  - `Description` (string, nullable): Beskrivning av verktyget.
  - `CategoryId` (Guid): FK till `Category`.
  - `Category` (Category): Navigeringsegenskap till kategorin.
  - `Status` (ToolStatus): Status på verktyget (ex: Available, Reserved, etc.).
  - `BookingItems` (ICollection<BookingItem>): Lista med bokningsposter där verktyget används.


## API Endpoints

### AuthController

#### POST /api/Auth/register
- Skapar en ny användare.
- Request Body:
  ```json
  {
    "email": "string",
    "userName": "string",
    "password": "string",
    "confirmPassword": "string"
  }
Response: JWT-token (TokenDtoApiResponse) vid lyckad registrering.

#### POST /api/Auth/login
- Loggar in en användare.
-Request Body:
  ```json
  {
  "email": "string",
  "password": "string"
  }
Response: JWT-token (TokenDtoApiResponse) vid lyckad inloggning.

#### POST /api/Auth/refresh
- Förnyar JWT-token med en giltig refresh-token.
- Request Body:
  ```json
  {
  "refreshToken": "string"
  }
Response: Ny JWT-token (TokenDtoApiResponse).

#### PUT /api/Auth/ToggleActivateAccount/{id}
- Aktiverar eller inaktiverar ett användarkonto.
- URL Parameter:
  ```bash
  id (string): ID för användaren som ska aktiveras/inaktiveras.
Response: Meddelande (StringApiResponse) som bekräftar ändringen.
