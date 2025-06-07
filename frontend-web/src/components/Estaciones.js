import React, { useState, useEffect } from "react";
import authService from "../services/authService";
import apiService from "../services/apiService";
import CatalogoProductos from "./CatalogoProductos";

const Estaciones = () => {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [ubicaciones, setUbicaciones] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [userProfile, setUserProfile] = useState(null);
  const [tenantInfo, setTenantInfo] = useState(null);
  
  // Estados para el modal de productos
  const [modalAbierto, setModalAbierto] = useState(false);
  const [ubicacionSeleccionada, setUbicacionSeleccionada] = useState(null);

  useEffect(() => {
    const checkAuth = () => {
      const authenticated = authService.isAuthenticated();
      setIsAuthenticated(authenticated);
      
      // Si está autenticado, cargar ubicaciones
      if (authenticated) {
        loadUbicaciones();
      }
    };

    checkAuth();
  }, []);

  const loadUbicaciones = async () => {
    setLoading(true);
    setError("");
    
    try {
      console.log("Cargando ubicaciones del tenant del usuario...");
      
      // Primero obtener el perfil del usuario
      const profile = await apiService.getUserProfile();
      setUserProfile(profile);
      
      // Obtener información del tenant
      const tenant = await apiService.getTenantInfo();
      setTenantInfo(tenant);
      
      // Luego obtener las ubicaciones de su tenant
      const ubicacionesData = await apiService.getUbicacionesByUserTenant();
      setUbicaciones(ubicacionesData);
      
      console.log("Ubicaciones cargadas:", ubicacionesData);
      console.log("Información del tenant:", tenant);
      
    } catch (err) {
      console.error("Error al cargar ubicaciones:", err);
      setError(err.message);
      
      // Como fallback, mostrar mensaje sin ubicaciones
      setUbicaciones([]);
    } finally {
      setLoading(false);
    }
  };

  // Función para formatear precios
  const formatPrice = (price) => {
    if (!price || price === 0) return "No disponible";
    return `${price.toFixed(2)}`;
  };

  // Función para abrir el modal de productos
  const abrirCatalogoProductos = (ubicacion) => {
    setUbicacionSeleccionada(ubicacion);
    setModalAbierto(true);
  };

  // Función para cerrar el modal de productos
  const cerrarCatalogoProductos = () => {
    setModalAbierto(false);
    setUbicacionSeleccionada(null);
  };

  return (
    <div
      style={{
        maxWidth: "1200px",
        margin: "0 auto",
        padding: "1rem",
      }}
    >
      {/* Header */}
      <div style={{ textAlign: "center", marginBottom: "2rem" }}>
        <h1 style={{ color: "#7B3F00", fontSize: "2.5rem", marginBottom: "0.5rem" }}>
          Estaciones de Servicio
        </h1>
        <p style={{ fontSize: "1.2rem", color: "#6c757d" }}>
          Listado completo de nuestras estaciones con información detallada
        </p>
      </div>

      {/* Contenido principal */}
      <div
        style={{
          backgroundColor: "#f8f9fa",
          padding: "2rem",
          borderRadius: "12px",
          marginBottom: "2rem",
        }}
      >
        {isAuthenticated ? (
          <>
            {loading && (
              <div style={{ textAlign: "center", padding: "3rem" }}>
                <div style={{
                  border: "4px solid #f3f3f3",
                  borderTop: "4px solid #7B3F00",
                  borderRadius: "50%",
                  width: "60px",
                  height: "60px",
                  animation: "spin 1s linear infinite",
                  margin: "0 auto 1rem"
                }} />
                <p style={{ color: "#7B3F00", fontSize: "1.1rem" }}>Cargando estaciones...</p>
                
                <style>{`
                  @keyframes spin {
                    0% { transform: rotate(0deg); }
                    100% { transform: rotate(360deg); }
                  }
                `}</style>
              </div>
            )}

            {error && (
              <div style={{
                backgroundColor: "#f8d7da",
                color: "#721c24",
                padding: "1.5rem",
                borderRadius: "8px",
                marginBottom: "1rem",
                border: "1px solid #f5c6cb"
              }}>
                <strong>⚠️ Error:</strong> {error}
              </div>
            )}

            {!loading && !error && ubicaciones.length === 0 && (
              <div style={{
                backgroundColor: "#fff3cd",
                color: "#856404",
                padding: "3rem",
                borderRadius: "12px",
                textAlign: "center",
                border: "1px solid #ffeaa7"
              }}>
                <h4 style={{ margin: "0 0 1rem 0", fontSize: "1.25rem" }}>
                  📋 Sin estaciones disponibles
                </h4>
                <p style={{ margin: "0", fontSize: "1rem" }}>
                  No se encontraron estaciones de servicio para tu red en este momento.
                </p>
              </div>
            )}

            {!loading && ubicaciones.length > 0 && (
              <>
                <div style={{
                  marginBottom: "1rem",
                  padding: "1rem",
                  backgroundColor: "#e3f2fd",
                  borderRadius: "8px",
                  border: "1px solid #bbdefb"
                }}>
                  <p style={{ 
                    margin: "0", 
                    color: "#1976d2",
                    fontWeight: "600",
                    display: "flex",
                    alignItems: "center",
                    gap: "0.5rem"
                  }}>
                    🏪 {ubicaciones.length} estación{ubicaciones.length !== 1 ? 'es' : ''} encontrada{ubicaciones.length !== 1 ? 's' : ''}
                  </p>
                </div>

                <div style={{
                  display: "grid",
                  gridTemplateColumns: "repeat(auto-fill, minmax(350px, 1fr))",
                  gap: "1.5rem"
                }}>
                  {ubicaciones.map((ubicacion) => (
                    <div
                      key={ubicacion.id}
                      style={{
                        backgroundColor: "white",
                        border: "1px solid #dee2e6",
                        borderRadius: "12px",
                        padding: "1.5rem",
                        boxShadow: "0 2px 4px rgba(0,0,0,0.1)",
                        transition: "transform 0.2s ease, box-shadow 0.2s ease"
                      }}
                      onMouseEnter={(e) => {
                        e.currentTarget.style.transform = "translateY(-2px)";
                        e.currentTarget.style.boxShadow = "0 4px 12px rgba(0,0,0,0.15)";
                      }}
                      onMouseLeave={(e) => {
                        e.currentTarget.style.transform = "translateY(0)";
                        e.currentTarget.style.boxShadow = "0 2px 4px rgba(0,0,0,0.1)";
                      }}
                    >
                      {/* Header de la ubicación */}
                      <div style={{ marginBottom: "1.25rem" }}>
                        <h4 style={{ 
                          margin: "0 0 0.5rem 0", 
                          color: "#495057",
                          fontSize: "1.25rem",
                          display: "flex",
                          alignItems: "center",
                          gap: "0.5rem"
                        }}>
                          🏪 {ubicacion.nombre}
                        </h4>
                        
                        <div style={{ display: "grid", gap: "0.25rem", fontSize: "0.9rem", color: "#6c757d" }}>
                          <p style={{ 
                            margin: "0", 
                            display: "flex",
                            alignItems: "center",
                            gap: "0.25rem"
                          }}>
                            📍 {ubicacion.direccion}
                          </p>
                          
                          <p style={{ 
                            margin: "0", 
                            display: "flex",
                            alignItems: "center",
                            gap: "0.25rem"
                          }}>
                            🌎 {ubicacion.ciudad}, {ubicacion.departamento}
                          </p>
                          
                          {ubicacion.telefono && (
                            <p style={{ 
                              margin: "0", 
                              display: "flex",
                              alignItems: "center",
                              gap: "0.25rem"
                            }}>
                              📞 {ubicacion.telefono}
                            </p>
                          )}
                        </div>
                      </div>

                      {/* Horarios de atención */}
                      {(ubicacion.horaApertura && ubicacion.horaCierre && 
                        ubicacion.horaApertura !== "00:00:00" && ubicacion.horaCierre !== "00:00:00") && (
                        <div style={{
                          marginBottom: "1.25rem",
                          padding: "0.75rem",
                          backgroundColor: "#e8f5e8",
                          borderRadius: "8px",
                          border: "1px solid #a5d6a7"
                        }}>
                          <h5 style={{ 
                            margin: "0 0 0.5rem 0", 
                            color: "#388e3c",
                            fontSize: "0.95rem",
                            display: "flex",
                            alignItems: "center",
                            gap: "0.5rem"
                          }}>
                            🕐 Horarios de Atención
                          </h5>
                          <p style={{ 
                            margin: "0", 
                            color: "#2e7d32",
                            fontWeight: "600",
                            fontSize: "0.9rem"
                          }}>
                            {ubicacion.horaApertura.slice(0, 5)} - {ubicacion.horaCierre.slice(0, 5)}
                          </p>
                        </div>
                      )}

                      {/* Precios de combustible */}
                      <div style={{
                        marginBottom: "1.25rem",
                        padding: "1rem",
                        backgroundColor: "#fff9c4",
                        borderRadius: "8px",
                        border: "1px solid #f9c74f"
                      }}>
                        <h5 style={{ 
                          margin: "0 0 0.75rem 0", 
                          color: "#d68910",
                          fontSize: "1rem",
                          display: "flex",
                          alignItems: "center",
                          gap: "0.5rem"
                        }}>
                          ⛽ Precios de Combustible
                        </h5>
                        <div style={{ display: "grid", gap: "0.5rem", fontSize: "0.9rem" }}>
                          <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center" }}>
                            <span style={{ color: "#7B3F00", fontWeight: "500" }}>Nafta Súper:</span>
                            <span style={{ fontWeight: "bold", color: "#d68910" }}>
                              ${formatPrice(ubicacion.precioNaftaSuper)}
                            </span>
                          </div>
                          <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center" }}>
                            <span style={{ color: "#7B3F00", fontWeight: "500" }}>Nafta Premium:</span>
                            <span style={{ fontWeight: "bold", color: "#d68910" }}>
                              ${formatPrice(ubicacion.precioNaftaPremium)}
                            </span>
                          </div>
                          <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center" }}>
                            <span style={{ color: "#7B3F00", fontWeight: "500" }}>Diesel:</span>
                            <span style={{ fontWeight: "bold", color: "#d68910" }}>
                              ${formatPrice(ubicacion.precioDiesel)}
                            </span>
                          </div>
                        </div>
                      </div>

                      {/* Botón para ver catálogo de productos */}
                      <div style={{ marginBottom: "1.25rem" }}>
                        <button
                          onClick={() => abrirCatalogoProductos(ubicacion)}
                          style={{
                            width: "100%",
                            backgroundColor: "#007bff",
                            color: "white",
                            border: "none",
                            borderRadius: "8px",
                            padding: "0.75rem 1rem",
                            fontSize: "1rem",
                            fontWeight: "500",
                            cursor: "pointer",
                            transition: "background-color 0.2s ease",
                            display: "flex",
                            alignItems: "center",
                            justifyContent: "center",
                            gap: "0.5rem"
                          }}
                          onMouseEnter={(e) => e.target.style.backgroundColor = "#0056b3"}
                          onMouseLeave={(e) => e.target.style.backgroundColor = "#007bff"}
                        >
                          🛒 Ver Catálogo de Productos
                        </button>
                      </div>

                      {/* Servicios adicionales */}
                      {(ubicacion.lavadoDeAuto || ubicacion.lavado || ubicacion.cambioDeAceite || ubicacion.cambioDeNeumaticos) && (
                        <div style={{
                          marginBottom: "1.25rem",
                          padding: "0.75rem",
                          backgroundColor: "#e8f5e8",
                          borderRadius: "8px",
                          border: "1px solid #a5d6a7"
                        }}>
                          <h5 style={{ 
                            margin: "0 0 0.5rem 0", 
                            color: "#388e3c",
                            fontSize: "0.95rem",
                            display: "flex",
                            alignItems: "center",
                            gap: "0.5rem"
                          }}>
                            🔧 Servicios Adicionales
                          </h5>
                          <div style={{ 
                            display: "flex", 
                            flexWrap: "wrap", 
                            gap: "0.5rem",
                            fontSize: "0.8rem"
                          }}>
                            {ubicacion.lavadoDeAuto && (
                              <span style={{
                                backgroundColor: "#4caf50",
                                color: "white",
                                padding: "0.25rem 0.5rem",
                                borderRadius: "12px",
                                fontSize: "0.75rem"
                              }}>
                                🚗 Lavado de Auto
                              </span>
                            )}
                            {ubicacion.lavado && (
                              <span style={{
                                backgroundColor: "#2196f3",
                                color: "white",
                                padding: "0.25rem 0.5rem",
                                borderRadius: "12px",
                                fontSize: "0.75rem"
                              }}>
                                🧽 Lavado
                              </span>
                            )}
                            {ubicacion.cambioDeAceite && (
                              <span style={{
                                backgroundColor: "#ff9800",
                                color: "white",
                                padding: "0.25rem 0.5rem",
                                borderRadius: "12px",
                                fontSize: "0.75rem"
                              }}>
                                🛢️ Cambio de Aceite
                              </span>
                            )}
                            {ubicacion.cambioDeNeumaticos && (
                              <span style={{
                                backgroundColor: "#9c27b0",
                                color: "white",
                                padding: "0.25rem 0.5rem",
                                borderRadius: "12px",
                                fontSize: "0.75rem"
                              }}>
                                🛞 Cambio de Neumáticos
                              </span>
                            )}
                          </div>
                        </div>
                      )}

                      {/* Promociones */}
                      {ubicacion.promociones && ubicacion.promociones.length > 0 && (
                        <div style={{
                          marginBottom: "1.25rem",
                          padding: "0.75rem",
                          backgroundColor: "#fce4ec",
                          borderRadius: "8px",
                          fontSize: "0.85rem",
                          border: "1px solid #f8bbd9"
                        }}>
                          <div style={{ 
                            color: "#e91e63", 
                            fontWeight: "600",
                            display: "flex",
                            alignItems: "center",
                            gap: "0.5rem"
                          }}>
                            🎉 Promociones Activas ({ubicacion.promociones.length})
                          </div>
                        </div>
                      )}

                      {/* Productos Locales */}
                      {ubicacion.productosLocales && ubicacion.productosLocales.length > 0 && (
                        <div style={{
                          marginBottom: "1.25rem",
                          padding: "0.75rem",
                          backgroundColor: "#e3f2fd",
                          borderRadius: "8px",
                          fontSize: "0.85rem",
                          border: "1px solid #bbdefb"
                        }}>
                          <div style={{ 
                            color: "#1976d2", 
                            fontWeight: "600",
                            display: "flex",
                            alignItems: "center",
                            gap: "0.5rem"
                          }}>
                            🛒 Productos Locales Disponibles ({ubicacion.productosLocales.length})
                          </div>
                        </div>
                      )}

                      {/* Información del sistema */}
                      <div style={{
                        padding: "0.75rem",
                        backgroundColor: "#f8f9fa",
                        borderRadius: "8px",
                        fontSize: "0.8rem",
                        color: "#6c757d",
                        border: "1px solid #dee2e6"
                      }}>
                        <div style={{ 
                          display: "flex", 
                          justifyContent: "space-between",
                          alignItems: "center",
                          flexWrap: "wrap",
                          gap: "0.5rem"
                        }}>
                          <span>🆔 ID: {ubicacion.id.slice(0, 8)}...</span>
                          <span>🏢 Tenant: {ubicacion.tenantId.slice(0, 8)}...</span>
                        </div>
                      </div>
                    </div>
                  ))}
                </div>
              </>
            )}
          </>
        ) : (
          <div style={{ textAlign: "center", padding: "3rem" }}>
            <h2 style={{ color: "#495057", marginBottom: "1rem" }}>
              Acceso Requerido
            </h2>
            <p style={{ marginBottom: "1.5rem", fontSize: "1.1rem", color: "#6c757d" }}>
              Necesitas iniciar sesión para ver las estaciones de servicio de tu red.
            </p>
            <div style={{
              display: "inline-block",
              backgroundColor: "#007bff",
              color: "white",
              padding: "0.75rem 1.5rem",
              borderRadius: "6px",
              textDecoration: "none",
              fontSize: "1rem",
              fontWeight: "500"
            }}>
              Iniciar Sesión
            </div>
          </div>
        )}
      </div>

      {/* Modal de catálogo de productos */}
      <CatalogoProductos
        ubicacion={ubicacionSeleccionada}
        isOpen={modalAbierto}
        onClose={cerrarCatalogoProductos}
      />
    </div>
  );
};

export default Estaciones;