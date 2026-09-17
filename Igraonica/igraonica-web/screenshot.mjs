import { chromium } from 'playwright';
import path from 'node:path';

const OUT_DIR = 'C:\\Users\\luka.stankovic\\Desktop\\Igraonica diplomski\\Igraonica\\screenshots';
const BASE = 'http://localhost:5173';

async function login(page, email, password) {
  await page.goto(`${BASE}/prijava`);
  await page.getByLabel('Adresa elektronske poste').fill(email);
  await page.getByLabel('Lozinka').fill(password);
  await page.getByRole('button', { name: 'Prijavi se' }).click();
  await page.waitForURL(BASE + '/');
}

async function main() {
  const browser = await chromium.launch();
  const context = await browser.newContext({ viewport: { width: 1360, height: 766 } });
  const page = await context.newPage();

  // p01: kalendar dostupnosti (pre prijave, javno dostupan pregled)
  await page.goto(`${BASE}/zakazivanje`);
  await page.waitForSelector('text=Zakazivanje proslave');
  await page.waitForTimeout(800);
  await page.screenshot({ path: path.join(OUT_DIR, 'p01_kalendar.png') });

  // Prijava kao klijent
  await login(page, 'roditelj@example.com', 'Roditelj123!');

  // p02: obrazac za kreiranje rezervacije, popunjen
  await page.goto(`${BASE}/zakazivanje`);
  await page.waitForTimeout(600);
  // Idi na sledeci mesec dok se ne pojavi slobodan datum (oktobar 2026)
  await page.getByLabel('Sledeci mesec').click();
  await page.waitForTimeout(400);
  const freeDay = page.locator('button:not([disabled])', { hasText: '15' }).first();
  await freeDay.click();
  await page.waitForTimeout(400);
  const slotButton = page.locator('button', { hasText: 'termin' }).first();
  await slotButton.click();
  await page.getByLabel('Ime deteta').fill('Nikola');
  await page.getByLabel('Uzrast deteta').fill('8');
  await page.getByLabel('Ocekivan broj dece').fill('12');
  await page.locator('button', { hasText: 'Premium' }).click();
  await page.waitForTimeout(400);
  await page.screenshot({ path: path.join(OUT_DIR, 'p02_obrazac.png') });

  // p03: pregled sopstvenih rezervacija
  await page.goto(`${BASE}/moje-rezervacije`);
  await page.waitForTimeout(800);
  await page.screenshot({ path: path.join(OUT_DIR, 'p03_moje.png') });

  // Odjava i prijava kao administrator
  await page.goto(`${BASE}/`);
  await page.getByRole('button', { name: 'Odjava' }).click();
  await page.waitForTimeout(300);

  await login(page, 'admin@joyland.local', 'Administrator123!');

  // p04: administratorski pregled rezervacija
  await page.goto(`${BASE}/admin`);
  await page.waitForTimeout(1000);
  await page.screenshot({ path: path.join(OUT_DIR, 'p04_admin.png') });

  await browser.close();
  console.log('Frontend screenshots done.');
}

main().catch((err) => {
  console.error(err);
  process.exit(1);
});
