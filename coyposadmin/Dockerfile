FROM node:18
WORKDIR /app

# Install app dependencies
COPY package*.json ./
RUN yarn install

# Bundle app source
COPY . .

EXPOSE 8080
CMD [ "yarn", "serve" ]