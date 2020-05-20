const path = require('path')
const rabbit = require('./rabbit')
const logger = require('./logfactory').getLogger(path.basename(__filename))

async function start () {
  await rabbit.init()
  logger.info('bot up and running')
}
start()
