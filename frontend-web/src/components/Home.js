import React, { useState, useEffect } from "react";
import { Link } from "react-router-dom";
import authService from "../services/authService";
import apiService from "../services/apiService";
import PuntosWidget from "./home/PuntosWidget";
import PreciosWidget from "./home/PreciosWidget";

const Home = () => {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [userProfile, setUserProfile] = useState(null);
  const [tenantInfo, setTenantInfo] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    const initializeHome = async () => {
      const authenticated = authService.isAuthenticated();
      setIsAuthenticated(authenticated);
      
      if (authenticated) {
        await loadUserData();
      } else {
        setLoading(false);
      }
    };

    initializeHome();
  }, []);

  const loadUserData = async () => {
    setLoading(true);
    setError("");
    
    try {
      
      // Cargar datos del usuario
      const profile = await apiService.getUserProfile();
      setUserProfile(profile);
      
      // Cargar información del tenant
      const tenant = await apiService.getTenantInfo();
      setTenantInfo(tenant);
      

      
      
    } catch (err) {
      setError("Error al cargar la información del dashboard");
    } finally {
      setLoading(false);
    }
  };

  // Renderizado para usuarios no autenticados
  const renderWelcomeScreen = () => (
    <div className="bg-light min-vh-100 d-flex align-items-center py-5">
      <div className="container text-center bg-white rounded shadow-sm p-5" style={{ maxWidth: "600px" }}>
        <div className="display-4 mb-3" role="img" aria-label="Estación" style={{ fontSize: "3rem" }}>⛽</div>
        <h1 className="mb-3" style={{ color: "#007BFF" }}>Servipuntos</h1>
        <p className="lead text-muted mb-4">
          Tu plataforma para gestionar puntos y consultar precios en toda nuestra red de estaciones.
        </p>
        <Link to="/login" className="btn btn-primary btn-lg mb-5" style={{ backgroundColor: "#007BFF" }}>
          Iniciar Sesión
        </Link>
        
        <div className="row justify-content-center">
          <div className="col-6 col-md-4 mb-4">
            <h5 style={{ color: "#007BFF" }}>Puntos y Recompensas</h5>
            <p className="text-muted small">Acumula puntos y canjea productos</p>
          </div>
          <div className="col-6 col-md-4 mb-4">
            <h5 style={{ color: "#007BFF" }}>Precios Actualizados</h5>
            <p className="text-muted small">Consulta valores en tiempo real</p>
          </div>
          <div className="col-6 col-md-4 mb-4">
            <h5 style={{ color: "#007BFF" }}>Estaciones Cercanas</h5>
            <p className="text-muted small">Encuentra ubicaciones en tu ruta</p>
          </div>
        </div>
      </div>
    </div>
  );

  // Loading state
  if (loading) {
    return (
      <div
        style={{
          minHeight: "80vh",
          display: "flex",
          alignItems: "center",
          justifyContent: "center",
          flexDirection: "column"
        }}
      >
        <div
          style={{
            border: "4px solid #f3f3f3",
            borderTop: "4px solid var(--primary-color)",
            borderRadius: "50%",
            width: "60px",
            height: "60px",
            animation: "spin 1s linear infinite",
            marginBottom: "1rem"
          }}
        />
        <p style={{ color: "var(--primary-color)", fontSize: "1.1rem" }}>Cargando tu dashboard...</p>
        
        <style>{`
          @keyframes spin {
            0% { transform: rotate(0deg); }
            100% { transform: rotate(360deg); }
          }
        `}</style>
      </div>
    );
  }

  // Error state
  if (error) {
    return (
      <div
        style={{
          maxWidth: "800px",
          margin: "2rem auto",
          padding: "2rem",
          textAlign: "center"
        }}
      >
        <div
          style={{
            backgroundColor: "#fff5f5",
            border: "1px solid #fed7d7",
            borderRadius: "8px",
            padding: "1.5rem",
            marginBottom: "1rem"
          }}
        >
          <h3 style={{ color: "#c53030", marginBottom: "0.5rem" }}>
            ⚠️ Error al cargar el dashboard
          </h3>
          <p style={{ color: "#742a2a", margin: "0" }}>{error}</p>
        </div>
        
        <button
          onClick={loadUserData}
          style={{
            backgroundColor: "var(--primary-color)",
            color: "white",
            border: "none",
            borderRadius: "8px",
            padding: "0.75rem 1.5rem",
            cursor: "pointer",
            fontSize: "1rem",
            fontWeight: "600"
          }}
        >
          🔄 Reintentar
        </button>
      </div>
    );
  }

  // Dashboard para usuarios autenticados
  if (isAuthenticated && userProfile && tenantInfo) {
    return (
      <div style={{ 
        minHeight: "80vh", 
        backgroundColor: "#f8f9fa",
        padding: "2rem 1rem"
      }}>
        <div style={{ maxWidth: "1200px", margin: "0 auto" }}>
          
          {/* Header del Dashboard */}
          <div style={{ marginBottom: "2rem" }}>
            <h1 style={{
              fontSize: "2.5rem",
              color: "var(--primary-color)",
              margin: "0 0 0.5rem 0",
              fontWeight: "700"
            }}>
              ¡Hola, {userProfile.nombre}! 👋
            </h1>
            <p style={{ 
              fontSize: "1.1rem", 
              color: "#6c757d", 
              margin: "0",
              lineHeight: "1.5"
            }}>
              Bienvenido a {tenantInfo.nombre}
            </p>
          </div>

          {/* Grid de Widgets */}
          <div style={{
            display: "grid",
            gridTemplateColumns: "repeat(auto-fit, minmax(350px, 1fr))",
            gap: "2rem",
            marginBottom: "2rem"
          }}>
            
            {/* Widget de Puntos */}
            <PuntosWidget 
              userProfile={userProfile} 
              tenantInfo={tenantInfo} 
            />

            {/* Widget de Precios */}
            <PreciosWidget 
              tenantInfo={tenantInfo} 
            />

          </div>

          {/* Acciones Rápidas */}
          <div style={{
            backgroundColor: "white",
            borderRadius: "12px",
            padding: "1.5rem",
            boxShadow: "0 4px 12px rgba(0,0,0,0.1)",
            border: "1px solid #e9ecef",
            marginBottom: "2rem"
          }}>
            <h3 style={{
              margin: "0 0 1.5rem 0",
              color: "var(--primary-color)",
              fontSize: "1.2rem",
              fontWeight: "600",
              display: "flex",
              alignItems: "center",
              gap: "0.5rem"
            }}>
              🚀 Acciones Rápidas
            </h3>
            
            <div style={{
              display: "grid",
              gridTemplateColumns: "repeat(auto-fit, minmax(200px, 1fr))",
              gap: "1rem"
            }}>
              
              <Link
                to="/estaciones"
                style={{
                  display: "flex",
                  alignItems: "center",
                  gap: "0.75rem",
                  padding: "1rem",
                  backgroundColor: "#e8f5e8",
                  borderRadius: "8px",
                  textDecoration: "none",
                  color: "#2d5a2d",
                  border: "1px solid #c3e6c3",
                  transition: "transform 0.2s ease, box-shadow 0.2s ease"
                }}
                onMouseEnter={(e) => {
                  e.target.style.transform = "translateY(-2px)";
                  e.target.style.boxShadow = "0 4px 12px rgba(45,90,45,0.2)";
                }}
                onMouseLeave={(e) => {
                  e.target.style.transform = "translateY(0)";
                  e.target.style.boxShadow = "none";
                }}
              >
                <span style={{ fontSize: "1.5rem" }}>⛽</span>
                <div>
                  <strong>Ver Estaciones</strong>
                  <div style={{ fontSize: "0.8rem", color: "#6c757d" }}>Ubicaciones y precios</div>
                </div>
              </Link>

              <Link
                to="/perfil"
                style={{
                  display: "flex",
                  alignItems: "center",
                  gap: "0.75rem",
                  padding: "1rem",
                  backgroundColor: "#fff9c4",
                  borderRadius: "8px",
                  textDecoration: "none",
                  color: "#b8860b",
                  border: "1px solid #f9c74f",
                  transition: "transform 0.2s ease, box-shadow 0.2s ease"
                }}
                onMouseEnter={(e) => {
                  e.target.style.transform = "translateY(-2px)";
                  e.target.style.boxShadow = "0 4px 12px rgba(184,134,11,0.2)";
                }}
                onMouseLeave={(e) => {
                  e.target.style.transform = "translateY(0)";
                  e.target.style.boxShadow = "none";
                }}
              >
                <span style={{ fontSize: "1.5rem" }}>👤</span>
                <div>
                  <strong>Mi Perfil</strong>
                  <div style={{ fontSize: "0.8rem", color: "#6c757d" }}>Configuración y datos</div>
                </div>
              </Link>

              <div
                style={{
                  display: "flex",
                  alignItems: "center",
                  gap: "0.75rem",
                  padding: "1rem",
                  backgroundColor: "#f8f9fa",
                  borderRadius: "8px",
                  color: "#6c757d",
                  border: "1px dashed #dee2e6",
                  opacity: 0.7
                }}
              >
                <span style={{ fontSize: "1.5rem" }}>🗺️</span>
                <div>
                  <strong>Mapa</strong>
                  <div style={{ fontSize: "0.8rem" }}>Próximamente</div>
                </div>
              </div>

              <Link
                to="/promociones"
                style={{
                  display: "flex",
                  alignItems: "center",
                  gap: "0.75rem",
                  padding: "1rem",
                  backgroundColor: "#e8f0fe",
                  borderRadius: "8px",
                  textDecoration: "none",
                  color: "#1a73e8",
                  border: "1px solid #c3dafe",
                  transition: "transform 0.2s ease, box-shadow 0.2s ease"
                }}
                onMouseEnter={(e) => {
                  e.target.style.transform = "translateY(-2px)";
                  e.target.style.boxShadow = "0 4px 12px rgba(26,115,232,0.2)";
                }}
                onMouseLeave={(e) => {
                  e.target.style.transform = "translateY(0)";
                  e.target.style.boxShadow = "none";
                }}
              >
                <span style={{ fontSize: "1.5rem" }}>🎁</span>
                <div>
                  <strong>Promociones</strong>
                  <div style={{ fontSize: "0.8rem", color: "#6c757d" }}>Ofertas y beneficios</div>
                </div>
              </Link>
            </div>
          </div>

          
        </div>
      </div>
    );
  }

  // Fallback para usuarios autenticados sin datos
  return renderWelcomeScreen();
};

export default Home;