import React, { useState, useEffect } from "react";
import { Link } from "react-router-dom";
import authService from "../services/authService";
import apiService from "../services/apiService";

const NavBar = () => {
  const isAuthenticated = authService.isAuthenticated();
  const user = authService.getCurrentUser();
  const [tenantInfo, setTenantInfo] = useState(null);

  // Cargar información del tenant si el usuario está autenticado
  useEffect(() => {
    const loadTenantInfo = async () => {
      if (isAuthenticated) {
        try {
          const tenant = await apiService.getTenantInfo();
          setTenantInfo(tenant);
        } catch (err) {
          console.error("Error al cargar información del tenant:", err);
          // No mostrar error en la UI, mantener el nombre por defecto
        }
      }
    };

    loadTenantInfo();
  }, [isAuthenticated]);

  return (
    <nav
      style={{
        display: "flex",
        justifyContent: "space-between",
        padding: "1rem",
        backgroundColor: "#343a40",
        color: "white",
        marginBottom: "1rem",
      }}
    >
      <div>
        <Link
          to="/"
          style={{
            color: "white",
            textDecoration: "none",
            fontWeight: "bold",
            fontSize: "1.2rem",
          }}
        >
          {isAuthenticated && tenantInfo ? (
            <>
              {tenantInfo.nombre} <span style={{ color: "grey", fontSize: "0.9rem" }}>Servipuntos</span>
            </>
          ) : (
            <>
              Servipuntos.uy <span style={{ color: "grey" }}>*Demo*</span>
            </>
          )}
        </Link>
      </div>
      <div style={{ display: "flex", alignItems: "center" }}>
        {isAuthenticated ? (
          <>
            {user && (
              <span
                style={{
                  marginRight: "1rem",
                  backgroundColor:
                    user.role === "admin" ? "#6f42c1" : "#28a745",
                  color: "white",
                  padding: "0.25rem 0.5rem",
                  borderRadius: "4px",
                  fontSize: "0.8rem",
                }}
              >
                {user.username || user.uniqueName || user.name || user.email}
              </span>
            )}
            
            {/* Enlace a Estaciones */}
            <Link 
              to="/estaciones" 
              style={{ 
                color: 'white', 
                textDecoration: 'none',
                marginRight: '1rem',
                padding: '0.5rem 1rem',
                borderRadius: '6px',
                transition: 'background-color 0.2s',
                display: 'flex',
                alignItems: 'center',
                gap: '0.5rem',
                backgroundColor: 'rgba(255,255,255,0.1)',
                border: '1px solid rgba(255,255,255,0.2)'
              }}
              onMouseEnter={(e) => {
                e.target.style.backgroundColor = 'rgba(255,255,255,0.2)';
                e.target.style.borderColor = 'rgba(255,255,255,0.3)';
              }}
              onMouseLeave={(e) => {
                e.target.style.backgroundColor = 'rgba(255,255,255,0.1)';
                e.target.style.borderColor = 'rgba(255,255,255,0.2)';
              }}
            >
              🏪 Estaciones
            </Link>

            {/* Enlace al Dashboard */}
            <Link 
              to="/dashboard" 
              style={{ 
                color: 'white', 
                textDecoration: 'none',
                marginRight: '1rem',
                padding: '0.25rem 0.5rem',
                borderRadius: '4px',
                transition: 'background-color 0.2s'
              }}
              onMouseEnter={(e) => e.target.style.backgroundColor = 'rgba(255,255,255,0.1)'}
              onMouseLeave={(e) => e.target.style.backgroundColor = 'transparent'}
            >
              Dashboard
            </Link>

            {/* Enlace al Perfil */}
            <Link 
              to="/perfil" 
              style={{ 
                color: 'white', 
                textDecoration: 'none',
                marginRight: '1rem',
                padding: '0.25rem 0.5rem',
                borderRadius: '4px',
                transition: 'background-color 0.2s'
              }}
              onMouseEnter={(e) => e.target.style.backgroundColor = 'rgba(255,255,255,0.1)'}
              onMouseLeave={(e) => e.target.style.backgroundColor = 'transparent'}
            >
              👤 Perfil
            </Link>

            {/* Enlace para cerrar sesión */}
            <Link
              to="/"
              onClick={() => authService.logout()}
              style={{
                color: "white",
                textDecoration: "none",
                padding: '0.25rem 0.5rem',
                borderRadius: '4px',
                transition: 'background-color 0.2s',
                backgroundColor: '#dc3545',
              }}
              onMouseEnter={(e) => e.target.style.backgroundColor = '#c82333'}
              onMouseLeave={(e) => e.target.style.backgroundColor = '#dc3545'}
            >
              Cerrar Sesión
            </Link>
          </>
        ) : (
          <>
            {/* Cuando no está autenticado, mostrar enlaces públicos */}
            <Link 
              to="/login" 
              style={{ 
                color: 'white', 
                textDecoration: 'none',
                padding: '0.5rem 1rem',
                borderRadius: '6px',
                backgroundColor: '#007bff',
                transition: 'background-color 0.2s',
                fontWeight: '500'
              }}
              onMouseEnter={(e) => e.target.style.backgroundColor = '#0056b3'}
              onMouseLeave={(e) => e.target.style.backgroundColor = '#007bff'}
            >
              Iniciar Sesión
            </Link>
          </>
        )}
      </div>
    </nav>
  );
};

export default NavBar;