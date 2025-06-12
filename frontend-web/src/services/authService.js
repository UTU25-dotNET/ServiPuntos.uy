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

const API_URL = "https://localhost:5019/api/auth/";

const authService = {
  // Registro de usuario

  getTenants: async () => {
    try {
<<<<<<< HEAD
      console.log("Obteniendo lista de tenants...");
      
      const response = await axios.get(`${API_URL}tenants`);
      console.log("Tenants obtenidos:", response.data);
      
      return response.data;
    } catch (err) {
      console.error("Error al obtener tenants:", err);
=======
      
      const response = await axios.get(`${API_URL}tenants`);
      
      return response.data;
    } catch (err) {
>>>>>>> origin/dev
      
      if (err.response) {
        const errorMessage = err.response.data?.message || 
                            err.response.data?.error || 
                            `Error del servidor: ${err.response.status}`;
<<<<<<< HEAD
        throw { message: errorMessage };
      } else if (err.request) {
        throw { message: "Error de conexión. Verifica tu conexión a internet." };
      } else {
        throw { message: err.message || "Error desconocido al obtener tenants" };
=======
        throw new Error(errorMessage);
      } else if (err.request) {
        throw new Error("Error de conexión. Verifica tu conexión a internet.");
      } else {
        throw new Error(err.message || "Error desconocido al obtener tenants");
>>>>>>> origin/dev
      }
    }
  },


register: async (name, email, password, ci, tenantId) => {
  try {
<<<<<<< HEAD
    console.log("Iniciando registro para:", { name, email, ci, tenantId });
=======
>>>>>>> origin/dev

    const response = await axios.post(`${API_URL}register`, {
      nombre: name,
      email: email,
      password: password,
      ci: ci,
      tenantId: tenantId // **Enviar como string (Guid)**
    });

<<<<<<< HEAD
    console.log("Respuesta del registro:", response.data);
=======
>>>>>>> origin/dev
    
    return {
      success: true,
      message: response.data.message || "Usuario registrado exitosamente",
      data: response.data
    };

  } catch (err) {
<<<<<<< HEAD
    console.error("Error en el registro:", err);
=======
>>>>>>> origin/dev
    
    if (err.response) {
      const errorMessage = err.response.data?.message || 
                           err.response.data?.error || 
                          `Error del servidor: ${err.response.status}`;
<<<<<<< HEAD
      throw { message: errorMessage };
    } else if (err.request) {
      throw { message : "Error de conexión. Verifica tu conexión a internet." };
    } else {
      throw { message: err.message || "Error desconocido en el registro" };
=======
      throw new Error(errorMessage);
    } else if (err.request) {
      throw new Error("Error de conexión. Verifica tu conexión a internet.");
    } else {
      throw new Error(err.message || "Error desconocido en el registro");
>>>>>>> origin/dev
    }
  }
},

  // Inicio de sesión
  login: async (email, password) => {
    try {
      // Para desarrollo, usamos tokens hardcodeados
<<<<<<< HEAD
      console.log("Login simulado para:", { email, password });
=======
>>>>>>> origin/dev

      // Simulamos la lógica de login
      let token;

      const response = await axios.post(`${API_URL}signin`, {
        email,
        password
      });
      
      token = response.data.token;

      if (!token) {
<<<<<<< HEAD
        throw { message: "Credenciales inválidas" };
      } else {
        // Guardar el token en localStorage
        console.log("Token recibido:", token);
        localStorage.setItem('token', response.data.token);
      }
      // localStorage.setItem("token", token);
      return response.data;
      // if (email === "admin@gmail.com" && password === "admin") {
      //   token = tokenUtils.adminToken;
      // } else if (email === "user@gmail.com" && password === "user") {
      //   token = tokenUtils.userToken;
      // } else {
      //   throw { message: "Credenciales inválidas" };
      // }

      // Guardar el token
      
      // window.location.href = '/dashboard'; -> no es necesario xq esta en el componente del Login, handleSubmit.
      // return {
      //   token,
      //   user: tokenUtils.getUserFromToken(token),
      // };
=======
        throw new Error("Credenciales inválidas");
      } else {
        // Guardar el token en localStorage
        localStorage.setItem('token', response.data.token);
        scheduleAutoLogout();
      }
      return response.data;
>>>>>>> origin/dev

      // // Descomentar cuando tengas el backend listo
      
      //       const response = await axios.post(`${API_URL}signin`, {
      //         email,
      //         password
      //       });
            
      //       if (response.data.token) {
      //         localStorage.setItem('token', response.data.token);
      //       }
            
      //       return response.data;
            
    } catch (error) {
<<<<<<< HEAD
      throw (
        error.response?.data ||
        error || { message: "Error en el inicio de sesión" }
      );
    }
  },

=======
      const message =
        error.response?.data?.message || error.message || "Error en el inicio de sesión";
      throw new Error(message);
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


>>>>>>> origin/dev
  // Cerrar sesión
  logout: async () => {
    // Primero verificamos si el usuario se logueó con Google
    const token = localStorage.getItem("token");
<<<<<<< HEAD
=======
    if (logoutTimer) {
      clearTimeout(logoutTimer);
      logoutTimer = null;
    }
>>>>>>> origin/dev
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
<<<<<<< HEAD
          console.log("Detectado login con Google, revocando sesión...");

          // Llamar al endpoint de logout del backend para revocar sesión de Google
          try {
            console.log("Revocando sesión de Google...");
            console.log("Token:", token);
=======

          // Llamar al endpoint de logout del backend para revocar sesión de Google
          try {
>>>>>>> origin/dev
            await axios.get(`${API_URL}logout`, {
              headers: {
                Authorization: `Bearer ${token}`, // Enviamos el token para que el backend identifique la sesión
              },
            });
<<<<<<< HEAD
            console.log("Sesión de Google revocada exitosamente");
          } catch (error) {
            console.error("Error al revocar sesión de Google:", error);
=======
          } catch (error) {
>>>>>>> origin/dev
            // Continuamos con el logout local aunque falle la revocación en Google
          }
        }
      } catch (error) {
<<<<<<< HEAD
        console.error("Error al decodificar token durante logout:", error);
=======
>>>>>>> origin/dev
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

<<<<<<< HEAD
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
=======
  scheduleAutoLogout,
};

export default authService;

// Al cargar el servicio, programar cierre automático si existe un token
scheduleAutoLogout();
>>>>>>> origin/dev
