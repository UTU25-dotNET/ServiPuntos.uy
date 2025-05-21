import { jwtDecode } from "jwt-decode";

const tokenUtils = {
  // Token de usuario normal (válido por 24h desde ahora)
  userToken:
    "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjEiLCJ1c2VybmFtZSI6InVzdWFyaW9fbm9ybWFsIiwiZW1haWwiOiJ1c3VhcmlvQGVqZW1wbG8uY29tIiwicm9sZSI6InVzZXIiLCJwZXJtaXNzaW9ucyI6WyJyZWFkIl0sImlhdCI6MTYyMDEyMzQ1NiwiZXhwIjoxOTIwMTY2NjU2fQ.7AMp1fXBmLCFNQvkRGLXjP4_O_ZB2uiSRRjP-KnKpg0",

  // Token de administrador (válido por 24h desde ahora)
  adminToken:
    "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjEyMzQ1Njc4OTAiLCJub21icmUiOiJKdWFuIiwiYXBlbGxpZG8iOiJQZXJleiIsImVtYWlsIjoiYWRtaW5AZ21haWwuY29tIiwidXNlcm5hbWUiOiJhZG1pbiIsInJvbGUiOiJhZG1pbiIsInN0YXR1cyI6ImFjdGl2ZSJ9.wALQ_J6L_pYAAs6xqak2LIs6xzHQgjK-8AfRMgIuA7A",

  // Token expirado (expiró en 2020)
  expiredToken:
    "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjMiLCJ1c2VybmFtZSI6ImV4cGlyZWQiLCJlbWFpbCI6ImV4cGlyZWRAZWplbXBsby5jb20iLCJpYXQiOjE1MDAwMDAwMDAsImV4cCI6MTUwMDAwMDAwMX0.aRbKHS7NqWDRlvyNRGdWZrsgLUJlPvVZ5NeJfq-mGnI",

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
      console.error("Error al decodificar token:", error);
      return null;
    }
  },

  // Verificar si un token está expirado
  isTokenExpired: (token) => {
    try {
      const decodedToken = jwtDecode(token);
      return decodedToken.exp * 1000 < Date.now();
    } catch (error) {
      console.error("Error al verificar expiración del token:", error);
      return true; // Si hay error, consideramos que está expirado
    }
  },

  // Obtener información del usuario desde el token
  getUserFromToken: (token) => {
    try {
      return jwtDecode(token);
    } catch (error) {
      console.error("Error al obtener usuario desde token:", error);
      return null;
    }
  },
};

export default tokenUtils;
