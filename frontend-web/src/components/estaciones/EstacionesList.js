import React, { useState, useEffect } from "react";
import { Link } from "react-router-dom";
import Breadcrumb from "../layout/Breadcrumb";
import authService from "../../services/authService";
import apiService from "../../services/apiService";
import CatalogoProductos from "../productos/CatalogoProductos";
import ComprarCombustibleModal from "./ComprarCombustibleModal";
import ComprarServicioModal from "./ComprarServicioModal";

const EstacionesList = () => {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [ubicaciones, setUbicaciones] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [userProfile, setUserProfile] = useState(null);
  
  // Estados para el modal de productos
  const [modalAbierto, setModalAbierto] = useState(false);
  const [ubicacionSeleccionada, setUbicacionSeleccionada] = useState(null);
  const [modalCombustible, setModalCombustible] = useState({ abierto: false, tipo: "", precio: 0, ubicacion: null });
  const [modalServicio, setModalServicio] = useState({ abierto: false, servicio: "", precio: 0, ubicacion: null });

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

      // Fallback: intentar obtener todas las ubicaciones públicas
      try {
        const ubicacionesData = await apiService.getAllUbicaciones();
        setUbicaciones(ubicacionesData);
      } catch {
        // Si también falla, dejar la lista vacía
        setUbicaciones([]);
      }
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

  const abrirModalCombustible = (ubicacion, tipo, precio) => {
    setModalCombustible({ abierto: true, tipo, precio, ubicacion });
  };

  const cerrarModalCombustible = () => {
    setModalCombustible({ abierto: false, tipo: "", precio: 0, ubicacion: null });
  };

  const abrirModalServicio = (ubicacion, servicio, precio) => {
    setModalServicio({ abierto: true, servicio, precio, ubicacion });
  };

  const cerrarModalServicio = () => {
    setModalServicio({ abierto: false, servicio: "", precio: 0, ubicacion: null });
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
        <Breadcrumb current="Estaciones" />
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
                  <div
                    style={{
                      marginBottom: "1.25rem",
                      display: "grid",
                      gap: "0.5rem",
                    }}
                  >
                    {[
                      {
                        label: "Nafta Súper",
                        precio: ubicacion.precioNaftaSuper,
                        color: "#2d5a2d",
                        bg: "#e8f5e8",
                        border: "#c3e6c3",
                        emoji: "🚗",
                      },
                      {
                        label: "Nafta Premium",
                        precio: ubicacion.precioNaftaPremium,
                        color: "#b8860b",
                        bg: "#fff9c4",
                        border: "#f9c74f",
                        emoji: "⭐",
                      },
                      {
                        label: "Diesel",
                        precio: ubicacion.precioDiesel,
                        color: "#1565c0",
                        bg: "#e3f2fd",
                        border: "#90caf9",
                        emoji: "🚛",
                      },
                    ].map((c) => (
                      <div
                        key={c.label}
                        style={{
                          display: "flex",
                          justifyContent: "space-between",
                          alignItems: "center",
                          padding: "0.75rem",
                          backgroundColor: c.bg,
                          borderRadius: "8px",
                          border: `1px solid ${c.border}`,
                        }}
                      >
                        <div style={{ display: "flex", alignItems: "center", gap: "0.5rem" }}>
                          <span style={{ fontSize: "1rem" }}>{c.emoji}</span>
                          <span style={{ fontWeight: "600", color: c.color }}>{c.label}</span>
                        </div>
                        <div style={{ textAlign: "right" }}>
                          <div style={{ fontWeight: "bold", color: c.color }}>
                            ${formatPrice(c.precio)}
                          </div>
                          <button
                            style={{
                              marginTop: "0.25rem",
                              padding: "0.25rem 0.5rem",
                              backgroundColor: c.color,
                              color: "white",
                              border: "none",
                              borderRadius: "4px",
                              cursor: "pointer",
                            }}
                            onClick={() => abrirModalCombustible(ubicacion, c.label, c.precio)}
                          >
                            Comprar
                          </button>
                        </div>
                      </div>
                    ))}
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
                  {(ubicacion.cambioDeAceite || ubicacion.cambioDeNeumaticos || ubicacion.lavadoDeAuto) && (
                    <div style={{ display: "grid", gap: "0.5rem", marginBottom: "1rem" }}>
                      {ubicacion.cambioDeAceite && (
                        <button
                          onClick={() => abrirModalServicio(ubicacion, "Cambio de Aceite", ubicacion.precioCambioAceite)}
                          style={{
                            width: "100%",
                            padding: "0.5rem",
                            backgroundColor: "#6f42c1",
                            color: "white",
                            border: "none",
                            borderRadius: "6px",
                            cursor: "pointer",
                          }}
                        >
                          Cambio de Aceite
                        </button>
                      )}
                      {ubicacion.cambioDeNeumaticos && (
                        <button
                          onClick={() => abrirModalServicio(ubicacion, "Cambio de Neumáticos", ubicacion.precioCambioNeumaticos)}
                          style={{
                            width: "100%",
                            padding: "0.5rem",
                            backgroundColor: "#20c997",
                            color: "white",
                            border: "none",
                            borderRadius: "6px",
                            cursor: "pointer",
                          }}
                        >
                          Cambio de Neumáticos
                        </button>
                      )}
                      {ubicacion.lavadoDeAuto && (
                        <button
                          onClick={() => abrirModalServicio(ubicacion, "Lavado de Auto", ubicacion.precioLavado)}
                          style={{
                            width: "100%",
                            padding: "0.5rem",
                            backgroundColor: "#17a2b8",
                            color: "white",
                            border: "none",
                            borderRadius: "6px",
                            cursor: "pointer",
                          }}
                        >
                          Lavado de Auto
                        </button>
                      )}
                    </div>
                  )}
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
      <ComprarCombustibleModal
        isOpen={modalCombustible.abierto}
        onClose={cerrarModalCombustible}
        ubicacion={modalCombustible.ubicacion}
        tipo={modalCombustible.tipo}
        precio={modalCombustible.precio}
        userProfile={userProfile}
      />
      <ComprarServicioModal
        isOpen={modalServicio.abierto}
        onClose={cerrarModalServicio}
        ubicacion={modalServicio.ubicacion}
        servicio={modalServicio.servicio}
        precio={modalServicio.precio}
        userProfile={userProfile}
      />
    </div>
  );
};

export default EstacionesList;