import axios from "axios";
import tokenUtils from "../utils/tokenUtils";

const API_URL = "https://localhost:5001/api/auth/";

const authService = {
  // Registro de usuario
  // Versión corregida para la función register en authService.js
  register: async (name, email, password) => {
    try {
      // Para desarrollo, simulamos el registro
      console.log("Registro simulado para:", { name, email, password });
      return { success: true };

      // Descomentar cuando tengas el backend listo
      /*
      const response = await axios.post(`${API_URL}registrar`, {
        username: name,
        email,
        password
      });
      return response.data;
      */
    } catch (err) {
      throw err.response?.data || err || { message: "Error en el registro" };
    }
  },

  // Inicio de sesión
  login: async (email, password) => {
    try {
      // Para desarrollo, usamos tokens hardcodeados
      console.log("Login simulado para:", { email, password });

      // Simulamos la lógica de login
      let token;
      if (email === "admin@gmail.com" && password === "admin") {
        token = tokenUtils.adminToken;
      } else if (email === "user@gmail.com" && password === "user") {
        token = tokenUtils.userToken;
      } else {
        throw { message: "Credenciales inválidas" };
      }

      // Guardar el token
      localStorage.setItem("token", token);
      // window.location.href = '/dashboard'; -> no es necesario xq esta en el componente del Login, handleSubmit.
      return {
        token,
        user: tokenUtils.getUserFromToken(token),
      };

      // Descomentar cuando tengas el backend listo
      /*
      const response = await axios.post(`${API_URL}ingresar`, {
        email,
        password
      });
      
      if (response.data.token) {
        localStorage.setItem('token', response.data.token);
      }
      
      return response.data;
      */
    } catch (error) {
      throw (
        error.response?.data ||
        error || { message: "Error en el inicio de sesión" }
      );
    }
  },

  // Cerrar sesión
  logout: () => {
    localStorage.removeItem("token");

    window.location.href = "/";
  },

  // Comprobar si el usuario está autenticado
  isAuthenticated: () => {
    try {
      const token = localStorage.getItem("token");
      if (!token) return false;

      return !tokenUtils.isTokenExpired(token);
    } catch (error) {
      return false;
    }
  },

  // Obtener el usuario actual
  getCurrentUser: () => {
    try {
      const token = localStorage.getItem("token");
      if (!token) return null;

      return tokenUtils.getUserFromToken(token);
    } catch (error) {
      return null;
    }
  },

  // Obtener el token
  getToken: () => {
    return localStorage.getItem("token");
  },

  // Para desarrollo - hardcodear un token específico
  setHardcodedToken: (tokenType) => {
    switch (tokenType) {
      case "user":
        localStorage.setItem("token", tokenUtils.userToken);
        return tokenUtils.userToken;
      case "admin":
        localStorage.setItem("token", tokenUtils.adminToken);
        return tokenUtils.adminToken;
      case "expired":
        localStorage.setItem("token", tokenUtils.expiredToken);
        return tokenUtils.expiredToken;
      default:
        return null;
    }
  },
};

export default authService;
