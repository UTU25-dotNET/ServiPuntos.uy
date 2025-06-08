import React, { useState, useEffect } from "react";
import { Link } from "react-router-dom";
import authService from "../../services/authService";
import apiService from "../../services/apiService";

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
        alignItems: "center",
        padding: "1rem 2rem",
        backgroundColor: "#343a40",
        color: "white",
        boxShadow: "0 2px 4px rgba(0,0,0,0.1)"
      }}
    >
      {/* Logo/Brand */}
      <div>
        <Link
          to="/"
          style={{
            color: "white",
            textDecoration: "none",
            fontWeight: "bold",
            fontSize: "1.3rem",
            display: "flex",
            alignItems: "center",
            gap: "0.5rem"
          }}
        >
          <span style={{ fontSize: "1.5rem" }}>⛽</span>
          {isAuthenticated && tenantInfo ? (
            <>
              {tenantInfo.nombre} 
              <span style={{ color: "#adb5bd", fontSize: "0.9rem", fontWeight: "normal" }}>
                Servipuntos
              </span>
            </>
          ) : (
            <>
              Servipuntos.uy 
              <span style={{ color: "#adb5bd", fontWeight: "normal" }}>
                *Demo*
              </span>
            </>
          )}
        </Link>
      </div>

      {/* Navigation Items */}
      <div style={{ display: "flex", alignItems: "center", gap: "1rem" }}>
        {isAuthenticated ? (
          <>
            {/* User Info */}
            {user && (
              <div
                style={{
                  display: "flex",
                  alignItems: "center",
                  gap: "0.5rem",
                  padding: "0.5rem 1rem",
                  backgroundColor: user.role === "admin" ? "#6f42c1" : "#28a745",
                  borderRadius: "20px",
                  fontSize: "0.85rem"
                }}
              >
                <span>👤</span>
                <span>{user.username || user.uniqueName || user.name || user.email}</span>
              </div>
            )}
            
            {/* Navigation Links */}
            <Link 
              to="/estaciones" 
              style={{ 
                color: 'white', 
                textDecoration: 'none',
                padding: '0.5rem 1rem',
                borderRadius: '6px',
                transition: 'background-color 0.2s',
                display: 'flex',
                alignItems: 'center',
                gap: '0.5rem',
                backgroundColor: 'rgba(255,255,255,0.1)',
                border: '1px solid rgba(255,255,255,0.2)',
                fontWeight: '500'
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

            <Link 
              to="/perfil" 
              style={{ 
                color: 'white', 
                textDecoration: 'none',
                padding: '0.5rem 1rem',
                borderRadius: '6px',
                transition: 'background-color 0.2s',
                display: 'flex',
                alignItems: 'center',
                gap: '0.5rem',
                fontWeight: '500'
              }}
              onMouseEnter={(e) => e.target.style.backgroundColor = 'rgba(255,255,255,0.1)'}
              onMouseLeave={(e) => e.target.style.backgroundColor = 'transparent'}
            >
              ⚙️ Mi Perfil
            </Link>

            {/* Logout */}
            <Link
              to="/"
              onClick={() => authService.logout()}
              style={{
                color: "white",
                textDecoration: "none",
                padding: '0.5rem 1rem',
                borderRadius: '6px',
                transition: 'background-color 0.2s',
                backgroundColor: '#dc3545',
                fontWeight: '500',
                display: 'flex',
                alignItems: 'center',
                gap: '0.5rem'
              }}
              onMouseEnter={(e) => e.target.style.backgroundColor = '#c82333'}
              onMouseLeave={(e) => e.target.style.backgroundColor = '#dc3545'}
            >
              🚪 Salir
            </Link>
          </>
        ) : (
          <>
            {/* Login button for non-authenticated users */}
            <Link 
              to="/login" 
              style={{ 
                color: 'white', 
                textDecoration: 'none',
                padding: '0.75rem 1.5rem',
                borderRadius: '6px',
                backgroundColor: '#007bff',
                transition: 'background-color 0.2s',
                fontWeight: '600',
                display: 'flex',
                alignItems: 'center',
                gap: '0.5rem'
              }}
              onMouseEnter={(e) => e.target.style.backgroundColor = '#0056b3'}
              onMouseLeave={(e) => e.target.style.backgroundColor = '#007bff'}
            >
              🔑 Iniciar Sesión
            </Link>
          </>
        )}
      </div>
    </nav>
  );
};

export default NavBar;