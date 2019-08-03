const path = require('path')
const shell = require('shelljs')
const chalk = require('chalk')
const figlet = require('figlet')

var printInformation = function (information) {
  shell.echo('')
  shell.echo(chalk.gray(information))
  shell.echo('')
}

shell.echo(chalk.green.bold(figlet.textSync('Media Spin', 'Computer')))

printInformation(
  'copying default .env files foreach service to gitingored folder ./environment'
)

if (!shell.test('-e', path.join(__dirname, '../environment'))) {
  shell.mkdir(path.join(__dirname, '../environment'))
}
shell.cp('-R', path.join(__dirname, './default_env_files/*.env'), path.join(__dirname, '../environment'))
