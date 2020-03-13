FROM node:12
# Create app directory
WORKDIR /usr/src/app
# Install app dependencies
# A wildcard is used to ensure both package.json AND package-lock.json are copied
# where available (npm@5+)
COPY package*.json ./

RUN npm install

# Bundle app source
COPY . .
# set command to expose env variable
EXPOSE 3001

# production
CMD [ "npm", "run", "start:prod" ]
#development
# CMD [ "npm", "run", "start:dev" ]