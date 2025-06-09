const express = require('express');
const cors = require('cors');
const { runCrawler } = require('./crawler/naver-news');

const app = express();
const PORT = 3000;

app.use(cors());

app.get('/crawl', async (req, res) => {
    const keyword = req.query.q ?? 'GPT';

    try {
        const results = await runCrawler(keyword);
        res.json({ keyword, count: results.length, results });
    } catch (err) {
        console.error(err);
        res.status(500).json({ error: '크롤링 실패', details: err.message });
    }
});

app.listen(PORT, () => {
    console.log(`🚀 크롤러 API 서버 실행: http://localhost:${PORT}/crawl?q=GPT`);
});