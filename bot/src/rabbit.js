const path = require('path')
const amqp = require('amqplib/callback_api')
const logger = require('./logfactory').getLogger(path.basename(__filename))
const rabbit = {}

rabbit.init = function () {
  return new Promise((resolve, reject) => {
    tryConnect(resolve)
  })
}

const username = process.env.RABBITMQ_DEFAULT_USER
const password = process.env.RABBITMQ_DEFAULT_PASS
const connectionString = `amqp://${username}:${password}@rabbit`
function tryConnect (resolve, attempt = 0) {
  amqp.connect(connectionString, (err, conn) => {
    if (err) {
      if (attempt < 10) {
        return setTimeout(() => {
          const nextAttempt = attempt + 1
          logger.debug(
            `connection failed, re-trying in 3 seconds attempt ${nextAttempt}/10`
          )
          tryConnect(resolve, nextAttempt)
        }, 3000)
      } else {
        throw err
      }
    }
    rabbit.conn = conn
    logger.info(`connected to rabbit attempt ${attempt}/10`)
    resolve()
  })
}
rabbit.listen = function (handle) {
  rabbit.conn.createChannel((err, ch) => {
    if (err) throw err

    ch.assertQueue(
      `bot_queue`,
      {
        durable: true
      },
      function (err, q) {
        if (err) throw err
        logger.info(`Waiting for messages in ${q.queue}`)
        ch.consume(
          q.queue,
          function (msg) {
            logger.info(
              `received msg ${JSON.stringify(msg.content.toString())}`
            )
            handle(JSON.parse(msg.content.toString()), () => {
              ch.ack(msg)
            })
          },
          {
            noAck: false
          }
        )
      }
    )
  })
}
module.exports = rabbit
