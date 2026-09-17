import { chromium } from 'playwright';
import path from 'node:path';

const OUT_DIR = 'C:\\Users\\luka.stankovic\\Desktop\\Igraonica diplomski\\Igraonica\\screenshots';

async function main() {
  const browser = await chromium.launch();
  const page = await browser.newPage({ viewport: { width: 1280, height: 652 } });
  await page.goto('http://localhost:5197/scalar/v1');
  await page.waitForTimeout(1500);

  // Prosiri grupu "Reservations" i prikazi primer zahteva za kreiranje rezervacije.
  const reservationsGroup = page.getByText('Reservations', { exact: true }).first();
  if (await reservationsGroup.isVisible().catch(() => false)) {
    await reservationsGroup.click().catch(() => {});
    await page.waitForTimeout(600);
  }

  await page.screenshot({ path: path.join(OUT_DIR, 'p05_swagger.png') });
  await browser.close();
  console.log('API docs screenshot done.');
}

main().catch((err) => {
  console.error(err);
  process.exit(1);
});
