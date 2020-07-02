

# mern boilerplate

###### mern stack: ` mongodb, expresjs, React.js, node.js `

## end points
* `POST`/images  [?sort=[order]]
  * request body: ["img-url"] 
    * img-url: a String of any pulcic imge url address. 
    In.jpeg, .jpg, .png formats.
  * order (*optional): 'decending' or 'acending'

###### Installation

- clone the git repository:
`git clone https://github.com/ronerlih/project-3-boiler-authentication`

- install dependencies: ```npm i```

- manually install bcrypt (`npm i bcrypt`)
  - windows: (tricky, make sure to run in powershell in admin mode)
  - mac/linux: run with `sudo`

- run localy (dev enviorment): 
  - install mongoDB and run server locally.
  - Set up an ```.env``` file in root project folder with Redis keys (holds user sessions):
  REDIS_URL=[your-redis-service-url]
REDIS_PASS=[your-redis-service-password]
  - ```npm start```
  - no cluster: ```npm run start:dev-no-cluster```
  - no server side rendering: ```npm run start:dev-no-ssr```
- run in production (Heroku):
  
  - build to Heroku:
    - ```heroku login```
    - ```heroku create [unique-name-of-app]```
    - add mongo and redis add-on: on Heroku dashboard, inside your app click add-ons and add 'mLab' form mongodb and 'Redis'.
    - check the heroku env variables where added (on settings)
    - ```git push heroku master```


###### Packages information
- logging: [morgan docs](https://www.npmjs.com/package/morgan)

- user manegement: [express-session](link)

- mongodb ORM: [mongoose](link)
- internal load balancing with cluster: [node cluster docs](https://nodejs.org/api/cluster.html)

- serve compressed: [commpression](link)

- cache and session storage: [Redis](link)
  - (important!) set up heroku redis eviction policy: `heroku redis:maxmemory  --policy=volatile-lru`
  - heroku redis cli: `heroku redis:cli --confirm [heroku-app-name]`
  - e.g. clear redis db (from cli) `FLUSHALL`

- load tests: [loadtest](https://www.npmjs.com/package/loadtest)
  - suggested test: `loadtest http://localhost:3001/ -k -n 10000 -c 1000`
  - with cookie (add you cookie value according to session): `loadtest http://localhost:3001/ -k -n 10000 -c 1000 -C cookie-__id=s%3AcvzqMmnAaar3bIknaKsQSsFiQNGbFRC1.c4US2JD17dcuBg1zA46jhdzBuE0O2pglgO6STcQrkCI` 
  - on heroku: `loadtest https://mern-stack-boilerplat.herokuapp.com/ -k -n 10000 -c 1000`

- Docker deployment
  - [Docker docs](https://docs.docker.com/)
  - (Dockerfile config in root)
  - commands and process:
    - `docker build -t boiler-docker .`
    - `docker images`
    - `docker run -p 49160:3001 -d boiler-docker`
    - `docker swarm -p 3001:3001 -d boiler-docker`
    - `docker ps`
    - `curl -i localhost:49160`
    - from docker guides: [docs](https://nodejs.org/de/docs/guides/nodejs-docker-webapp/)
  
- load tests with wrk:
  - [documentation](https://github.com/wg/wrk)
    - `brew install wrk`
    - i.e. `wrk -t8 -c100 -d30s http://localhost:3001/` 

###### project structure
```
MERN-BOILERPLATE
├── client // front end (react app)
│   ├── README. md
│   ├── build `to be served in production or on server side rendering`
│   │   ├── asset-manifest.json
│   │   ├── favicon.ico
│   │   ├── index.html
│   │   ├── manifest.json
│   │   ├── precache-manifest.50ab651f5c82451e623a23f949bf95cb.js
│   │   ├── service-worker.js
│   │   └── static
│   ├── package-lock.json
│   ├── package.json // of react app
│   ├── public
│   │   ├── favicon.ico
│   │   ├── index.html
│   │   └── manifest.json
│   └── src
│       ├── App.js // root level component
│       ├── components
│       ├── index.js `root level page`
│       ├── pages
│       └── utils
  ----------- server -----------
├── index.js // entery point + transpile server from es6 + cluster it (runs server.js)`
├── controllers // routes function/handlers
│   ├── booksController.js
│   ├── serverSideRendering.js
│   ├── userController.js
│   └── workerController.js
├── models // db collections - mongoose
│   ├── book.js
│   ├── index.js
│   └── user.js
├── notes.txt // development notepad
├── Dockerfile // docker config
├── package-lock.json
├── package.json
├── readme.md
├── routes // routing
│   ├── api // api routes 
│   │   ├── books.js
│   │   ├── index.js
│   │   ├── user.js
│   │   └── worker.js
│   └── index.js // split to ssr and api routes
├── scripts // modulerizing scripts to here
│   ├── cluster.js
│   ├── errorHandler.js
│   ├── redis.js
│   ├── seedDB.js // seed db collections
│   └── session.js
└── server.js //express server 
```

###### additional resources
- [Heroku scaling documentaion](
https://devcenter.heroku.com/articles/scaling)
- [Node.js produciton best practices](https://www.cognitiveclouds.com/insights/top-node-js-production-best-practices)

- [Docker getting started docs](https://docs.docker.com/get-started/)

- [cluster communication](https://medium.com/js-imaginea/clustering-inter-process-communication-ipc-in-node-js-748f981214e9)


> [git repo](https://github.com/ronerlih/mern-boilerplate)

