const path = require('path')
require('dotenv').config({
  path: path.join(__dirname, '../environment/tracker.env')
})
var os = require('os')
const fs = require('fs')
const Sentiment = require('sentiment')
const swedish = { labels: JSON.parse(fs.readFileSync('swedish_adjusted.json')) }
const sentiment = new Sentiment()
sentiment.registerLanguage('sv', swedish)

console.log('******************************************')
console.log(`using AFINN-based sentiment analysis
along with a word lexicon from KAGGLE`)
console.log('******************************************')
const articles = JSON.parse(
  fs.readFileSync('../data/articles.json', 'utf8').trim()
)

// consulting Kaggle word lists as well
let positiveWords = fs
  .readFileSync('./positive_words_sv.txt', 'utf8')
  .toString()
  .trim()
  .split(os.EOL)

positiveWords = positiveWords.map(w => w.trim())

let negativeWords = fs
  .readFileSync('./negative_words_sv.txt', 'utf8')
  .toString()
  .trim()
  .split(os.EOL)

negativeWords = negativeWords.map(w => w.trim())

const results = []
const fileWriter = fs.createWriteStream(`../data/articles.tsv`, { flags: 'a' })
if (!fs.existsSync('../data/articles.tsv')) {
  // write headers first time around
  const tsvHeader = `label\tscore\ttext\n`
  fileWriter.write(tsvHeader)
}
let rowCounter = 0
articles.forEach(article => {
  article.Text.split('.').forEach(sentence => {
    if (!invalidSentence(sentence)) {
      replaceTrackingKeywords(sentence)
      const result = sentiment.analyze(sentence, { language: 'sv' })
      if (result.score >= 7 || result.score <= -7) { // only very positive and very negative
        const numberOfPositiveWordsInSentence = numberOfWordsInList(
          positiveWords,
          sentence
        )
        const numberOfNegativeWordsInSentence = numberOfWordsInList(
          negativeWords,
          sentence
        )
        let tsvRow = ''
        if (
          // AFINN and KAGGLE agree on positive
          result.score > 0 &&
          numberOfPositiveWordsInSentence >= numberOfNegativeWordsInSentence
        ) {
          tsvRow = `0\t${result.score}\t${sentence}\n`
          results.push(result)
          rowCounter++
          fileWriter.write(tsvRow)
        } else if (
          // AFINN and KAGGLE agree on negative
          result.score < 0 &&
          numberOfNegativeWordsInSentence >= numberOfPositiveWordsInSentence
        ) {
          tsvRow = `1\t${result.score}\t${sentence}\n`
          results.push(result)
          rowCounter++
          fileWriter.write(tsvRow)
        }
      }
    }
  })
})
console.log('number of rows', rowCounter)
console.log('negativa meningar', results.filter(r => r.score < -2).length)
console.log('positiva meningar', results.filter(r => r.score > 2).length)

function replaceTrackingKeywords (sentence) {
  process.env.KEYWORDS.split(';').forEach(word => {
    var re = new RegExp(word, 'g')
    sentence.replace(re, 'neutral-place-holder-for-keyword')
  })
}
function invalidSentence (sentence) {
  return sentence.includes('<') || sentence.includes('>')
}
function numberOfWordsInList (listOfWords, sentence) {
  let count = 0
  sentence.split(' ').forEach(word => {
    if (listOfWords.includes(word)) {
      count++
    }
  })
  return count
}
