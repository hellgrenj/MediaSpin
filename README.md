# MediaSpin

## Prerequisites

You need to have the following tech installed locally:

* dotnet core 3.1 (LTS)
* node 12.16 (LTS)
* docker version 18.x. 
* docker-compose version 1.23.x. 

## Start the system

* clone this repo
* run ``npm install`` in /scripts
* run ``node scripts/setup``
* run ``docker-compose up``

## Test
run ``node script/run-all-unit-tests.js``

## Grow the dataset and re-train the model 
First run the system and let tracker execute a full article fetch. Make sure that tracker has the writeArticlesToFile env var set to true. Then navigate to the script folder. In the script folder run     
  
 ``node add-to-dataset-and-re-train-model.js``
 
 (more about the dataset under Challanges below)

## What and why
The idea is to track how Swedish media reports on different subjects by using sentiment analysis.
This hobby project scratches two itches at once for me;  
1) I find the idea interesting  
2) I use this solution to learn, try and evaluate different *things* ... (hence the overengineering,
 complex architecture and different versions of a single component)


## Conceptual overview
             tracker (tracks and extracts articles containing keywords)  
                |
                |
             analyzer (analyzes sentiment)
                |
                |
             storage (stores analysis)
            /       \          
           /         \
      visualizer     bot



## Tech and other stuff in this project
* Kubernetes (yaml files)
* Docker & Docker Compose
* dotnet core 3 (web/service and worker)
* ML.NET
* Sentiment (AFINN-based sentiment analysis for Node.js)
* gRPC
* NodeServices in .net core (soon to be replaced...)
* Ef core
* Postgres
* RabbitMQ
* Client side Blazor (WebAssembly) - hence the initial load time :)
* Vue.js (Currently porting the client, see visualizer2)  
* The mediator pattern (without a library)
* CQRS
* Some hexagonal architecture principles in a microservices setting

## Challanges 
I have not been able to find a good Swedish dataset to train my ML.NET model with. So, for now, I am doing the following: 

I am using a Swedish translated AFINN lexicon and word lists I found on Kaggle to generate a dataset from actual articles pulled down by the tracker component.

See /dataset-gen/index.js and scripts/add-to-dataset-and-re-train-model.js and the code in tracker that writes articles to a file to see how this works. 

Its actually working better than I thought it would. But the current model still gets it really wrong sometimes so I am using an AFINN server as a safety net (see analyzer - Engine.cs).
