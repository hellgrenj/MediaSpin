const fs = require('fs')
const path = require('path')
const Sentiment = require('sentiment')
const swedish = { labels: JSON.parse(fs.readFileSync(path.join(__dirname, 'swedish_adjusted.json'))) }
const sentiment = new Sentiment()
sentiment.registerLanguage('sv', swedish)
console.log('\n\n node.js AFINN server started \n\n')
module.exports = function (callback, sentence) {
  const result = sentiment.analyze(sentence, { language: 'sv' })
  callback(null, result)
}
