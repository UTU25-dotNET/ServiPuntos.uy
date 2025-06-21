import React, { useState, useEffect } from "react";
import { Link } from "react-router-dom";
import authService from "../../services/authService";
import apiService from "../../services/apiService";
import NotificationsBell from "./NotificationsBell";

const NavBar = () => {
  const isAuthenticated = authService.isAuthenticated();
  const user = authService.getCurrentUser();
  const [tenantInfo, setTenantInfo] = useState(null);


    // Calcular color de contraste para un fondo dado
    const getContrastColor = (hex) => {
      if (!hex) return "#ffffff";
      const clean = hex.replace("#", "");
      const r = parseInt(clean.substr(0, 2), 16);
      const g = parseInt(clean.substr(2, 2), 16);
      const b = parseInt(clean.substr(4, 2), 16);
      const luminance = (0.299 * r + 0.587 * g + 0.114 * b) / 255;
      return luminance > 0.5 ? "#000000" : "#ffffff";
    };
  
  // Cargar información del tenant si el usuario está autenticado
  useEffect(() => {
    const loadTenantInfo = async () => {
      if (isAuthenticated) {
        try {
          const tenant = await apiService.getTenantInfo();
          setTenantInfo(tenant);
        } catch (err) {
          // No mostrar error en la UI, mantener el nombre por defecto
        }
      }
    };

    loadTenantInfo();
  }, [isAuthenticated]);

  // Aplicar color principal del tenant al tema
  useEffect(() => {
    const color = isAuthenticated && tenantInfo?.color ? tenantInfo.color : "#7B3F00";
    document.documentElement.style.setProperty("--primary-color", color);
    const metaTheme = document.querySelector('meta[name="theme-color"]');
    if (metaTheme) metaTheme.setAttribute('content', color);
  }, [isAuthenticated, tenantInfo]);

  useEffect(() => {
    if (tenantInfo?.nombre) {
      document.title = `${tenantInfo.nombre} - Servipuntos`;
    } else {
      document.title = "Servipuntos";
    }
  }, [tenantInfo]);

  const textColor = getContrastColor(tenantInfo?.color || "#343a40");


  return (
    <nav
      style={{
        display: "flex",
        justifyContent: "space-between",
        alignItems: "center",
        padding: "1rem 2rem",
        backgroundColor: tenantInfo?.color || "#343a40",
        color: textColor,
        boxShadow: "0 2px 4px rgba(0,0,0,0.1)"
      }}
    >
      {/* Logo/Brand */}
      <div>
        <Link
          to="/"
          style={{
            color: textColor,
            textDecoration: "none",
            fontWeight: "bold",
            fontSize: "1.3rem",
            display: "flex",
            alignItems: "center",
            gap: "0.5rem"
          }}
        >
          {tenantInfo?.logoUrl ? (
            <img src={tenantInfo.logoUrl} alt="Logo" style={{ height: "40px" }} />
          ) : (
            <span role="img" aria-label="Estación" style={{ fontSize: "1.5rem" }}>⛽</span>
          )}
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
            
            <NotificationsBell textColor={textColor} user={user} />
            <Link
              to="/perfil"
              style={{ 
                color: textColor,
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
                color: textColor,
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
                color: textColor,
                textDecoration: 'none',
                padding: '0.75rem 1.5rem',
                borderRadius: '6px',
                backgroundColor: 'var(--primary-color)',
                transition: 'opacity 0.2s',
                fontWeight: '600',
                display: 'flex',
                alignItems: 'center',
                gap: '0.5rem'
              }}
              onMouseEnter={(e) => e.target.style.opacity = '0.85'}
              onMouseLeave={(e) => e.target.style.opacity = '1'}
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