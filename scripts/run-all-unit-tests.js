const path = require('path')
const shell = require('shelljs')
const chalk = require('chalk')
const figlet = require('figlet')

var printInformation = function (information) {
  shell.echo('')
  shell.echo(chalk.gray(information))
  shell.echo('')
}

shell.echo(chalk.green.bold(figlet.textSync('testing...', 'Computer')))

printInformation('running all unit tests')

function runUnitTest (service) {
  shell.cd(path.join(__dirname, `../${service}/unittests`))
  printInformation('running (dotnet core xunit) unit tests in ' + shell.pwd())
  if (shell.exec('dotnet test').code !== 0) {
    printInformation(`/${service}/unittests failed\n`)
    return false
  } else {
    printInformation(`/${service}/unittests passed\n`)
    return true
  }
}
var allTestsPassed = (a, b) => {
  let prevPassed = a === true
  return prevPassed && b === true
}
var servicesWithTests = ['tracker', 'analyzer', 'storage']

var allPassed = servicesWithTests.map(runUnitTest).reduce(allTestsPassed, true)

if (allPassed) {
  shell.echo(chalk.green('all tests passed'))
} else {
  shell.echo(chalk.red('tests failed'))
}
