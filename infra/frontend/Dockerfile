FROM node:20-alpine AS build

WORKDIR /app

ARG VITE_API_URL=https://api.redmobile.com.br/api
ARG VITE_MAPBOX=
ENV VITE_API_URL=$VITE_API_URL
ENV VITE_MAPBOX=$VITE_MAPBOX

COPY frontend/package*.json ./
RUN npm ci

COPY frontend/ .
RUN npm run build

FROM nginx:alpine AS runtime

COPY infra/frontend/nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist /usr/share/nginx/html

EXPOSE 80

HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD wget -q -O /dev/null http://127.0.0.1/health || exit 1

CMD ["nginx", "-g", "daemon off;"]