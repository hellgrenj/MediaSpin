const path = require('path')
const rabbit = require('./rabbit')
const logger = require('./logfactory').getLogger(path.basename(__filename))
const Twit = require('twit')

var T = new Twit({
  consumer_key: process.env.TWITTER_CONSUMER_KEY,
  consumer_secret: process.env.TWITTER_CONSUMER_SECRET,
  access_token: process.env.TWITTER_ACCESS_TOKEN,
  access_token_secret: process.env.TWITTER_ACCESS_TOKEN_SECRET,
  timeout_ms: 60 * 1000, // optional HTTP request timeout to apply to all requests.
  strictSSL: true // optional - requires SSL certificates to be valid.
})

async function start () {
  await rabbit.init()
  rabbit.listen(handle)
  logger.info('bot up and running')
}
start()

function handle (msg, ack) {
  const tweet = buildTweet(msg)
  if (process.env.TWITTER_BOT_ENABLED === 'true') {
    T.post('statuses/update', { status: tweet }, function (
      err,
      data,
      response
    ) {
      console.log(data)
    })
  } else {
    logger.info(
      `bot disabled, this is the tweet that would have been published ${tweet}`
    )
  }
  ack()
}
function buildTweet (msg) {
  const charLimit = 280
  const firstOption = `Jag hittade en ${
    msg.Positive ? 'positiv' : 'negativ'
  } mening innehållande nyckelordet #${msg.Keyword.trim()} i artikeln ${
    msg.ArticleUrl
  } 
  "${msg.Sentence.trim()}"`
  const secondOption = `${
    msg.Positive ? 'Positiv' : 'Negativ'
  } mening (nyckelord: #${msg.Keyword.trim()}, källa: ${msg.ArticleUrl}) 
  "${msg.Sentence.trim()}"`

  const thirdOption = `Jag hittade en ${
    msg.Positive ? 'positiv' : 'negativ'
  } mening innehållande nyckelordet #${msg.Keyword.trim()} i artikeln ${
    msg.ArticleUrl
  }. Mer info på https://mediaspin.johanhellgren.se.`

  if (firstOption.length <= charLimit) {
    return firstOption
  } else if (secondOption.length <= charLimit) {
    return secondOption
  } else {
    return thirdOption
  }
}
