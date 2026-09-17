# Faza izgradnje - prevodi React aplikaciju u staticke datoteke.
FROM node:22-alpine AS build
WORKDIR /app

COPY package.json package-lock.json* .npmrc ./
RUN npm install --legacy-peer-deps

COPY . .
RUN npm run build

# Faza izvrsavanja - Nginx posluzuje staticke datoteke i prosledjuje /api pozive serverskom delu.
FROM nginx:alpine AS final
COPY --from=build /app/dist /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
