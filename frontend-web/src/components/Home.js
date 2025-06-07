import React, { useState, useEffect } from "react";
import { Link } from "react-router-dom";
import authService from "../services/authService";
import apiService from "../services/apiService";
import PuntosWidget from "./home/PuntosWidget";

const Home = () => {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [userProfile, setUserProfile] = useState(null);
  const [tenantInfo, setTenantInfo] = useState(null);
  const [ubicaciones, setUbicaciones] = useState([]);
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
      console.log("Cargando datos del usuario para dashboard...");
      
      // Cargar datos del usuario
      const profile = await apiService.getUserProfile();
      setUserProfile(profile);
      
      // Cargar información del tenant
      const tenant = await apiService.getTenantInfo();
      setTenantInfo(tenant);
      
      // Cargar ubicaciones para información general
      const ubicacionesData = await apiService.getUbicacionesByUserTenant();
      setUbicaciones(ubicacionesData);
      
      console.log("Datos del dashboard cargados:", { profile, tenant, ubicaciones: ubicacionesData.length });
      
    } catch (err) {
      console.error("Error al cargar datos del dashboard:", err);
      setError("Error al cargar la información del dashboard");
    } finally {
      setLoading(false);
    }
  };

  // Renderizado para usuarios no autenticados
  const renderWelcomeScreen = () => (
    <div
      style={{
        minHeight: "80vh",
        display: "flex",
        alignItems: "center",
        justifyContent: "center",
        backgroundColor: "#f8f9fa"
      }}
    >
      <div
        style={{
          textAlign: "center",
          maxWidth: "600px",
          padding: "3rem",
          backgroundColor: "white",
          borderRadius: "16px",
          boxShadow: "0 8px 32px rgba(0,0,0,0.1)"
        }}
      >
        <div style={{ fontSize: "4rem", marginBottom: "1rem" }}>⛽</div>
        <h1 style={{ color: "#7B3F00", fontSize: "3rem", marginBottom: "1rem" }}>
          Servipuntos
        </h1>
        <p style={{ fontSize: "1.3rem", color: "#6c757d", marginBottom: "2rem", lineHeight: "1.6" }}>
          Tu plataforma para gestionar puntos, consultar precios y canjear productos en toda nuestra red de estaciones.
        </p>
        <Link
          to="/login"
          style={{
            display: "inline-block",
            backgroundColor: "#7B3F00",
            color: "white",
            padding: "1rem 2rem",
            borderRadius: "8px",
            textDecoration: "none",
            fontSize: "1.1rem",
            fontWeight: "600",
            transition: "transform 0.2s ease, box-shadow 0.2s ease"
          }}
          onMouseEnter={(e) => {
            e.target.style.transform = "translateY(-2px)";
            e.target.style.boxShadow = "0 8px 20px rgba(123,63,0,0.3)";
          }}
          onMouseLeave={(e) => {
            e.target.style.transform = "translateY(0)";
            e.target.style.boxShadow = "none";
          }}
        >
          Iniciar Sesión
        </Link>
        
        {/* Características destacadas */}
        <div
          style={{
            display: "grid",
            gridTemplateColumns: "repeat(auto-fit, minmax(180px, 1fr))",
            gap: "1.5rem",
            marginTop: "3rem",
            textAlign: "center"
          }}
        >
          <div>
            <div style={{ fontSize: "2rem", marginBottom: "0.5rem" }}>💰</div>
            <h4 style={{ color: "#7B3F00", marginBottom: "0.5rem" }}>Puntos y Recompensas</h4>
            <p style={{ color: "#6c757d", fontSize: "0.9rem" }}>Acumula puntos y canjea productos</p>
          </div>
          <div>
            <div style={{ fontSize: "2rem", marginBottom: "0.5rem" }}>📊</div>
            <h4 style={{ color: "#7B3F00", marginBottom: "0.5rem" }}>Precios en Tiempo Real</h4>
            <p style={{ color: "#6c757d", fontSize: "0.9rem" }}>Consulta precios actualizados</p>
          </div>
          <div>
            <div style={{ fontSize: "2rem", marginBottom: "0.5rem" }}>🗺️</div>
            <h4 style={{ color: "#7B3F00", marginBottom: "0.5rem" }}>Red de Estaciones</h4>
            <p style={{ color: "#6c757d", fontSize: "0.9rem" }}>Encuentra estaciones cerca tuyo</p>
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
            borderTop: "4px solid #7B3F00",
            borderRadius: "50%",
            width: "60px",
            height: "60px",
            animation: "spin 1s linear infinite",
            marginBottom: "1rem"
          }}
        />
        <p style={{ color: "#7B3F00", fontSize: "1.1rem" }}>Cargando tu dashboard...</p>
        
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
          backgroundColor: "#f8d7da",
          color: "#721c24",
          borderRadius: "8px",
          border: "1px solid #f5c6cb",
          textAlign: "center"
        }}
      >
        <h3>⚠️ Error al cargar el dashboard</h3>
        <p>{error}</p>
        <button
          onClick={loadUserData}
          style={{
            backgroundColor: "#721c24",
            color: "white",
            border: "none",
            padding: "0.5rem 1rem",
            borderRadius: "4px",
            cursor: "pointer",
            marginTop: "1rem"
          }}
        >
          Reintentar
        </button>
      </div>
    );
  }

  // Dashboard para usuarios autenticados
  if (isAuthenticated && userProfile && tenantInfo) {
    return (
      <div
        style={{
          maxWidth: "1200px",
          margin: "0 auto",
          padding: "1rem",
          backgroundColor: "#f8f9fa",
          minHeight: "calc(100vh - 120px)"
        }}
      >
        {/* Header del Dashboard */}
        <div
          style={{
            marginBottom: "2rem",
            padding: "1.5rem",
            backgroundColor: "white",
            borderRadius: "12px",
            boxShadow: "0 2px 8px rgba(0,0,0,0.1)"
          }}
        >
          <h1
            style={{
              margin: "0 0 0.5rem 0",
              color: "#212529",
              fontSize: "2rem",
              fontWeight: "600"
            }}
          >
            ¡Bienvenido, {userProfile.nombre || userProfile.email}!
          </h1>
          <div
            style={{
              display: "flex",
              alignItems: "center",
              gap: "0.5rem",
              color: "#6c757d",
              fontSize: "1.1rem"
            }}
          >
            <span>🏢</span>
            <span>Red: <strong style={{ color: "#7B3F00" }}>{tenantInfo.nombre}</strong></span>
            <span style={{ margin: "0 0.5rem" }}>•</span>
            <span>📍 {ubicaciones.length} estacion{ubicaciones.length !== 1 ? 'es' : ''} disponible{ubicaciones.length !== 1 ? 's' : ''}</span>
          </div>
        </div>

        {/* Grid de Widgets */}
        <div
          style={{
            display: "grid",
            gridTemplateColumns: "repeat(auto-fit, minmax(320px, 1fr))",
            gap: "1.5rem",
            marginBottom: "2rem"
          }}
        >
          {/* Widget de Puntos */}
          <PuntosWidget 
            userProfile={userProfile}
            tenantInfo={tenantInfo}
          />

          {/* Placeholder para próximos widgets */}
          <div
            style={{
              padding: "1.5rem",
              backgroundColor: "white",
              borderRadius: "12px",
              boxShadow: "0 2px 8px rgba(0,0,0,0.1)",
              border: "2px dashed #dee2e6",
              textAlign: "center",
              color: "#6c757d"
            }}
          >
            <div style={{ fontSize: "2rem", marginBottom: "1rem" }}>⛽</div>
            <h4 style={{ margin: "0 0 0.5rem 0" }}>Precios Widget</h4>
            <p style={{ margin: "0", fontSize: "0.9rem" }}>Próximamente: Precios en tiempo real</p>
          </div>

          <div
            style={{
              padding: "1.5rem",
              backgroundColor: "white",
              borderRadius: "12px",
              boxShadow: "0 2px 8px rgba(0,0,0,0.1)",
              border: "2px dashed #dee2e6",
              textAlign: "center",
              color: "#6c757d"
            }}
          >
            <div style={{ fontSize: "2rem", marginBottom: "1rem" }}>🛒</div>
            <h4 style={{ margin: "0 0 0.5rem 0" }}>Productos Widget</h4>
            <p style={{ margin: "0", fontSize: "0.9rem" }}>Próximamente: Productos destacados</p>
          </div>
        </div>

        {/* Accesos Rápidos */}
        <div
          style={{
            padding: "1.5rem",
            backgroundColor: "white",
            borderRadius: "12px",
            boxShadow: "0 2px 8px rgba(0,0,0,0.1)"
          }}
        >
          <h3
            style={{
              margin: "0 0 1rem 0",
              color: "#212529",
              fontSize: "1.5rem",
              display: "flex",
              alignItems: "center",
              gap: "0.5rem"
            }}
          >
            🚀 Accesos Rápidos
          </h3>
          
          <div
            style={{
              display: "grid",
              gridTemplateColumns: "repeat(auto-fit, minmax(200px, 1fr))",
              gap: "1rem"
            }}
          >
            <Link
              to="/estaciones"
              style={{
                display: "flex",
                alignItems: "center",
                gap: "0.75rem",
                padding: "1rem",
                backgroundColor: "#f8f9fa",
                borderRadius: "8px",
                textDecoration: "none",
                color: "#495057",
                transition: "all 0.2s ease",
                border: "1px solid #dee2e6"
              }}
              onMouseEnter={(e) => {
                e.target.style.backgroundColor = "#e9ecef";
                e.target.style.transform = "translateY(-2px)";
              }}
              onMouseLeave={(e) => {
                e.target.style.backgroundColor = "#f8f9fa";
                e.target.style.transform = "translateY(0)";
              }}
            >
              <span style={{ fontSize: "1.5rem" }}>🏪</span>
              <div>
                <strong>Ver Estaciones</strong>
                <div style={{ fontSize: "0.8rem", color: "#6c757d" }}>Ubicaciones y servicios</div>
              </div>
            </Link>

            <Link
              to="/perfil"
              style={{
                display: "flex",
                alignItems: "center",
                gap: "0.75rem",
                padding: "1rem",
                backgroundColor: "#f8f9fa",
                borderRadius: "8px",
                textDecoration: "none",
                color: "#495057",
                transition: "all 0.2s ease",
                border: "1px solid #dee2e6"
              }}
              onMouseEnter={(e) => {
                e.target.style.backgroundColor = "#e9ecef";
                e.target.style.transform = "translateY(-2px)";
              }}
              onMouseLeave={(e) => {
                e.target.style.backgroundColor = "#f8f9fa";
                e.target.style.transform = "translateY(0)";
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
              <span style={{ fontSize: "1.5rem" }}>🎁</span>
              <div>
                <strong>Promociones</strong>
                <div style={{ fontSize: "0.8rem" }}>Próximamente</div>
              </div>
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