const fs = require('fs')
var moment = require('moment')
moment().format('L')

const dir = '/logs'
if (!fs.existsSync(dir)) {
  fs.mkdirSync(dir)
}

const LOGLEVEL = process.env.LOG_LEVEL

var getLogLevelValue = function (logLevel) {
  var value = 4 // default to off
  if (logLevel === 'debug') {
    value = 1
  } else if (logLevel === 'info') {
    value = 2
  } else if (logLevel === 'error') {
    value = 3
  } else if (logLevel === 'off') {
    value = 4
  }
  return value
}

var logLevelActive = function (loglevel) {
  return getLogLevelValue(loglevel) >= getLogLevelValue(LOGLEVEL)
}

function log (loggedLevel, instance, msg) {
  if (logLevelActive(loggedLevel)) {
    const logMsg = `${moment().format('l, h:mm:ss A')} ${loggedLevel.toUpperCase()} > ${instance.caller}: ${msg}`
    console.log(logMsg)
    fs.createWriteStream(`${dir}/bot-${moment().format('YYYYMMDD')}.log`, { flags: 'a' }).write(logMsg + '\n')
  }
}

var logFactory = {
  getLogger: function (caller) {
    var logger = {}
    logger.caller = caller
    logger.debug = function (msg) {
      log('debug', logger, msg)
    }
    logger.info = function (msg) {
      log('info', logger, msg)
    }
    logger.error = function (msg) {
      log('error', logger, msg)
    }
    return logger
  }
}
module.exports = logFactory
