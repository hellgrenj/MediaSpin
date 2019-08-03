const htmlToText = require('html-to-text')

module.exports = function (callback, html) {
  const text = htmlToText.fromString(html, {
    wordwrap: 130,
    ignoreHref: true,
    ignoreImage: true
  })
  callback(null, text)
}
