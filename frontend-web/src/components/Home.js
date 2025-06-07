import React, { useState, useEffect } from "react";
import { Link } from "react-router-dom";
import authService from "../services/authService";
import apiService from "../services/apiService";
import CatalogoProductos from "./CatalogoProductos";

const Home = () => {
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
      
      // Si está autenticado, cargar ubicaciones para mostrar precios
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
      console.log("Cargando ubicaciones para mostrar precios...");
      
      // Primero obtener el perfil del usuario
      const profile = await apiService.getUserProfile();
      setUserProfile(profile);
      
      // Obtener información del tenant
      const tenant = await apiService.getTenantInfo();
      setTenantInfo(tenant);
      
      // Luego obtener las ubicaciones de su tenant
      const ubicacionesData = await apiService.getUbicacionesByUserTenant();
      setUbicaciones(ubicacionesData);
      
      console.log("Ubicaciones cargadas para precios:", ubicacionesData);
      
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
    if (!price || price === 0) return "N/D";
    return `$${price.toFixed(2)}`;
  };

  // Función para determinar el color del precio (puedes personalizar la lógica)
  const getPriceColor = (price, type) => {
    if (!price || price === 0) return "#6c757d";
    
    // Colores diferentes para cada tipo de combustible
    switch (type) {
      case "super": return "#28a745";      // Verde para Súper
      case "premium": return "#007bff";    // Azul para Premium  
      case "diesel": return "#fd7e14";     // Naranja para Diesel
      default: return "#495057";
    }
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
          Servipuntos
        </h1>
        <p style={{ fontSize: "1.2rem", color: "#6c757d" }}>
          Precios actualizados de combustible en nuestras estaciones
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
                <p style={{ color: "#7B3F00", fontSize: "1.1rem" }}>Cargando precios...</p>
                
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
                  📋 Sin precios disponibles
                </h4>
                <p style={{ margin: "0", fontSize: "1rem" }}>
                  No se encontraron estaciones de servicio para mostrar precios en este momento.
                </p>
              </div>
            )}

            {!loading && ubicaciones.length > 0 && (
              <>
                {/* Información resumida */}
                <div style={{
                  marginBottom: "2rem",
                  padding: "1.5rem",
                  backgroundColor: "#e3f2fd",
                  borderRadius: "12px",
                  border: "1px solid #bbdefb",
                  textAlign: "center"
                }}>
                  <h3 style={{ 
                    margin: "0 0 0.5rem 0", 
                    color: "#1976d2",
                    fontSize: "1.5rem"
                  }}>
                    💰 Precios de Combustible
                  </h3>
                  <p style={{ 
                    margin: "0 0 1rem 0", 
                    color: "#1565c0",
                    fontSize: "1.1rem"
                  }}>
                    {ubicaciones.length} estación{ubicaciones.length !== 1 ? 'es' : ''} disponible{ubicaciones.length !== 1 ? 's' : ''} • Precios actualizados
                  </p>
                  
                  {/* Enlaces para navegación */}
                  <div style={{ 
                    display: "flex", 
                    justifyContent: "center", 
                    gap: "1rem",
                    flexWrap: "wrap"
                  }}>
                    <Link 
                      to="/estaciones"
                      style={{
                        display: "inline-block",
                        backgroundColor: "#007bff",
                        color: "white",
                        padding: "0.75rem 1.5rem",
                        borderRadius: "8px",
                        textDecoration: "none",
                        fontSize: "1rem",
                        fontWeight: "500",
                        transition: "background-color 0.2s ease"
                      }}
                      onMouseEnter={(e) => e.target.style.backgroundColor = "#0056b3"}
                      onMouseLeave={(e) => e.target.style.backgroundColor = "#007bff"}
                    >
                      🏪 Ver información completa de las estaciones
                    </Link>
                  </div>
                </div>

                {/* Grid de precios por estación */}
                <div style={{
                  display: "grid",
                  gridTemplateColumns: "repeat(auto-fill, minmax(320px, 1fr))",
                  gap: "1.5rem"
                }}>
                  {ubicaciones.map((ubicacion) => (
                    <div
                      key={ubicacion.id}
                      style={{
                        backgroundColor: "white",
                        border: "2px solid #e9ecef",
                        borderRadius: "16px",
                        padding: "1.5rem",
                        boxShadow: "0 4px 6px rgba(0,0,0,0.07)",
                        transition: "all 0.3s ease"
                      }}
                      onMouseEnter={(e) => {
                        e.currentTarget.style.transform = "translateY(-4px)";
                        e.currentTarget.style.boxShadow = "0 8px 25px rgba(0,0,0,0.15)";
                        e.currentTarget.style.borderColor = "#007bff";
                      }}
                      onMouseLeave={(e) => {
                        e.currentTarget.style.transform = "translateY(0)";
                        e.currentTarget.style.boxShadow = "0 4px 6px rgba(0,0,0,0.07)";
                        e.currentTarget.style.borderColor = "#e9ecef";
                      }}
                    >
                      {/* Header de la estación */}
                      <div style={{ 
                        marginBottom: "1.5rem",
                        paddingBottom: "1rem",
                        borderBottom: "2px solid #f8f9fa"
                      }}>
                        <h4 style={{ 
                          margin: "0 0 0.5rem 0", 
                          color: "#212529",
                          fontSize: "1.3rem",
                          fontWeight: "600",
                          display: "flex",
                          alignItems: "center",
                          gap: "0.5rem"
                        }}>
                          ⛽ {ubicacion.nombre}
                        </h4>
                        <p style={{ 
                          margin: "0", 
                          color: "#6c757d",
                          fontSize: "0.95rem",
                          display: "flex",
                          alignItems: "center",
                          gap: "0.25rem"
                        }}>
                          📍 {ubicacion.ciudad}, {ubicacion.departamento}
                        </p>
                      </div>

                      {/* Grid de precios */}
                      <div style={{
                        display: "grid",
                        gap: "1rem",
                        marginBottom: "1.5rem"
                      }}>
                        {/* Nafta Súper */}
                        <div style={{
                          display: "flex",
                          justifyContent: "space-between",
                          alignItems: "center",
                          padding: "1rem",
                          backgroundColor: "#f8fff9",
                          borderRadius: "10px",
                          border: "1px solid #d4edda"
                        }}>
                          <div style={{
                            display: "flex",
                            alignItems: "center",
                            gap: "0.5rem"
                          }}>
                            <span style={{ 
                              fontSize: "1.2rem",
                              backgroundColor: "#28a745",
                              color: "white",
                              padding: "0.25rem 0.5rem",
                              borderRadius: "6px",
                              fontSize: "0.8rem",
                              fontWeight: "600"
                            }}>
                              S
                            </span>
                            <span style={{ 
                              fontWeight: "600",
                              color: "#155724",
                              fontSize: "1rem"
                            }}>
                              Nafta Súper
                            </span>
                          </div>
                          <span style={{ 
                            fontWeight: "bold",
                            fontSize: "1.4rem",
                            color: getPriceColor(ubicacion.precioNaftaSuper, "super")
                          }}>
                            {formatPrice(ubicacion.precioNaftaSuper)}
                          </span>
                        </div>

                        {/* Nafta Premium */}
                        <div style={{
                          display: "flex",
                          justifyContent: "space-between",
                          alignItems: "center",
                          padding: "1rem",
                          backgroundColor: "#f8f9ff",
                          borderRadius: "10px",
                          border: "1px solid #cce7ff"
                        }}>
                          <div style={{
                            display: "flex",
                            alignItems: "center",
                            gap: "0.5rem"
                          }}>
                            <span style={{ 
                              backgroundColor: "#007bff",
                              color: "white",
                              padding: "0.25rem 0.5rem",
                              borderRadius: "6px",
                              fontSize: "0.8rem",
                              fontWeight: "600"
                            }}>
                              P
                            </span>
                            <span style={{ 
                              fontWeight: "600",
                              color: "#004085",
                              fontSize: "1rem"
                            }}>
                              Nafta Premium
                            </span>
                          </div>
                          <span style={{ 
                            fontWeight: "bold",
                            fontSize: "1.4rem",
                            color: getPriceColor(ubicacion.precioNaftaPremium, "premium")
                          }}>
                            {formatPrice(ubicacion.precioNaftaPremium)}
                          </span>
                        </div>

                        {/* Diesel */}
                        <div style={{
                          display: "flex",
                          justifyContent: "space-between",
                          alignItems: "center",
                          padding: "1rem",
                          backgroundColor: "#fff8f0",
                          borderRadius: "10px",
                          border: "1px solid #ffeaa7"
                        }}>
                          <div style={{
                            display: "flex",
                            alignItems: "center",
                            gap: "0.5rem"
                          }}>
                            <span style={{ 
                              backgroundColor: "#fd7e14",
                              color: "white",
                              padding: "0.25rem 0.5rem",
                              borderRadius: "6px",
                              fontSize: "0.8rem",
                              fontWeight: "600"
                            }}>
                              D
                            </span>
                            <span style={{ 
                              fontWeight: "600",
                              color: "#8a4100",
                              fontSize: "1rem"
                            }}>
                              Diesel
                            </span>
                          </div>
                          <span style={{ 
                            fontWeight: "bold",
                            fontSize: "1.4rem",
                            color: getPriceColor(ubicacion.precioDiesel, "diesel")
                          }}>
                            {formatPrice(ubicacion.precioDiesel)}
                          </span>
                        </div>
                      </div>

                      {/* Botón para ver productos */}
                      <div style={{ marginBottom: "1rem" }}>
                        <button
                          onClick={() => abrirCatalogoProductos(ubicacion)}
                          style={{
                            width: "100%",
                            backgroundColor: "#28a745",
                            color: "white",
                            border: "none",
                            borderRadius: "8px",
                            padding: "0.75rem 1rem",
                            fontSize: "0.95rem",
                            fontWeight: "500",
                            cursor: "pointer",
                            transition: "background-color 0.2s ease",
                            display: "flex",
                            alignItems: "center",
                            justifyContent: "center",
                            gap: "0.5rem"
                          }}
                          onMouseEnter={(e) => e.target.style.backgroundColor = "#218838"}
                          onMouseLeave={(e) => e.target.style.backgroundColor = "#28a745"}
                        >
                          🛒 Ver Productos
                        </button>
                      </div>

                      {/* Footer con información adicional */}
                      <div style={{
                        paddingTop: "1rem",
                        borderTop: "1px solid #f8f9fa",
                        display: "flex",
                        justifyContent: "space-between",
                        alignItems: "center",
                        fontSize: "0.85rem",
                        color: "#6c757d"
                      }}>
                        <span>🕐 Precios actualizados</span>
                        <Link 
                          to="/estaciones"
                          style={{
                            color: "#007bff",
                            textDecoration: "none",
                            fontWeight: "500",
                            fontSize: "0.9rem"
                          }}
                        >
                          Ver detalles →
                        </Link>
                      </div>
                    </div>
                  ))}
                </div>

                {/* Nota sobre el futuro mapa */}
                <div style={{
                  marginTop: "2rem",
                  padding: "1.5rem",
                  backgroundColor: "#fff3cd",
                  borderRadius: "12px",
                  border: "1px solid #ffeaa7",
                  textAlign: "center"
                }}>
                  <h4 style={{ 
                    margin: "0 0 0.5rem 0", 
                    color: "#856404",
                    fontSize: "1.2rem"
                  }}>
                    🗺️ Próximamente: Mapa Interactivo
                  </h4>
                  <p style={{ 
                    margin: "0", 
                    color: "#856404",
                    fontSize: "1rem"
                  }}>
                    Estamos trabajando en un mapa interactivo con OpenStreetMaps para mostrar la ubicación de nuestras estaciones.
                  </p>
                </div>
              </>
            )}
          </>
        ) : (
          <>
            <h2 style={{ color: "#495057", marginBottom: "1rem" }}>
              ¡Bienvenido a Servipuntos!
            </h2>
            <p style={{ marginBottom: "1.5rem", fontSize: "1.1rem" }}>
              Ingresa para ver precios actualizados de combustible en las estaciones de tu red.
            </p>
            <Link
              to="/login"
              style={{
                display: "inline-block",
                backgroundColor: "#7B3F00",
                color: "white",
                padding: "0.75rem 1.5rem",
                borderRadius: "6px",
                textDecoration: "none",
                fontSize: "1rem",
                fontWeight: "500",
                transition: "background-color 0.2s ease"
              }}
              onMouseEnter={(e) => e.target.style.backgroundColor = "#5a2d00"}
              onMouseLeave={(e) => e.target.style.backgroundColor = "#7B3F00"}
            >
              Iniciar Sesión
            </Link>
            
            {/* Información adicional para usuarios no autenticados */}
            <div style={{
              marginTop: "2rem",
              padding: "2rem",
              backgroundColor: "white",
              borderRadius: "12px",
              border: "1px solid #dee2e6"
            }}>
              <h3 style={{ color: "#495057", marginBottom: "1rem" }}>
                ¿Qué puedes hacer en Servipuntos?
              </h3>
              <div style={{
                display: "grid",
                gridTemplateColumns: "repeat(auto-fit, minmax(250px, 1fr))",
                gap: "1.5rem",
                marginTop: "1.5rem"
              }}>
                <div style={{ textAlign: "center" }}>
                  <div style={{ fontSize: "3rem", marginBottom: "0.5rem" }}>💰</div>
                  <h4 style={{ color: "#7B3F00", marginBottom: "0.5rem" }}>Precios Actualizados</h4>
                  <p style={{ color: "#6c757d", fontSize: "0.95rem" }}>
                    Consulta precios en tiempo real de Nafta Súper, Premium y Diesel
                  </p>
                </div>
                <div style={{ textAlign: "center" }}>
                  <div style={{ fontSize: "3rem", marginBottom: "0.5rem" }}>🏪</div>
                  <h4 style={{ color: "#7B3F00", marginBottom: "0.5rem" }}>Red de Estaciones</h4>
                  <p style={{ color: "#6c757d", fontSize: "0.95rem" }}>
                    Encuentra todas las estaciones de tu red con información completa
                  </p>
                </div>
                <div style={{ textAlign: "center" }}>
                  <div style={{ fontSize: "3rem", marginBottom: "0.5rem" }}>🛒</div>
                  <h4 style={{ color: "#7B3F00", marginBottom: "0.5rem" }}>Catálogo de Productos</h4>
                  <p style={{ color: "#6c757d", fontSize: "0.95rem" }}>
                    Descubre productos disponibles para canje en cada estación
                  </p>
                </div>
                <div style={{ textAlign: "center" }}>
                  <div style={{ fontSize: "3rem", marginBottom: "0.5rem" }}>🗺️</div>
                  <h4 style={{ color: "#7B3F00", marginBottom: "0.5rem" }}>Ubicaciones</h4>
                  <p style={{ color: "#6c757d", fontSize: "0.95rem" }}>
                    Próximamente: mapa interactivo para ubicar estaciones fácilmente
                  </p>
                </div>
              </div>
            </div>
          </>
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

export default Home;