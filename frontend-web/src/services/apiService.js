import axios from "axios";
import authService from "./authService";

//const API_URL = "https://localhost:5019/api/";

const API_URL = "https://ec2-18-220-251-96.us-east-2.compute.amazonaws.com:5019/api/";
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
    if (
      error.response &&
      error.response.status === 401 &&
      !(error.config && error.config.skipAuthError)
    ) {
      // Redirijo al login
      authService.logout();
      window.location.href = "/login";
    }
    return Promise.reject(error);
  }
);

// Cliente HTTP
const apiService = {
  // Obtener perfil del usuario actual
  getUserProfile: async (config = {}) => {
    try {
      // Obtener email del usuario desde el token
      const user = authService.getCurrentUser();
      if (!user || !user.email) {
        throw new Error("Usuario no autenticado o email no disponible");
      }


      try {
        // Intentar usar el endpoint específico por email
        const response = await apiClient.get(
          `usuario/email/${encodeURIComponent(user.email)}`,
          config
        );
        
        return {
          // Información básica
          id: response.data.id,
          tenantId: response.data.tenantId,
          nombre: response.data.nombre,
          apellido: response.data.apellido,
          email: response.data.email,
          ci: response.data.ci,
          rol: response.data.rol,
          telefono: response.data.telefono,
          ciudadResidencia: response.data.ciudadResidencia,
          fechaNacimiento: response.data.fechaNacimiento,
          
          // Estadísticas y puntos
          puntos: response.data.puntos,
          puntosUtilizados: response.data.puntosUtilizados,
          totalVisitas: response.data.totalVisitas,
          totalCompras: response.data.totalCompras,
          gastoPromedio: response.data.gastoPromedio,
          gastoTotal: response.data.gastoTotal,
          visitasPorMes: response.data.visitasPorMes,
          
          // Preferencias
          combustiblePreferido: response.data.combustiblePreferido,
          intereses: response.data.intereses,
          ubicacionPreferida: response.data.ubicacionPreferida,
          
          // Estado de cuenta
          esSubscriptorPremium: response.data.esSubscriptorPremium,
          verificadoVEAI: response.data.verificadoVEAI,
          bloqueado: response.data.bloqueado,
          intentosFallidos: response.data.intentosFallidos,
          
          // Segmentación
          ultimaCategoriaComprada: response.data.ultimaCategoriaComprada,
          segmentoClientes: response.data.segmentoClientes,
          segmentoDinamicoId: response.data.segmentoDinamicoId,
          
          // Fechas
          ultimaVisita: response.data.ultimaVisita,
          fechaCreacion: response.data.fechaCreacion,
          fechaModificacion: response.data.fechaModificacion
        };
      } catch (endpointError) {
        
        // Fallback: Obtener todos los usuarios y filtrar por email
        const allUsersResponse = await apiClient.get('usuario', config);
        const currentUser = allUsersResponse.data.find(
          (u) => u.email.toLowerCase() === user.email.toLowerCase()
        );
        
        if (!currentUser) {
          throw new Error("Usuario no encontrado en la base de datos");
        }

        return {
          // Información básica
          id: currentUser.id,
          tenantId: currentUser.tenantId,
          nombre: currentUser.nombre,
          apellido: currentUser.apellido,
          email: currentUser.email,
          ci: currentUser.ci,
          rol: currentUser.rol,
          telefono: currentUser.telefono,
          ciudadResidencia: currentUser.ciudadResidencia,
          fechaNacimiento: currentUser.fechaNacimiento,
          
          // Estadísticas y puntos
          puntos: currentUser.puntos,
          puntosUtilizados: currentUser.puntosUtilizados,
          totalVisitas: currentUser.totalVisitas,
          totalCompras: currentUser.totalCompras,
          gastoPromedio: currentUser.gastoPromedio,
          gastoTotal: currentUser.gastoTotal,
          visitasPorMes: currentUser.visitasPorMes,
          
          // Preferencias
          combustiblePreferido: currentUser.combustiblePreferido,
          intereses: currentUser.intereses,
          ubicacionPreferida: currentUser.ubicacionPreferida,
          
          // Estado de cuenta
          esSubscriptorPremium: currentUser.esSubscriptorPremium,
          verificadoVEAI: currentUser.verificadoVEAI,
          bloqueado: currentUser.bloqueado,
          intentosFallidos: currentUser.intentosFallidos,
          
          // Segmentación
          ultimaCategoriaComprada: currentUser.ultimaCategoriaComprada,
          segmentoClientes: currentUser.segmentoClientes,
          segmentoDinamicoId: currentUser.segmentoDinamicoId,
          
          // Fechas
          ultimaVisita: currentUser.ultimaVisita,
          fechaCreacion: currentUser.fechaCreacion,
          fechaModificacion: currentUser.fechaModificacion
        };
      }

    } catch (error) {
      
      // Si el backend falla, proporcionar información clara
      if (error.response?.status === 404) {
        throw new Error("Usuario no encontrado en la base de datos");
      } else if (error.response?.status === 500) {
        throw new Error("Error del servidor al obtener el perfil");
      } else {
        throw new Error(error.message || "Error al obtener el perfil");
      }
    }
  },

  // Actualizar perfil del usuario actual
// Reemplaza el método updateUserProfile en apiService.js

// Actualizar perfil del usuario actual
updateUserProfile: async (profileData) => {
  try {
    // Primero obtener el perfil actual para conseguir el ID
    const currentProfile = await apiService.getUserProfile();
    

    // Construir objeto de actualización solo con campos presentes
    const updateData = {};
    
    // Campos básicos
    if (profileData.nombre !== undefined) updateData.nombre = profileData.nombre;
    if (profileData.apellido !== undefined) updateData.apellido = profileData.apellido || null;
    if (profileData.email !== undefined) updateData.email = profileData.email;
    if (profileData.ciudadResidencia !== undefined) updateData.ciudadResidencia = profileData.ciudadResidencia || null;
    if (profileData.combustiblePreferido !== undefined) updateData.combustiblePreferido = profileData.combustiblePreferido || "";
    
    // Teléfono - manejar conversión
    if (profileData.telefono !== undefined) {
      const telefonoNum = parseInt(profileData.telefono);
      updateData.telefono = isNaN(telefonoNum) ? 0 : telefonoNum;
    }
    
    // Fecha de nacimiento - solo si se proporciona
    if (profileData.fechaNacimiento) {
      updateData.fechaNacimiento = profileData.fechaNacimiento;
    } else {
      // Mantener valor existente o usar default
      updateData.fechaNacimiento = currentProfile.fechaNacimiento || "0001-01-01T00:00:00";
    }
    
    // Intereses - solo si se proporciona
    if (profileData.intereses !== undefined) {
      updateData.intereses = profileData.intereses;
    }
    
    // Mantener campos que no se editan
    updateData.puntos = currentProfile.puntos || 0;
    updateData.tenantId = currentProfile.tenantId;
    
    // Solo incluir password si se proporciona
    if (profileData.password) {
      updateData.password = profileData.password;
    }


    const response = await apiClient.put(`usuario/${currentProfile.id}`, updateData);
    
    return response.data;

  } catch (error) {
    
    // Manejar errores específicos
    if (error.response?.status === 400) {
      const errorMessage = error.response.data?.message || 
                          error.response.data?.errors?.[0] || 
                          "Datos inválidos";
      throw new Error(errorMessage);
    } else if (error.response?.status === 404) {
      throw new Error("Usuario no encontrado");
    } else if (error.response?.status === 401) {
      throw new Error("No autorizado para actualizar este perfil");
    } else {
      throw new Error(error.message || "Error al actualizar el perfil");
    }
  }
},

  // ==========================================
  // MÉTODOS DE TENANT
  // ==========================================

  // Obtener información del tenant del usuario actual
  getTenantInfo: async () => {
    try {
      // Primero obtener el perfil del usuario para conseguir su tenantId
      const userProfile = await apiService.getUserProfile();
      
      if (!userProfile.tenantId) {
        throw new Error("Usuario no tiene tenant asociado");
      }


      // Obtener información del tenant
      const response = await apiClient.get(`tenant/${userProfile.tenantId}`);
      
      return response.data;

    } catch (error) {
      
      if (error.response?.status === 404) {
        throw new Error("Tenant no encontrado");
      } else if (error.response?.status === 500) {
        throw new Error("Error del servidor al obtener el tenant");
      } else {
        throw new Error(error.message || "Error al obtener información del tenant");
      }
    }
  },

  // ==========================================
  // MÉTODOS DE UBICACIONES
  // ==========================================
  // Estructura real de la entidad Ubicacion según la respuesta del API:
  // {
  //   "id": "guid",
  //   "tenantId": "guid", 
  //   "tenant": null,
  //   "nombre": "string",
  //   "direccion": "string",
  //   "ciudad": "string",
  //   "departamento": "string", 
  //   "telefono": "string|null",
  //   "latitud": number,
  //   "longitud": number,
  //   "fechaCreacion": "datetime",
  //   "fechaModificacion": "datetime",
  //   "horaApertura": "time", // formato "HH:mm:ss"
  //   "horaCierre": "time",   // formato "HH:mm:ss"
  //   "lavadoDeAuto": boolean,
  //   "lavado": boolean,
  //   "cambioDeAceite": boolean,
  //   "cambioDeNeumaticos": boolean,
  //   "precioNaftaSuper": decimal,
  //   "precioNaftaPremium": decimal,
  //   "precioDiesel": decimal,
  //   "productosLocales": array,
  //   "promociones": array
  // }

  // Obtener ubicaciones del tenant del usuario actual
  getUbicacionesByUserTenant: async () => {
    try {
      // Primero obtener el perfil del usuario para conseguir su tenantId
      const userProfile = await apiService.getUserProfile();
      
      if (!userProfile.tenantId) {
        throw new Error("Usuario no tiene tenant asociado");
      }


      // Obtener ubicaciones del tenant usando tu endpoint existente
      const response = await apiClient.get(`ubicacion/tenant/${userProfile.tenantId}`);
      
      return response.data;

    } catch (error) {
      
      if (error.response?.status === 404) {
        throw new Error("No se encontraron ubicaciones para este tenant");
      } else if (error.response?.status === 500) {
        throw new Error("Error del servidor al obtener las ubicaciones");
      } else {
        throw new Error(error.message || "Error al obtener las ubicaciones");
      }
    }
  },

  
  // Obtener todas las ubicaciones
  getAllUbicaciones: async () => {
    try {
      const response = await apiClient.get('ubicacion');
      return response.data;
    } catch (error) {
      throw new Error(error.response?.data?.message || "Error al obtener las ubicaciones");
    }
  },

  // Obtener una ubicación específica
  getUbicacion: async (id) => {
    try {
      const response = await apiClient.get(`ubicacion/${id}`);
      return response.data;
    } catch (error) {
      throw new Error(error.response?.data?.message || "Error al obtener la ubicación");
    }
  },

  // ==========================================
// MÉTODOS DE PRODUCTOS (agregar a tu apiService.js existente)
// ==========================================

// Estructura de ProductoCanjeable según la API:
// {
//   "id": "guid",
//   "nombre": "string",
//   "descripcion": "string|null", 
//   "costoEnPuntos": int,
//   "disponibilidadesPorUbicacion": array
// }

// Estructura de ProductoUbicacion según la API:
// {
//   "id": "guid",
//   "ubicacionId": "guid",
//   "ubicacion": object|null,
//   "productoCanjeableId": "guid", 
//   "productoCanjeable": object,
//   "stockDisponible": int,
//   "activo": boolean
// }

// Obtener todos los productos canjeables disponibles
getProductosCanjeables: async () => {
  try {

    const response = await apiClient.get('ProductoCanjeable');

    return response.data;

  } catch (error) {
    
    if (error.response?.status === 404) {
      throw new Error("No se encontraron productos canjeables");
    } else if (error.response?.status === 500) {
      throw new Error("Error del servidor al obtener productos canjeables");
    } else {
      throw new Error(error.message || "Error al obtener productos canjeables");
    }
  }
},

// Obtener un producto canjeable por ID
getProductoCanjeable: async (id) => {
  try {
    if (!id) throw new Error('ID requerido');
    const response = await apiClient.get(`ProductoCanjeable/${id}`);
    return response.data;
  } catch (error) {
    throw new Error(error.response?.data?.message || 'Error al obtener el producto');
  }
},

// Obtener productos disponibles en una ubicación específica
getProductosByUbicacion: async (ubicacionId, categoria) => {
  try {
    if (!ubicacionId) {
      throw new Error("ID de ubicación es requerido");
    }

    
    const url = categoria ? `ProductoUbicacion/ubicacion/${ubicacionId}?categoria=${encodeURIComponent(categoria)}` : `ProductoUbicacion/ubicacion/${ubicacionId}`;
    const response = await apiClient.get(url);
    
    return response.data;
    
  } catch (error) {
    
    if (error.response?.status === 404) {
      throw new Error("No se encontraron productos para esta ubicación");
    } else if (error.response?.status === 500) {
      throw new Error("Error del servidor al obtener productos de la ubicación");
    } else {
      throw new Error(error.message || "Error al obtener productos de la ubicación");
    }
  }
},

// Obtener todos los productos con información de ubicaciones
  getAllProductosUbicacion: async () => {
    try {
    
    const response = await apiClient.get('ProductoUbicacion');
    
    return response.data;
    
  } catch (error) {
    
    if (error.response?.status === 404) {
      throw new Error("No se encontraron productos en ubicaciones");
    } else if (error.response?.status === 500) {
      throw new Error("Error del servidor al obtener productos de ubicaciones");
    } else {
      throw new Error(error.message || "Error al obtener productos de ubicaciones");
    }
  }
},

// Generar un canje para un producto y obtener el código QR
  generarCanje: async (productoId, ubicacionId) => {
  try {
    if (!productoId || !ubicacionId) {
      throw new Error("Producto y ubicación son requeridos para el canje");
    }

    const user = await apiService.getUserProfile();

    const mensaje = {
      tipoMensaje: 2, // TipoMensajeNAFTA.Canje
      ubicacionId: ubicacionId,
      tenantId: user.tenantId,
      datos: {
        productoCanjeableId: productoId,
        usuarioId: user.id
      }
    };

    const response = await apiClient.post('nafta/generar-canje', mensaje);
    return response.data;
  } catch (error) {
    throw new Error(error.response?.data?.mensaje || 'Error al generar el canje');
  }
},


  // ==========================================
  // MÉTODOS HTTP GENÉRICOS
  // ==========================================

  // Métodos HTTP genéricos
  get: async (endpoint) => {
    try {
      const response = await apiClient.get(endpoint);
      return response.data;
    } catch (error) {
      throw error;
    }
  },

  // Procesa la compra de un producto utilizando PayPal
  procesarTransaccion: async (
    productoUbicacion,
    ubicacionId,
    puntosUtilizados = 0,
    valorPunto = 0,
    tipoTransaccion = 2
  ) => {
    try {
      if (!productoUbicacion || !ubicacionId) {
        throw new Error("Producto y ubicación son requeridos");
      }

      const user = await apiService.getUserProfile();

      const linea = {
        idProducto: productoUbicacion.productoCanjeable.id,
        nombreProducto: productoUbicacion.productoCanjeable.nombre,
        categoria: productoUbicacion.categoria || "general",
        cantidad: 1,
        precioUnitario: Math.round(productoUbicacion.precio),
        subTotal: Math.round(productoUbicacion.precio)
      };

      const total = Math.round(productoUbicacion.precio);
      const montoPayPal = Math.max(
        0,
        Math.round(total - puntosUtilizados * valorPunto)
      );
      const transaccion = {
        IdentificadorUsuario: user.id,
        fechaTransaccion: new Date().toISOString(),
        tipoTransaccion,
        monto: total,
        metodoPago: 1,
        MontoPayPal: montoPayPal,
        productos: [linea],
        puntosUtilizados,
        datosAdicionales: {}
      };
      console.log(transaccion.MontoPayPal);
      const mensaje = {
        tipoMensaje: 1,
        ubicacionId: ubicacionId,
        tenantId: user.tenantId,
        datos: { transaccion }
      };

      const response = await apiClient.post('nafta/transaccion', mensaje);
      return response.data;
    } catch (error) {
      throw new Error(error.response?.data?.mensaje || 'Error al procesar transacción');
    }
  },

  // Procesa la compra de varios productos (carrito) utilizando PayPal
  procesarTransaccionMultiple: async (productosUbicacion, ubicacionId) => {
    try {
      if (!Array.isArray(productosUbicacion) || productosUbicacion.length === 0 || !ubicacionId) {
        throw new Error("Productos y ubicación son requeridos");
      }

      const user = await apiService.getUserProfile();

      const lineas = productosUbicacion.map(p => ({
        idProducto: p.productoCanjeable.id,
        nombreProducto: p.productoCanjeable.nombre,
        categoria: p.categoria || "general",
        cantidad: 1,
        precioUnitario: Math.round(p.precio),
        subTotal: Math.round(p.precio)
      }));

      const total = lineas.reduce((sum, l) => sum + l.subTotal, 0);

      const transaccion = {
        IdentificadorUsuario: user.id,
        fechaTransaccion: new Date().toISOString(),
        tipoTransaccion: 2,
        monto: total,
        metodoPago: 1,
        MontoPayPal: total,
        productos: lineas,
        puntosUtilizados: 0,
        datosAdicionales: {}
      };

      const mensaje = {
        tipoMensaje: 1,
        ubicacionId: ubicacionId,
        tenantId: user.tenantId,
        datos: { transaccion }
      };

      const response = await apiClient.post('nafta/transaccion', mensaje);
      return response.data;
    } catch (error) {
      throw new Error(error.response?.data?.mensaje || 'Error al procesar transacción');
    }
  },

   // Procesa la compra de varios productos utilizando combinación de dinero y puntos
   procesarTransaccionMultipleMixto: async (
    productosUbicacion,
    ubicacionId,
    puntosUtilizados = 0,
    valorPunto = 0
  ) => {
    try {
      if (!Array.isArray(productosUbicacion) || productosUbicacion.length === 0 || !ubicacionId) {
        throw new Error("Productos y ubicación son requeridos");
      }

      const user = await apiService.getUserProfile();

      const lineas = productosUbicacion.map(p => ({
        idProducto: p.productoCanjeable.id,
        nombreProducto: p.productoCanjeable.nombre,
        categoria: p.categoria || "general",
        cantidad: 1,
        precioUnitario: Math.round(p.precio),
        subTotal: Math.round(p.precio)
      }));

      const total = lineas.reduce((sum, l) => sum + l.subTotal, 0);
      const montoPayPal = Math.max(0, Math.round(total - puntosUtilizados * valorPunto));

      const transaccion = {
        IdentificadorUsuario: user.id,
        fechaTransaccion: new Date().toISOString(),
        tipoTransaccion: 2,
        monto: total,
        metodoPago: 1,
        MontoPayPal: montoPayPal,
        productos: lineas,
        puntosUtilizados,
        datosAdicionales: {}
      };

      const mensaje = {
        tipoMensaje: 1,
        ubicacionId: ubicacionId,
        tenantId: user.tenantId,
        datos: { transaccion }
      };

      const response = await apiClient.post('nafta/transaccion', mensaje);
      return response.data;
    } catch (error) {
      throw new Error(error.response?.data?.mensaje || 'Error al procesar transacción');
    }
  },
  // Confirma un pago de PayPal con los datos devueltos por la aprobación
  confirmarPagoPaypal: async (paymentId, payerId) => {
    try {
      const mensaje = {
        tipoMensaje: 1,
        datos: { paymentId, payerId }
      };

      const response = await apiClient.post('nafta/confirmar-paypal', mensaje);
      return response.data;
    } catch (error) {
      throw new Error(error.response?.data?.mensaje || 'Error al confirmar pago');
    }
  },


  // Obtener historial de canjes del usuario
  getCanjesByUsuario: async (usuarioId, cursor = null, limit = 10) => {
    try {
      const params = { limit };
      if (cursor) params.cursor = cursor;
      const response = await apiClient.get(`canje/usuario/${usuarioId}`, { params });
      return response.data;
    } catch (error) {
      throw new Error(error.response?.data?.message || 'Error al obtener el historial de canjes');
    }
  },

  getTransaccionesByUsuario: async (cursor = null, limit = 10) => {
    try {
      const params = { limit };
      if (cursor) params.cursor = cursor;
      console.log('Llamando a /usuario/historial-transacciones', params); // Added log
      const response = await apiClient.get(`usuario/historial-transacciones`, { params });
      console.log('Respuesta de historial-transacciones', response.data); // Added log
      return response.data;
    } catch (error) {
      throw new Error(error.response?.data?.message || 'Error al obtener el historial de transacciones');
    }
  },
  getTransaccionById: async (id) => {
    try {
      const response = await apiClient.get(`usuario/transaccion/${id}`);
      return response.data;
    } catch (error) {
      throw new Error(error.response?.data?.message || 'Error al obtener la transacci\u00f3n');
    }
  },

  getPromociones: async () => {
    try {
      const response = await apiClient.get('Promocion');
      console.log('Respuesta de Promocion', response.data); // Added log
      return response.data;
    } catch (error) {
      console.error('Error en getPromociones', error); // Added log
      throw new Error(error.response?.data?.message || 'Error al obtener las promociones');
    }
  },

  getPromocionesByUserTenant: async () => {
    try {
      const user = await apiService.getUserProfile();
      if (!user.tenantId) {
        throw new Error('Usuario no tiene tenant asociado');
      }
      const response = await apiClient.get(`Promocion/tenant/${user.tenantId}`);
      return response.data;
    } catch (error) {
      console.error('Error en getPromocionesByUserTenant', error);
      throw new Error(error.response?.data?.message || 'Error al obtener las promociones');
    }
  },

  generarCanjes: async (productoIds, ubicacionId) => {
    try {
      if (!Array.isArray(productoIds) || productoIds.length === 0 || !ubicacionId) {
        throw new Error("Productos y ubicación son requeridos");
      }

      const user = await apiService.getUserProfile();

      const mensaje = {
        tipoMensaje: 2,
        ubicacionId: ubicacionId,
        tenantId: user.tenantId,
        datos: {
          productoIds: productoIds,
          usuarioId: user.id
        }
      };

      const response = await apiClient.post('nafta/generar-canjes', mensaje);
      return response.data;
    } catch (error) {
      throw new Error(error.response?.data?.mensaje || 'Error al generar canjes');
    }
  },

  getMisNotificaciones: async (config = {}) => {
    try {
      const response = await apiClient.get('notificacion/mine', config);
      return response.data;
    } catch (error) {
      throw new Error(
        error.response?.data?.message || 'Error al obtener las notificaciones'
      );
    }
  },

  crearNotificacion: async (titulo, mensaje, audienciaId) => {
    try {
      const response = await apiClient.post('notificacion', { titulo, mensaje, audienciaId });
      return response.data;
    } catch (error) {
      throw new Error(error.response?.data?.message || 'Error al crear la notificacion');
    }
  },

  marcarNotificacionLeida: async (id) => {
    try {
      await apiClient.put(`notificacion/leida/${id}`);
    } catch (error) {
      throw new Error(error.response?.data?.message || 'Error al marcar la notificacion');
    }
  },

  borrarNotificacion: async (id) => {
    try {
      await apiClient.delete(`notificacion/${id}`);
    } catch (error) {
      throw new Error(error.response?.data?.message || 'Error al borrar la notificacion');
    }
  },

  borrarTodasMisNotificaciones: async () => {
    try {
      await apiClient.delete('notificacion/mine');
    } catch (error) {
      throw new Error(error.response?.data?.message || 'Error al borrar las notificaciones');
    }
  },

  // Verificar la edad de un usuario usando el servicio mock del backend
  verifyAge: async (cedula) => {
    try {
      const response = await apiClient.get('verify/age_verify', {
        params: { cedula },
        skipAuthError: true
      });
      return response.data; // { isAllowed, edad }
    } catch (error) {
      throw new Error(error.response?.data?.message || 'Error al verificar la edad');
    }
  },
  
  post: async (endpoint, data) => {
    try {
      const response = await apiClient.post(endpoint, data);
      return response.data;
    } catch (error) {
      throw error;
    }
  },

  put: async (endpoint, data) => {
    try {
      const response = await apiClient.put(endpoint, data);
      return response.data;
    } catch (error) {
      throw error;
    }
  },

  delete: async (endpoint) => {
    try {
      const response = await apiClient.delete(endpoint);
      return response.data;
    } catch (error) {
      throw error;
    }
  },
};

export default apiService;