// The docker-compose stack, where nginx serves this bundle and proxies /api to
// the API container. Browser and API share an origin, so the URL is relative
// and CORS does not apply at all.
export const environment = {
  production: true,
  apiUrl: '/api'
};
