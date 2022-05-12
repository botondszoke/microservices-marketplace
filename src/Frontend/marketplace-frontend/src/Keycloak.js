import Keycloak from "keycloak-js";
const keycloakConfig = {
    url: "http://keycloak.localhost/auth",
    realm: 'buyte',
    clientId: 'buyte-frontend'
}
const keycloak = new Keycloak(keycloakConfig);
export default keycloak