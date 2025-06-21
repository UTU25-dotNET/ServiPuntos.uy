import { jwtDecode } from "jwt-decode";

const tokenUtils = {

  // Función para decodificar un token
  decodeToken: (token) => {
    try {
      // Decodificar el header y payload
      const decoded = {
        payload: jwtDecode(token),
        // La biblioteca jwt-decode solo decodifica el payload por defecto
        // Para obtener el header
        header: jwtDecode(token, { header: true }),

        // Para obtener la firma, separamos el token
        signature: token.split(".")[2],
      };

      return decoded;
    } catch (error) {
      return null;
    }
  },

  // Verificar si un token está expirado
  isTokenExpired: (token) => {
    try {
      const decodedToken = jwtDecode(token);
      return decodedToken.exp * 1000 < Date.now();
    } catch (error) {
      return true; // Si hay error, consideramos que está expirado
    }
  },

  // Obtener información del usuario desde el token
  getUserFromToken: (token) => {
    try {
      const decoded = jwtDecode(token);

      // Normalizar nombre de los claims para mayor compatibilidad
      if (!decoded.email && decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"]) {
        decoded.email = decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"];
      }

      if (!decoded.name && decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"]) {
        decoded.name = decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"];
      }

      // Asegurar consistencia en el identificador de usuario
      if (!decoded.id) {
        decoded.id =
          decoded["nameid"] ||
          decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"] ||
          decoded.sub || null;
      }

      // Unificar claim de tenant
      if (!decoded.tenantId) {
        decoded.tenantId = decoded.tenantId || decoded.TenantId;
      }

      return decoded;
    } catch (error) {
      return null;
    }
  },
};

export default tokenUtils;
