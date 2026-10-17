# Build context is this project directory.
#   docker build -f frontend/talaan-ni-ana/Dockerfile frontend/talaan-ni-ana

FROM node:20-alpine AS build
WORKDIR /src

# Lockfile first so npm ci is cached independently of source changes.
COPY package.json package-lock.json ./
RUN npm ci

COPY . .

# Served behind nginx on one origin, so the bundle calls /api rather than the
# deployed API's own domain. That is what environment.compose.ts sets.
RUN npm run build -- --configuration compose

FROM nginx:1.27-alpine AS runtime

COPY nginx.conf /etc/nginx/conf.d/default.conf

# The application builder emits to dist/<project>/browser.
COPY --from=build /src/dist/talaan-ni-ana/browser /usr/share/nginx/html

EXPOSE 80
