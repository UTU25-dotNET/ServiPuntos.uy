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
      return jwtDecode(token);
    } catch (error) {
      return null;
    }
  },
};

export default tokenUtils;
