FROM node:22-alpine AS build

WORKDIR /app
COPY package.json tsconfig.json ./
COPY src ./src
RUN npm install && npm run build

FROM nginx:1.27-alpine

COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY . /usr/share/nginx/html
COPY --from=build /app/js /usr/share/nginx/html/js

EXPOSE 80
