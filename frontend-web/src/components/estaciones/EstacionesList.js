import React, { useState, useEffect } from "react";
import { Link } from "react-router-dom";
import authService from "../../services/authService";
import apiService from "../../services/apiService";
import CatalogoProductos from "../productos/CatalogoProductos";

const EstacionesList = () => {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [ubicaciones, setUbicaciones] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [userProfile, setUserProfile] = useState(null);
  
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
      
      // Primero obtener el perfil del usuario
      const profile = await apiService.getUserProfile();
      setUserProfile(profile);
      
      // Obtener las ubicaciones de su tenant
      const ubicacionesData = await apiService.getUbicacionesByUserTenant();
      setUbicaciones(ubicacionesData);
      
      
    } catch (err) {
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

  if (!isAuthenticated) {
    return (
      <div
        style={{
          maxWidth: "800px",
          margin: "2rem auto",
          padding: "2rem",
          textAlign: "center",
          backgroundColor: "white",
          borderRadius: "12px",
          boxShadow: "0 4px 6px rgba(0,0,0,0.1)"
        }}
      >
        <div style={{ fontSize: "3rem", marginBottom: "1rem" }}>🔒</div>
        <h2 style={{ color: "#495057", marginBottom: "1rem" }}>
          Acceso Requerido
        </h2>
        <p style={{ marginBottom: "1.5rem", fontSize: "1.1rem", color: "#6c757d" }}>
          Necesitas iniciar sesión para ver las estaciones de servicio de tu red.
        </p>
        <Link
          to="/login"
          style={{
            display: "inline-block",
            backgroundColor: "#007bff",
            color: "white",
            padding: "0.75rem 1.5rem",
            borderRadius: "6px",
            textDecoration: "none",
            fontSize: "1rem",
            fontWeight: "500"
          }}
        >
          Iniciar Sesión
        </Link>
      </div>
    );
  }

  return (
    <div
      style={{
        maxWidth: "1200px",
        margin: "0 auto",
        padding: "1rem",
      }}
    >
      {/* Header con breadcrumb */}
      <div style={{ marginBottom: "2rem" }}>
        <div style={{ 
          fontSize: "0.9rem", 
          color: "#6c757d", 
          marginBottom: "0.5rem",
          display: "flex",
          alignItems: "center",
          gap: "0.5rem"
        }}>
          <Link to="/" style={{ color: "#007bff", textDecoration: "none" }}>🏠 Inicio</Link>
          <span>›</span>
          <span>Estaciones</span>
        </div>
        <h1 style={{ color: "#7B3F00", fontSize: "2.5rem", marginBottom: "0.5rem", margin: 0 }}>
          Estaciones de Servicio
        </h1>
        <p style={{ fontSize: "1.2rem", color: "#6c757d", margin: 0 }}>
          Información completa de todas nuestras estaciones
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
              border: "1px solid #bbdefb",
              display: "flex",
              justifyContent: "space-between",
              alignItems: "center",
              flexWrap: "wrap",
              gap: "1rem"
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
              
              <Link
                to="/"
                style={{
                  color: "#1976d2",
                  textDecoration: "none",
                  fontSize: "0.9rem",
                  fontWeight: "500",
                  display: "flex",
                  alignItems: "center",
                  gap: "0.25rem"
                }}
              >
                ← Volver al inicio
              </Link>
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

                  {/* Horarios y servicios (colapsados para ahorrar espacio) */}
                  {(ubicacion.horaApertura && ubicacion.horaCierre && 
                    ubicacion.horaApertura !== "00:00:00" && ubicacion.horaCierre !== "00:00:00") && (
                    <div style={{
                      marginBottom: "1rem",
                      padding: "0.75rem",
                      backgroundColor: "#e8f5e8",
                      borderRadius: "6px",
                      fontSize: "0.85rem"
                    }}>
                      <span style={{ color: "#388e3c", fontWeight: "600" }}>
                        🕐 {ubicacion.horaApertura.slice(0, 5)} - {ubicacion.horaCierre.slice(0, 5)}
                      </span>
                    </div>
                  )}

                  {/* Información del sistema */}
                  <div style={{
                    padding: "0.5rem",
                    backgroundColor: "#f8f9fa",
                    borderRadius: "4px",
                    fontSize: "0.75rem",
                    color: "#6c757d",
                    textAlign: "center"
                  }}>
                    ID: {ubicacion.id.slice(0, 8)}...
                  </div>
                </div>
              ))}
            </div>
          </>
        )}
      </div>

      {/* Modal de catálogo de productos */}
      <CatalogoProductos
        ubicacion={ubicacionSeleccionada}
        isOpen={modalAbierto}
        onClose={cerrarCatalogoProductos}
        userProfile={userProfile}
      />
    </div>
  );
};

export default EstacionesList;