import React, { useState, useEffect } from "react";
import { Link } from "react-router-dom";
import authService from "../services/authService";
import apiService from "../services/apiService";

const Home = () => {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [ubicaciones, setUbicaciones] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [userProfile, setUserProfile] = useState(null);
  const [tenantInfo, setTenantInfo] = useState(null);

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
          Encuentra nuestras estaciones de servicio con precios actualizados
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
            <div style={{ 
              display: "flex", 
              justifyContent: "space-between", 
              alignItems: "center",
              marginBottom: "2rem",
              flexWrap: "wrap",
              gap: "1rem"
            }}>
              <div>
                <h2 style={{ margin: "0", color: "#495057" }}>¡Bienvenido de vuelta!</h2>
                {tenantInfo && (
                  <p style={{ margin: "0.5rem 0 0 0", color: "#6c757d" }}>
                    Red de estaciones: <strong>{tenantInfo.nombre}</strong>
                  </p>
                )}
              </div>
              <Link
                to="/dashboard"
                style={{
                  display: "inline-block",
                  backgroundColor: "#007bff",
                  color: "white",
                  textDecoration: "none",
                  borderRadius: "8px",
                  padding: "0.75rem 1.5rem",
                  fontWeight: "600",
                  transition: "background-color 0.3s ease"
                }}
                onMouseEnter={(e) => e.target.style.backgroundColor = "#0056b3"}
                onMouseLeave={(e) => e.target.style.backgroundColor = "#007bff"}
              >
                Ir al Dashboard
              </Link>
            </div>

            {/* Sección de ubicaciones */}
            <div>
              <h3 style={{ 
                color: "#495057", 
                marginBottom: "1.5rem",
                display: "flex",
                alignItems: "center",
                gap: "0.5rem"
              }}>
                📍 Nuestras Estaciones de Servicio
              </h3>

              {loading && (
                <div style={{ textAlign: "center", padding: "3rem" }}>
                  <div style={{
                    border: "4px solid #f3f3f3",
                    borderTop: "4px solid #007bff",
                    borderRadius: "50%",
                    width: "50px",
                    height: "50px",
                    animation: "spin 2s linear infinite",
                    margin: "0 auto 1rem"
                  }} />
                  <p style={{ color: "#6c757d" }}>Cargando ubicaciones...</p>
                  
                  <style jsx>{`
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
                    📋 Sin ubicaciones disponibles
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

                        {/* Servicios adicionales */}
                        {(ubicacion.lavadoDeAuto || ubicacion.lavado || ubicacion.cambioDeAceite || ubicacion.cambioDeNeumaticos) && (
                          <div style={{
                            marginBottom: "1.25rem",
                            padding: "0.75rem",
                            backgroundColor: "#f3e5f5",
                            borderRadius: "8px",
                            border: "1px solid #ce93d8"
                          }}>
                            <h5 style={{ 
                              margin: "0 0 0.75rem 0", 
                              color: "#7b1fa2",
                              fontSize: "0.95rem",
                              display: "flex",
                              alignItems: "center",
                              gap: "0.5rem"
                            }}>
                              🔧 Servicios Adicionales
                            </h5>
                            <div style={{ 
                              display: "grid", 
                              gridTemplateColumns: "repeat(auto-fit, minmax(140px, 1fr))",
                              gap: "0.5rem"
                            }}>
                              {ubicacion.lavadoDeAuto && (
                                <div style={{
                                  display: "flex",
                                  alignItems: "center",
                                  gap: "0.5rem",
                                  fontSize: "0.85rem",
                                  color: "#7b1fa2",
                                  fontWeight: "500"
                                }}>
                                  🚗 Lavado de Auto
                                </div>
                              )}
                              {ubicacion.lavado && (
                                <div style={{
                                  display: "flex",
                                  alignItems: "center",
                                  gap: "0.5rem",
                                  fontSize: "0.85rem",
                                  color: "#7b1fa2",
                                  fontWeight: "500"
                                }}>
                                  🧽 Lavado
                                </div>
                              )}
                              {ubicacion.cambioDeAceite && (
                                <div style={{
                                  display: "flex",
                                  alignItems: "center",
                                  gap: "0.5rem",
                                  fontSize: "0.85rem",
                                  color: "#7b1fa2",
                                  fontWeight: "500"
                                }}>
                                  🛢️ Cambio de Aceite
                                </div>
                              )}
                              {ubicacion.cambioDeNeumaticos && (
                                <div style={{
                                  display: "flex",
                                  alignItems: "center",
                                  gap: "0.5rem",
                                  fontSize: "0.85rem",
                                  color: "#7b1fa2",
                                  fontWeight: "500"
                                }}>
                                  🛞 Cambio de Neumáticos
                                </div>
                              )}
                            </div>
                          </div>
                        )}

                        {/* Precios de combustible */}
                        <div>
                          <h5 style={{ 
                            margin: "0 0 1rem 0", 
                            color: "#495057",
                            fontSize: "1rem",
                            display: "flex",
                            alignItems: "center",
                            gap: "0.5rem"
                          }}>
                            ⛽ Precios por Litro
                          </h5>
                          
                          <div style={{ display: "grid", gap: "0.5rem" }}>
                            {ubicacion.precioNaftaSuper && ubicacion.precioNaftaSuper > 0 && (
                              <div style={{
                                display: "flex",
                                justifyContent: "space-between",
                                alignItems: "center",
                                padding: "0.75rem",
                                backgroundColor: "#e3f2fd",
                                borderRadius: "8px",
                                border: "1px solid #bbdefb"
                              }}>
                                <span style={{ 
                                  display: "flex", 
                                  alignItems: "center", 
                                  gap: "0.5rem",
                                  fontWeight: "500"
                                }}>
                                  🚗 Nafta Super
                                </span>
                                <span style={{ 
                                  fontWeight: "bold", 
                                  color: "#1976d2",
                                  fontSize: "1.1rem"
                                }}>
                                  {formatPrice(ubicacion.precioNaftaSuper)}
                                </span>
                              </div>
                            )}

                            {ubicacion.precioNaftaPremium && ubicacion.precioNaftaPremium > 0 && (
                              <div style={{
                                display: "flex",
                                justifyContent: "space-between",
                                alignItems: "center",
                                padding: "0.75rem",
                                backgroundColor: "#fff3e0",
                                borderRadius: "8px",
                                border: "1px solid #ffcc02"
                              }}>
                                <span style={{ 
                                  display: "flex", 
                                  alignItems: "center", 
                                  gap: "0.5rem",
                                  fontWeight: "500"
                                }}>
                                  ⭐ Nafta Premium
                                </span>
                                <span style={{ 
                                  fontWeight: "bold", 
                                  color: "#f57c00",
                                  fontSize: "1.1rem"
                                }}>
                                  {formatPrice(ubicacion.precioNaftaPremium)}
                                </span>
                              </div>
                            )}

                            {ubicacion.precioDiesel && ubicacion.precioDiesel > 0 && (
                              <div style={{
                                display: "flex",
                                justifyContent: "space-between",
                                alignItems: "center",
                                padding: "0.75rem",
                                backgroundColor: "#f3e5f5",
                                borderRadius: "8px",
                                border: "1px solid #ce93d8"
                              }}>
                                <span style={{ 
                                  display: "flex", 
                                  alignItems: "center", 
                                  gap: "0.5rem",
                                  fontWeight: "500"
                                }}>
                                  🚛 Diesel
                                </span>
                                <span style={{ 
                                  fontWeight: "bold", 
                                  color: "#7b1fa2",
                                  fontSize: "1.1rem"
                                }}>
                                  {formatPrice(ubicacion.precioDiesel)}
                                </span>
                              </div>
                            )}

                            {/* Si no hay precios disponibles */}
                            {(!ubicacion.precioNaftaSuper || ubicacion.precioNaftaSuper === 0) &&
                             (!ubicacion.precioNaftaPremium || ubicacion.precioNaftaPremium === 0) &&
                             (!ubicacion.precioDiesel || ubicacion.precioDiesel === 0) && (
                              <div style={{
                                padding: "1.5rem",
                                backgroundColor: "#f8f9fa",
                                borderRadius: "8px",
                                textAlign: "center",
                                color: "#6c757d",
                                fontSize: "0.9rem",
                                border: "1px solid #dee2e6"
                              }}>
                                📋 Precios no disponibles en este momento
                              </div>
                            )}
                          </div>
                        </div>

                        {/* Footer con información adicional */}
                        <div style={{ display: "grid", gap: "0.75rem", marginTop: "1.25rem" }}>
                          
                          {/* Promociones */}
                          {ubicacion.promociones && ubicacion.promociones.length > 0 && (
                            <div style={{
                              padding: "0.75rem",
                              backgroundColor: "#fff3e0",
                              borderRadius: "8px",
                              fontSize: "0.85rem",
                              border: "1px solid #ffcc02"
                            }}>
                              <div style={{ 
                                color: "#f57c00", 
                                fontWeight: "600",
                                marginBottom: "0.5rem",
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

                          {/* Información del sistema (siempre visible) */}
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
                      </div>
                    ))}
                  </div>
                </>
              )}
            </div>
          </>
        ) : (
          <>
            <h2 style={{ color: "#495057", marginBottom: "1rem" }}>
              ¡Bienvenido a Servipuntos!
            </h2>
            <p style={{ marginBottom: "1.5rem", fontSize: "1.1rem" }}>
              Ingresa para ver las estaciones de servicio de tu red con precios de Nafta Super, Nafta Premium y Diesel.
            </p>
            
            <div style={{ 
              backgroundColor: "#e9ecef", 
              padding: "1.5rem", 
              borderRadius: "8px",
              marginBottom: "2rem"
            }}>
              <h4 style={{ margin: "0 0 1rem 0", color: "#495057" }}>
                Credenciales de prueba:
              </h4>
              <div style={{ display: "grid", gap: "0.5rem" }}>
                <p style={{ margin: "0" }}>
                  <strong>Usuario:</strong> user@gmail.com | <strong>Contraseña:</strong> user
                </p>
                <p style={{ margin: "0" }}>
                  <strong>Admin:</strong> admin@gmail.com | <strong>Contraseña:</strong> admin
                </p>
              </div>
            </div>
            
            <Link
              to="/login"
              style={{
                display: "inline-block",
                backgroundColor: "#7B3F00",
                color: "white",
                textDecoration: "none",
                borderRadius: "8px",
                padding: "1rem 2rem",
                fontWeight: "600",
                fontSize: "1.1rem",
                transition: "background-color 0.3s ease"
              }}
              onMouseEnter={(e) => e.target.style.backgroundColor = "#5d2f00"}
              onMouseLeave={(e) => e.target.style.backgroundColor = "#7B3F00"}
            >
              Iniciar Sesión
            </Link>
          </>
        )}
      </div>
    </div>
  );
};

export default Home;