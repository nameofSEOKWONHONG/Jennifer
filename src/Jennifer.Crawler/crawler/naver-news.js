const { chromium } = require('playwright');

async function runCrawler(keyword = 'GPT') {
    const browser = await chromium.launch();
    const page = await browser.newPage();
    const url = `https://search.naver.com/search.naver?where=news&query=${encodeURIComponent(keyword)}`;

    await page.goto(url);
    await autoScroll(page, 5);

    const articles = await page.$$eval(
        'a:has(span.sds-comps-text.sds-comps-text-ellipsis-1)',
        anchors => anchors.map(a => ({
            title: a.innerText.trim(),
            href: a.href
        }))
    );

    await browser.close();
    return articles;
}

async function autoScroll(page, maxScrollCount = 5) {
    await page.evaluate(async (maxCount) => {
        await new Promise(resolve => {
            let count = 0;
            const interval = setInterval(() => {
                window.scrollBy(0, 300);
                count++;
                if (count >= maxCount) {
                    clearInterval(interval);
                    resolve();
                }
            }, 500);
        });
    }, maxScrollCount);
}

module.exports = { runCrawler };