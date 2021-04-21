# SugarChat

##Database Configurations

- Install MongoDb

You can run mongo from anywhere if you want and docker is recommended:

docker run -p 27017:27017 --name mongodb -d mongo

- Fit to ConnectionString

You can use any magic to setup your local DNS like:

Edit your hosts file as Administrator in Windows environment, and append a line

xxx.xxx.xxx.xxx    mongoserver

And it will work, no user and password needed.
