# TooliRent API

TooliRent är ett RESTful API för att hantera uthyrning av verktyg. Systemet stödjer användarregistrering och inloggning, bokningar, kategorisering av verktyg samt hantering av tillgänglighet.

---

## Projektbeskrivning och Arkitektur

TooliRent är ett REST-API byggt med **ASP.NET Core Web API** för att hantera uthyrning av verktyg i ett makerspace. API:et följer en **N-Tier-arkitektur** med tydlig separation mellan lager:

- **Presentation Layer (Controllers):**  
  Hanterar inkommande HTTP-förfrågningar och skickar svar tillbaka till klienten. Exempel: `AuthController`, `BookingsController`, `CategoriesController`, `ToolsController`.

- **Application Layer (Services):**  
  Innehåller affärslogik och orchestrerar operationer mellan Domain och Infrastructure.  
  Här används **Service Pattern** för att kapsla logik och säkerställa återanvändbarhet.

- **Domain Layer (Entities & Interfaces):**  
  Representerar kärndomänen av applikationen med modeller som `Booking`, `Tool`, `Category` och `ApplicationUser`.  
  Definierar även kontrakt för repository-interface.

- **Infrastructure Layer (Repositories & DbContext):**  
  Ansvarar för datalagring och access via **Repository Pattern**. Här ligger `TooliRentDbContext`, repository-implementationer och migrationer.

### Repository & Service Pattern
Projektet använder **Repository Pattern** för att abstrakta databasanrop och **Service Layer** för att hantera affärslogik. Detta gör koden modulär, testbar och lätt att underhålla.

### Autentisering & Auktorisering
- JWT-baserad autentisering.  
- Rollbaserad auktorisering med rollerna **Member** och **Admin**.

### Databas
- **SQL Server** används som relationsdatabas.  
- Code-First approach med migrations.  
- Seed data finns för utveckling och testning.

### DTOs & Standardiserade Svar
Alla endpoints returnerar standardiserade svar enligt `ApiResponse`-modellen som föjler RFC 7807, vilket inkluderar:
- `isError`: Boolean som anger om anropet misslyckades.  
- `message`: Eventuellt meddelande till klienten.  
- `data`: Det faktiska svaret (ex. `BookingDto`, `ToolDto`).  
- `error`: Detaljer om eventuella fel.


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


### BookingsController

#### GET /api/Bookings/all
- Hämtar alla bokningar.
- Response: Lista av bokningar (`BookingDtoIEnumerableApiResponse`).

#### GET /api/Bookings/{id}
- Hämtar en specifik bokning.
- URL Parameter:
  ```bash
  id (string, guid): ID för bokningen
Response: Bokningsdetaljer (BookingDtoApiResponse).


#### PUT /api/Bookings/{id}
- Uppdaterar en bokning.
- URL Parameter:
id (string, guid): ID för bokningen
Request Body:
  ```json
  {
  "startAt": "2025-09-27T10:00:00Z",
  "endAt": "2025-09-27T12:00:00Z",
  "status": 1
  }
Response: Bekräftelse (BookingDtoApiResponse).

#### DELETE /api/Bookings/{id}
- Tar bort en bokning.
- URL Parameter:
id (string, guid): ID för bokningen
Response: No Content (204).


#### GET /api/Bookings/{id}/items
- Hämtar alla verktyg för en bokning.
- URL Parameter:
id (string, guid): ID för bokningen
Response: Lista av verktyg (BookingWithItemsDtoApiResponse).


#### GET /api/Bookings/user/{userId}
- Hämtar alla bokningar för en användare.
- URL Parameter:
userId (string): ID för användaren
Response: Lista av bokningar (BookingDtoIEnumerableApiResponse).


#### GET /api/Bookings/status/{status}
- Hämtar alla bokningar med specifik status.
- URL Parameter:
status (int): Bokningsstatus (0 = Reserved, 1 = PickedUp, 2 = Returned, etc.)
Response: Lista av bokningar (BookingDtoIEnumerableApiResponse).

#### GET /api/Bookings/active
- Hämtar alla aktiva bokningar.
Response: Lista av bokningar (BookingDtoIEnumerableApiResponse).

#### POST /api/Bookings/{id}/return
- Markerar bokningen som återlämnad.
- URL Parameter:
id (string, guid): ID för bokningen
Response: Bekräftelse (ApiResponse).

#### POST /api/Bookings/create

Skapar en ny bokning.

Request Body:
    {
  "userId": "string",
  "startAt": "2025-09-27T10:00:00Z",
  "endAt": "2025-09-27T12:00:00Z",
  "toolIds": ["guid1", "guid2"]
  }
Response: ID för skapad bokning (GuidNullableApiResponse).

#### POST /api/Bookings/{id}/pickup
- Markerar bokningen som uthämtad.
- URL Parameter:
id (string, guid): ID för bokningen
Response: Bekräftelse (ApiResponse).

### CategoriesController

#### GET /api/Categories/all
- Hämtar alla kategorier.

#### GET /api/Categories/{id}
- Hämtar en specifik kategori.
- URL Parameter:
id (string, guid): ID för kategorin
Response: Kategoridetaljer (CategoryDtoApiResponse).


#### PUT /api/Categories/{id}
- Uppdaterar en kategori.
- URL Parameter:
id (string, guid): ID för kategorin
Response: Uppdaterad kategori (CategoryDtoApiResponse).

#### DELETE /api/Categories/{id}
- Tar bort en kategori.
- URL Parameter:
id (string, guid): ID för kategorin
Response: No Content (204).

#### POST /api/Categories/add
- Skapar en ny kategori.
- Request Body:
{
  "name": "string",
  "description": "string"
}
Response: Ny kategori (CategoryDtoApiResponse).

#### GET /api/Categories/search
- Söker kategorier baserat på namn.
- Query Parameter:
- query (string): Söksträng
Response: Lista av kategorier (CategoryDtoIEnumerableApiResponse).


#### GET /api/Categories/{id}/tools
- Hämtar alla verktyg i en specifik kategori.
- URL Parameter:
id (string, guid): ID för kategorin
Response: Lista av verktyg (ToolDtoIEnumerableApiResponse).

### Tools Controller

#### GET /api/Tools/all
- Hämtar alla verktyg.
Response: Lista av verktyg (ToolDtoIEnumerableApiResponse).

#### GET /api/Tools/{id}
- Hämtar ett specifikt verktyg.
- URL Parameter:
id (string, guid): ID för verktyget
Response: Verktygsdetaljer (ToolDtoApiResponse).

#### PUT /api/Tools/{id}
- Uppdaterar ett verktyg.
- URL Parameter:
id (string, guid): ID för verktyget
- Request Body:
{
  "name": "string",
  "description": "string",
  "categoryId": "guid",
  "status": 0
}
Response: Uppdaterat verktyg (ToolDtoApiResponse).

#### DELETE /api/Tools/{id}
- Tar bort ett verktyg.
- URL Parameter:
id (string, guid): ID för verktyget
Response: No Content (204).

#### POST /api/Tools
- Skapar ett nytt verktyg.
Request Body:
{
  "name": "string",
  "description": "string",
  "categoryId": "guid",
  "status": 0
}
Response: Nytt verktyg (ToolDtoApiResponse).


#### GET /api/Tools/available
- Hämtar alla verktyg med status Available.
Response: Lista av verktyg (ToolDtoIEnumerableApiResponse).

#### GET /api/Tools/search
- Söker verktyg baserat på namn.
- Query Parameter:
 - query (string): Söksträng
Response: Lista av verktyg (ToolDtoIEnumerableApiResponse).

#### GET /api/Tools/category/{categoryId}
- Hämtar alla verktyg i en specifik kategori.
- URL Parameter:
- categoryId (string, guid): ID för kategorin
Response: Lista av verktyg (ToolDtoIEnumerableApiResponse).



### Dtos
CategoryDto
{
  "id": "guid",
  "name": "string",
  "description": "string"
}

ToolDto
{
  "id": "guid",
  "name": "string",
  "description": "string",
  "categoryId": "guid",
  "status": 0
}

BookingDto
{
  "id": "guid",
  "userId": "string",
  "startAt": "datetime",
  "endAt": "datetime",
  "status": 0,
  "bookingItems": [
    {
      "id": "guid",
      "toolId": "guid"
    }
  ]
}

TokenDto
{
  "accessToken": "string",
  "refreshToken": "string",
  "expiresIn": "datetime"
}

ApiResponse
{
  "isError": false,
  "message": "string",
  "data": {},
  "error": null
}
