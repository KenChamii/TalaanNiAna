// Local development, where the API runs as a separate process.
//
// HTTPS on 7131, which needs the "https" launch profile. Plain HTTP does not
// work here: UseHttpsRedirection turns the CORS preflight into a redirect, and
// a preflight request is not allowed to follow one, so the browser blocks the
// call before it is sent.
export const environment = {
  production: false,
  apiUrl: 'https://localhost:7131/api'
};
