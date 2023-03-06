# RoadRager

NOTE: Please look at model/ folder for information about training the model as well as GCF code.

## Inspiration
Recently, one of our team members was involved in a road rage incident. After unintentionally cutting off a recklessly fast driver on the I-405, the driver attempted to follow him home until he was lost after a few turns.

We wanted to see if we could use machine learning to teach the general populace, especially in a city as traffic-prone as LA, good driving habits and attitudes to prevent road rage incidents.

## What it does
RoadRager is a 2D-style game that allows the user to control a vehicle in a fictional universe. There is a car in front of the vehicle, and they must be cautious not to be too aggressive a driver, especially when weather and road conditions constantly change.

The driver is rated by a slider that informs the user how aggressively they are driving. The driving data is uploaded to a Google Cloud endpoint, where it is analyzed against machine learning model of our own design, trained on real-world driving data.

The user must drive cautiously in order not to be rated as aggressive. We hope this will build good driving habits. 

## How we built it
The frontend (game) is built using Unity and the C# language. It is a 2D game with endless sprites. The game simultaneously collects data on how the user is driving (such as the speed and how closely they are tailgating) as well as environmental data such as weather. This data is sent to a Google Cloud endpoint, where a python script puts it into the model and gets an aggression rating.

The model was originally analyzed and trained locally. We had to integrate multiple tables of data and analyze features for importance. Finally, this involves training a logistic regression model and then saving is to a Pickle file. The Pickle file is used by the integration script on Google Cloud to do analysis of driving behavior. To gather the data for this project, we used an Aggressive Driving dataset by Veerala Krishna. The dataset is listed here: https://www.kaggle.com/datasets/veeralakrishna/aggressive-driving-data.

Lastly, the Google Cloud Function is activated whenever Unity makes a call to the HTTP endpoint with the appropriate driving condition data. This endpoint is activated frequently and given the data from the Unity game.

## Challenges we ran into
We ran into a lot of challenges. We are not new to hackathons as a team, but we wanted to use this smaller hackathon as an opportunity to do a smaller project where we can learn to implement a machine learning data model. Thus, everything was new to us. The machine learning model was originally to be built with PyTorch but we ran into compiling bugs and thus switched frameworks. The unity game scene we worked on got deleted so we had to rebuild it from scratch with less than 3 hours on the clock. We ran into issues with AWS and Google Cloud as well.

## Accomplishments that we're proud of
We’re proud of ourselves for taking the risk and trying something completely out of our comfort zone, and getting somewhere with it! We’re proud of the look of the game and though it’s simple, it hides a lot of technical complexity we worked hard on.

## What we learned
Machine learning, actually exporting a data model for production use, and utilizing Unity with shell scripts to connect the two. Parallel program development and integration as well was challenging and useful to learn.

We learned about Google Cloud Platform as well.

## What's next for Road Rager
It’d be possible to create a more accurate simulator and utilize more of the parameters that affect road rage. We could also train the model with more fine tuned settings for more accurate predictions. Furthermore, because the model tests various factors, then comes to a conclusion using logistical regression, we could also use the model’s information to give feedback about user’s driving.
