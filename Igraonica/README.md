# Igraonica &mdash; Sistem za rezervaciju termina proslava (JoyLand)

Realizacija diplomskog rada "Sistem za rezervaciju zakupa igraonice za malu decu i
njihove roditelje". Sistem pokriva ceo zivotni ciklus rezervacije rodjendanske
proslave u igraonici **JoyLand** (Karadjordjeva 18, Smederevo): registraciju i
potvrdu naloga, pregled dostupnosti, kreiranje i otkazivanje rezervacije,
administratorsku potvrdu dolaska i automatsko oslobadjanje isteklih termina.

## Arhitektura

- **src/Igraonica.Domain** &mdash; entiteti, poslovna pravila, domenski izuzeci. Bez ijedne spoljne zavisnosti.
- **src/Igraonica.Application** &mdash; servisi, DTO, validatori, apstrakcije tehnickih servisa.
- **src/Igraonica.Infrastructure** &mdash; EF Core (PostgreSQL), repozitorijumi, JWT/BCrypt, SMTP, pozadinski poslovi.
- **src/Igraonica.WebAPI** &mdash; ASP.NET Core 10 kontroleri, middleware, DI, ulazna tacka.
- **igraonica-web** &mdash; React 19 + TypeScript + Vite + Tailwind CSS 4 + TanStack Query klijent, sa tipovima generisanim iz OpenAPI dokumenta.
- **tests/Igraonica.UnitTests** &mdash; xUnit + FluentAssertions, poslovna pravila bez baze podataka.
- **tests/Igraonica.IntegrationTests** &mdash; WebApplicationFactory nad stvarnom PostgreSQL bazom, ukljucujuci test istovremenog pristupa (T-18).

## Pokretanje u razvoju (bez Dockera)

Ova instanca je izgradjena i testirana u okruzenju bez Docker Desktop-a, pa je
lokalni razvojni tok podesen da radi direktno sa .NET SDK-om i lokalno
instaliranim PostgreSQL-om:

1. **PostgreSQL 17** &mdash; baza `igraonica`, korisnik `igraonica_user` /
   `igraonica_dev_2026` (podesi po zelji u `appsettings.Development.json`).
2. Iz korena solucije:
   ```bash
   dotnet ef database update --project src/Igraonica.Infrastructure --startup-project src/Igraonica.WebAPI
   dotnet run --project src/Igraonica.WebAPI
   ```
   Backend se pokrece na `http://localhost:5197`, migracije i pocetni podaci
   (paketi, termini, admin nalog) primenjuju se automatski u razvojnom okruzenju.
3. Frontend:
   ```bash
   cd igraonica-web
   npm install
   npm run dev
   ```
   Dostupan na `http://localhost:5173`, `/api` pozivi se prosledjuju ka backend-u (vidi `vite.config.ts`).

### Nalozi za probu

| Uloga         | Adresa                    | Lozinka             |
|---------------|----------------------------|----------------------|
| Administrator | admin@joyland.local         | Administrator123!    |
| Klijent       | roditelj@example.com        | Roditelj123!         |

### Poruke elektronske poste u razvoju

`appsettings.Development.json` pokazuje na `localhost:1025` (Mailpit) bez
autentikacije. Ako Mailpit nije pokrenut, poruke ostaju u tabeli
`OutboxMessages` neposlate (RetryCount raste), sto ne utice na vec sacuvanu
rezervaciju (NFZ-07) &mdash; mozete ih pregledati direktno u bazi.

## Pokretanje u kontejnerima

```bash
DB_PASSWORD=... JWT_KEY=... docker compose up --build
```

`docker-compose.yml` sadrzi i opcioni servis `mail` (profil `dev`, alat
Mailpit) za presretanje poruka u razvoju:
`docker compose --profile dev up --build`.

## Slanje poruka u produkciji

Podrazumevana adresa posiljaoca je **igraonicajoyland@gmail.com**. Gmail
zahteva **App Password** (Nalog &rarr; Bezbednost &rarr; Dvofaktorska
autentikacija &rarr; Lozinke za aplikacije) jer obicna lozinka naloga ne radi
za SMTP AUTH. Vrednosti `Email__Username` i `Email__Password` se ne nalaze u
izvornom kodu (NFZ-04) &mdash; prosledjuju se kao promenljive okruzenja pri
pokretanju kontejnera ili kroz `dotnet user-secrets` lokalno.

## Testovi

```bash
dotnet test tests/Igraonica.UnitTests        # 42 testa, bez baze podataka
dotnet test tests/Igraonica.IntegrationTests # 2 testa, zahteva lokalnu bazu "igraonica_test"
```

Integracioni testovi (`tests/Igraonica.IntegrationTests/Infrastructure/CustomWebApplicationFactory.cs`)
koriste zasebnu lokalnu bazu `igraonica_test` umesto biblioteke Testcontainers,
jer Docker nije bio dostupan u razvojnom okruzenju u kome je projekat izradjen.
U okruzenju sa dostupnim Docker-om, isti testovi bi se lako preusmerili na
efemernu instancu podignutu bibliotekom `Testcontainers.PostgreSql`.

## Napomena o slikama u diplomskom radu

Slike p01&ndash;p05 (snimci ekrana korisnickog interfejsa i interaktivne
dokumentacije) u datoteci `Diplomski_rad_Luka_Stankovic_IT41_2022_sa_snimcima.docx`
napravljene su iz ove, stvarno pokrenute aplikacije (skripte
`igraonica-web/screenshot.mjs` i `screenshot-api.mjs`, alat Playwright).
