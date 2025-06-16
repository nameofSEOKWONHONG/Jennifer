const { chromium } = require('playwright');

async function runCrawler(keyword = 'GPT') {
    const browser = await chromium.launch();
    const page = await browser.newPage();
    const url = `https://search.naver.com/search.naver?ssc=tab.news.all&query=${encodeURIComponent(keyword)}&sm=tab_opt&sort=1&photo=0&field=0&pd=-1&ds=2025.06.16&de=2025.06.16&docid=&related=0&mynews=1&office_type=0&office_section_code=0&news_office_checked=&nso=so%3Add%2Cp%3Aall&is_sug_officeid=0&office_category=&service_area=`;

    await page.goto(url);
    await autoScroll(page, 5);
    const visitedSet = new Set();

    const rawResults  = await page.$$eval(
        'a:has(span.sds-comps-text.sds-comps-text-ellipsis-1)',
        anchors => anchors.map(a => ({
            title: a.innerText.trim(),
            href: a.href,
        }))
    );

    const domainOnlyRegex = /^https?:\/\/(?:[A-Za-z0-9-]+\.)+[A-Za-z]{2,}(?::\d+)?\/?$/;
    const numericPressPattern = /^https?:\/\/media\.naver\.com\/press\/(?:00[1-9]|0[1-9]\d|[1-9]\d{2})\/?$/;

    const newItems = rawResults.filter(({ href }) => {
        if (domainOnlyRegex.test(href)) return false;         // 도메인만 URL 제외
        if (numericPressPattern.test(href)) return false;    // 001~999 언론사 링크 제외
        if (visitedSet.has(href)) {
            return false;              // 이미 방문한 URL 제외
        }
        else {
            visitedSet.add(href);      // 새 URL은 방문 기록에 추가
            return true;               // 유효한 URL만 반환
        }
        return true;
    });    
    await browser.close();
    return newItems;
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