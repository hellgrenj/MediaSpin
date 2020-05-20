FROM node:12.16.3
WORKDIR /app/src
COPY ./src /app/src
RUN npm install -only=production
ENTRYPOINT ["npm","run","start"]