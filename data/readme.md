tracker generates a articles.json  
dataset-gen takes a articles.json and creates a labeled dataset articles.tsv  
trainer takes articles.tsv and creates a SentimentModel.zip  
this SentimentModel.zip is what we put in analyzer/src/MLModel  