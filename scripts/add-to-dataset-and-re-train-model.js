const path = require('path')
const shell = require('shelljs')
const chalk = require('chalk')
const figlet = require('figlet')

var printInformation = function (information) {
  shell.echo('')
  shell.echo(chalk.gray(information))
  shell.echo('')
}
var printError = function (information) {
  shell.echo('')
  shell.echo(chalk.red(information))
  shell.echo('')
}

shell.echo(chalk.green.bold(figlet.textSync('train model', 'Computer')))

if (!shell.which('dotnet')) {
  printError('Sorry, this script requires dotnet core')
  shell.exit(1)
}

if (!shell.test('-e', path.join(__dirname, '../data/articles.json'))) {
  printError(
    `you have to run MediaSpin first and have tracker write all fetched articles to disk, see tracker/secrets.env (setting writeArticlesToFile)`
  )
  shell.exit(1)
}

printInformation(
  'running dataset-gen - adding rows to article.tsv (the dataset) by processing articles.json'
)
shell.cd('../dataset-gen')
shell.exec(`node ${path.join(__dirname, '../dataset-gen/index.js')}`)

printInformation(
  're-training the SentimentModel with our newly updated articles.tsv'
)
shell.cd('../trainer')
shell.exec('dotnet run')

printInformation(
  'copying the new SentimentModel.zip to ../analyzer/src/MLModel - replacing the old one'
)
shell.cd('../data')
shell.cp(
  './SentimentModel.zip',
  path.join(__dirname, '../analyzer/src/MLModel')
)

printInformation(
  'finally removing the articles.json file to avoid adding the same rows to the dataset if executed multiple times'
)
shell.cd('../data')
shell.rm('articles.json')

printInformation('finished re-train the sentiment model')
shell.exit(0)
