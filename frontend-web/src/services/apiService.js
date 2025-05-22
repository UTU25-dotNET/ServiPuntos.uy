import axios from "axios";
import authService from "./authService";

const API_URL = "https://localhost:5019/api";

const apiClient = axios.create({
  baseURL: API_URL,
  headers: {
    "Content-Type": "application/json",
  },
  withCredentials: true, // si usas cookies de sesión
});

// Interceptor para añadir el token a todas las peticiones
apiClient.interceptors.request.use(
  (config) => {
    const token = authService.getToken();
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Interceptor para manejar errores de autenticación
apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      authService.logout();
      window.location.href = "/login";
    }
    return Promise.reject(error);
  }
);

const apiService = {
  // Perfil de usuario (simulado)
  getUserProfile: async () => {
    const user = authService.getCurrentUser();
    if (!user) throw new Error("Usuario no autenticado");
    return {
      id: user.id,
      username: user.username,
      email: user.email,
      nombre: `${user.nombre} ${user.apellido}`,
      role: user.role,
      lastLogin: new Date().toISOString(),
      status: "active",
    };
  },

  // HTTP genéricos
  get: async (endpoint) => {
    const res = await apiClient.get(endpoint);
    return res.data;
  },
  post: async (endpoint, payload) => {
    const res = await apiClient.post(endpoint, payload);
    return res.data;
  },
  put: async (endpoint, payload) => {
    const res = await apiClient.put(endpoint, payload);
    return res.data;
  },
  delete: async (endpoint) => {
    const res = await apiClient.delete(endpoint);
    return res.data;
  },

  // Tenants
  getTenants: () => apiClient.get("/tenant"),

  // Simulación NAFTA
  simulateTransaccion: (mensaje) =>
    apiClient.post("/nafta/transaccion", mensaje),
};

export default apiService;
