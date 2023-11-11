## How to use?

Fetch up-to-date database:
```js
let db = await fetch(`https://raw.github.com/qt-kaneko/MALibria/db/mapped.json`);
```

To get My Anime List/Shikimori ID from Anilibria ID:
```js
let id = db.find(title => title[`anilibria_id`] === YOUR_ID)?.[`myanimelist_id`];
```

To get Anilibria ID from My Anime List/Shikimori ID:
```js
let id = db.find(title => title[`myanimelist_id`] === YOUR_ID)?.[`anilibria_id`];
```