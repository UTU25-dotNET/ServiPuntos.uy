import axios from "axios";
import tokenUtils from "../utils/tokenUtils";

let logoutTimer = null;

const scheduleAutoLogout = () => {
  const token = localStorage.getItem("token");
  if (!token) {
    return;
  }

  try {
    const decoded = tokenUtils.getUserFromToken(token);
    if (!decoded || !decoded.exp) return;

    const expiresInMs = decoded.exp * 1000 - Date.now();
    if (expiresInMs <= 0) {
      authService.logout();
      return;
    }

    if (logoutTimer) {
      clearTimeout(logoutTimer);
    }

    logoutTimer = setTimeout(() => {
      authService.logout();
    }, expiresInMs);
  } catch (err) {
  }
};
// Dirección base de la API de autenticación
// Nota: se corrige la URL para incluir "//" después de "https:".
// De lo contrario, las peticiones se realizan al dominio actual en vez del backend.
//const API_URL = "https://localhost:5019/api/auth/";
 const API_URL = "https://ec2-18-220-251-96.us-east-2.compute.amazonaws.com:5019/api/auth/";

//const API_URL = "https://ec2-18-220-251-96.us-east-2.compute.amazonaws.com:5019/api/auth/";


const authService = {
  // Registro de usuario

  getTenants: async () => {
    try {
      
      const response = await axios.get(`${API_URL}tenants`);
      
      return response.data;
    } catch (err) {
      
      if (err.response) {
        const errorMessage = err.response.data?.message || 
                            err.response.data?.error || 
                            `Error del servidor: ${err.response.status}`;
        throw new Error(errorMessage);
      } else if (err.request) {
        throw new Error("Error de conexión. Verifica tu conexión a internet.");
      } else {
        throw new Error(err.message || "Error desconocido al obtener tenants");
      }
    }
  },


register: async (name, email, password, ci, tenantId) => {
  try {

    const response = await axios.post(`${API_URL}register`, {
      nombre: name,
      email: email,
      password: password,
      ci: ci,
      tenantId: tenantId // **Enviar como string (Guid)**
    });

    
    return {
      success: true,
      message: response.data.message || "Usuario registrado exitosamente",
      data: response.data
    };

  } catch (err) {
    
    if (err.response) {
      const errorMessage = err.response.data?.message || 
                           err.response.data?.error || 
                          `Error del servidor: ${err.response.status}`;
      throw new Error(errorMessage);
    } else if (err.request) {
      throw new Error("Error de conexión. Verifica tu conexión a internet.");
    } else {
      throw new Error(err.message || "Error desconocido en el registro");
    }
  }
},

  // Inicio de sesión
  login: async (email, password) => {
    try {
      // Para desarrollo, usamos tokens hardcodeados

      // Simulamos la lógica de login
      let token;

      const response = await axios.post(`${API_URL}signin`, {
        email,
        password
      });
      
      token = response.data.token;

      if (!token) {
        throw new Error("Credenciales inválidas");
      } else {
        // Guardar el token en localStorage
        localStorage.setItem('token', response.data.token);
        scheduleAutoLogout();
      }
      return response.data;


    } catch (error) {
      const message =
        error.response?.data?.message || error.message || "Error en el inicio de sesión";
      throw new Error(message);
    }
  },

  // Guardar un token ya obtenido (por ejemplo, tras login con Google)
  loginWithToken: (token) => {
    if (token) {
      localStorage.setItem('token', token);
      scheduleAutoLogout();
    }
  },

  verifyPassword: async (email, password) => {
    try {
      await axios.post(`${API_URL}verify-password`, { email, password });
      return true;
    } catch (err) {
      const message =
        err.response?.data?.message || err.message || "Error al verificar contraseña";
      throw new Error(message);
    }
  },


  // Cerrar sesión
  logout: async () => {
    // Primero verificamos si el usuario se logueó con Google
    const token = localStorage.getItem("token");
    if (logoutTimer) {
      clearTimeout(logoutTimer);
      logoutTimer = null;
    }
    if (token) {
      try {
        const decodedToken = tokenUtils.getUserFromToken(token);

        // Verificar si el usuario se autenticó con Google
        // Podemos asumir que si existe un claim 'sub' y el email termina en gmail.com,
        // o si hay un claim específico que indique autenticación con Google
        if (
          decodedToken &&
          (decodedToken.sub || decodedToken.email?.endsWith("@gmail.com"))
        ) {

          // Llamar al endpoint de logout del backend para revocar sesión de Google
          try {
            await axios.get(`${API_URL}logout`, {
              headers: {
                Authorization: `Bearer ${token}`, // Enviamos el token para que el backend identifique la sesión
              },
            });
          } catch (error) {
            // Continuamos con el logout local aunque falle la revocación en Google
          }
        }
      } catch (error) {
        // Continuamos con el proceso de logout local
      }
    }

    // Siempre eliminamos el token del localStorage
    localStorage.removeItem("token");

    // Redirigimos a la página de inicio
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

  // Decodificar un token JWT
  decodeToken: (token) => {
    return tokenUtils.decodeToken(token);
  },

  scheduleAutoLogout,
};

export default authService;

// Al cargar el servicio, programar cierre automático si existe un token
scheduleAutoLogout();
