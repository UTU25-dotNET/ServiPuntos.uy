import axios from "axios";
import authService from "./authService";

const API_URL = "https://localhost:5019/api/";

const apiClient = axios.create({
  baseURL: API_URL,
  headers: {
    "Content-Type": "application/json",
  },
});

// Interceptor para añadir el token a todas las peticiones
apiClient.interceptors.request.use(
  (config) => {
    const token = authService.getToken();
    if (token) {
      config.headers["Authorization"] = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Interceptor para manejar errores de autenticación
apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    // Si recibimos un 401 (Unauthorized)
    if (error.response && error.response.status === 401) {
      // Redirijo al login
      authService.logout();
      window.location.href = "/login";
    }
    return Promise.reject(error);
  }
);

// Cliente HTTP
const apiService = {
  // Métodos simulados para desarrollo
  getUserProfile: async () => {
    try {
      // Simulación para desarrollo
      const user = authService.getCurrentUser();
      if (!user) {
        throw new Error("Usuario no autenticado");
      }

      // Crear un perfil simulado basado en el token
      return {
        id: user.id,
        username: user.username,
        email: user.email,
        nombre: user.nombre + " " + user.apellido,
        role: user.role,
        lastLogin: new Date().toISOString(),
        status: "active",
      };

      // Descomentar cuando tengas el backend listo
      /*
      const response = await apiClient.get('usuario/perfil');
      return response.data;
      */
    } catch (error) {
      console.error("Error obteniendo perfil:", error);
      throw error.response?.data || error;
    }
  },

  // Métodos HTTP genéricos
  get: async (endpoint) => {
    try {
      const response = await apiClient.get(endpoint);
      return response.data;
    } catch (error) {
      console.error(`Error en GET ${endpoint}:`, error);
      throw error;
    }
  },

  post: async (endpoint, data) => {
    try {
      const response = await apiClient.post(endpoint, data);
      return response.data;
    } catch (error) {
      console.error(`Error en POST ${endpoint}:`, error);
      throw error;
    }
  },

  put: async (endpoint, data) => {
    try {
      const response = await apiClient.put(endpoint, data);
      return response.data;
    } catch (error) {
      console.error(`Error en PUT ${endpoint}:`, error);
      throw error;
    }
  },

  delete: async (endpoint) => {
    try {
      const response = await apiClient.delete(endpoint);
      return response.data;
    } catch (error) {
      console.error(`Error en DELETE ${endpoint}:`, error);
      throw error;
    }
  },
};

export default apiService;
