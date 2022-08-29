# SugarChat

##Database Configurations

- Install MongoDb
- You need to deploys Replia Sets

You can run mongo from anywhere if you want and docker is recommended:

```shell
# create network
docker network create --subnet=x.x.x.0/24 mongoDbNetwork

# creat image
docker run --name mongo --network mongoDbNetwork --ip x.x.x.2 --restart always -p 27017:27017 -d mongo mongod --replSet "rs"

# init Replia Sets
rs.initiate({ _id: "rs", members: [{_id:0,host:"x.x.x.2:27017"}]})
```

- Fit to ConnectionString

You can use any magic to setup your local DNS like:

Edit your hosts file as Administrator in Windows environment, and append a line

xxx.xxx.xxx.xxx    mongoserver

And it will work, no user and password needed.
