import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import authService from "../services/authService";
import apiService from "../services/apiService";

const Dashboard = () => {
  const [user, setUser] = useState(null);
  const [profile, setProfile] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const navigate = useNavigate();

  // Cargar datos del usuario
  useEffect(() => {
    const loadUserData = async () => {
      setLoading(true);

      try {
        // Si no está logueado va al login
        if (!authService.isAuthenticated()) {
          navigate("/login");
          return;
        }

        // Obtener datos del usuario desde el token
        const userData = authService.getCurrentUser();
        console.log("Datos del usuario desde el token:", userData);
        setUser(userData);

        // Intentar obtener perfil del usuario
        try {
          const userProfile = await apiService.getUserProfile();
          console.log("Perfil obtenido del API:", userProfile);
          setProfile(userProfile);
        } catch (profileError) {
          console.warn("Error al cargar perfil completo, usando datos del token:", profileError);
          // Si falla el API, usar datos del token como perfil básico
          setProfile({
            id: userData.id || userData.sub || "token-user",
            nombre: userData.name || userData.nombre || "Usuario",
            email: userData.email || "",
            role: userData.role || userData.rol || "usuario",
            puntos: userData.puntos || 0,
            isAdult: userData.is_adult === "true"
          });
          setError("Algunos datos del perfil podrían no estar actualizados");
        }

      } catch (err) {
        console.error("Error en loadUserData:", err);
        setError(err.message || "Error al cargar los datos del usuario");
      } finally {
        setLoading(false);
      }
    };

    loadUserData();
  }, [navigate]);

  // Cerrar sesión
  const handleLogout = () => {
    authService.logout();
    navigate("/login");
  };

  // Ver token actual
  const handleViewToken = () => {
    const token = authService.getToken();
    navigate(`/token?token=${encodeURIComponent(token)}`);
  };

  if (loading) {
    return (
      <div style={{ textAlign: "center", padding: "2rem" }}>
        <div style={{
          border: "4px solid #f3f3f3",
          borderTop: "4px solid #3498db",
          borderRadius: "50%",
          width: "40px",
          height: "40px",
          animation: "spin 2s linear infinite",
          margin: "0 auto 1rem"
        }} />
        <p>Cargando dashboard...</p>
        
        <style jsx>{`
          @keyframes spin {
            0% { transform: rotate(0deg); }
            100% { transform: rotate(360deg); }
          }
        `}</style>
      </div>
    );
  }

  return (
    <div style={{ maxWidth: "800px", margin: "0 auto", padding: "1rem" }}>
      <h2>Dashboard</h2>

      {error && (
        <div
          style={{
            backgroundColor: "#fff3cd",
            color: "#856404",
            padding: "0.75rem",
            borderRadius: "4px",
            marginBottom: "1rem",
            border: "1px solid #ffeaa7"
          }}
        >
          ⚠️ {error}
        </div>
      )}

      {/* Información del JWT Token */}
      {user && (
        <div
          style={{
            backgroundColor: "#f8f9fa",
            padding: "1rem",
            borderRadius: "4px",
            marginBottom: "1rem",
          }}
        >
          <h3>Información del JWT</h3>
          <div
            style={{
              display: "grid",
              gridTemplateColumns: "1fr 3fr",
              rowGap: "0.5rem",
              columnGap: "1rem",
            }}
          >
            <div><strong>ID:</strong></div>
            <div>{user.id || user.sub || "No disponible"}</div>

            <div><strong>Nombre:</strong></div>
            <div>{user.name || user.nombre || "No disponible"}</div>

            <div><strong>Email:</strong></div>
            <div>{user.email || "No disponible"}</div>

            <div><strong>Rol:</strong></div>
            <div>{user.role || user.rol || "No disponible"}</div>

            {user.ci && (
              <>
                <div><strong>CI:</strong></div>
                <div>{user.ci}</div>
              </>
            )}

            {user.tenantId && (
              <>
                <div><strong>Tenant ID:</strong></div>
                <div>{user.tenantId}</div>
              </>
            )}

            {user.exp && (
              <>
                <div><strong>Expiración:</strong></div>
                <div>{new Date(user.exp * 1000).toLocaleString()}</div>
              </>
            )}
          </div>
        </div>
      )}

      {/* Información del Perfil (API) */}
      {profile && (
        <div
          style={{
            backgroundColor: "#e2f0ff",
            padding: "1rem",
            borderRadius: "4px",
            marginBottom: "1rem",
          }}
        >
          <h3>Perfil de Usuario</h3>
          <div
            style={{
              display: "grid",
              gridTemplateColumns: "1fr 3fr",
              rowGap: "0.5rem",
              columnGap: "1rem",
            }}
          >
            <div><strong>ID:</strong></div>
            <div>{profile.id}</div>

            <div><strong>Nombre:</strong></div>
            <div>{profile.nombre}</div>

            <div><strong>Email:</strong></div>
            <div>{profile.email}</div>

            <div><strong>Rol:</strong></div>
            <div>
              <span style={{
                backgroundColor: profile.role === "admin" ? "#6f42c1" : "#28a745",
                color: "white",
                padding: "0.25rem 0.5rem",
                borderRadius: "3px",
                fontSize: "0.8rem"
              }}>
                {profile.role}
              </span>
            </div>

            {profile.ci && (
              <>
                <div><strong>CI:</strong></div>
                <div>{profile.ci}</div>
              </>
            )}

            <div><strong>Puntos:</strong></div>
            <div>{profile.puntos || 0}</div>

            {profile.tenantId && (
              <>
                <div><strong>Tenant:</strong></div>
                <div>{profile.tenantId}</div>
              </>
            )}

            {profile.fechaCreacion && (
              <>
                <div><strong>Fecha de Registro:</strong></div>
                <div>{new Date(profile.fechaCreacion).toLocaleDateString()}</div>
              </>
            )}
          </div>
        </div>
      )}

      {/* Verificación de Edad */}
      {user && (
        <div
          style={{
            padding: "0.75rem",
            borderRadius: "4px",
            marginBottom: "1rem",
            backgroundColor: user.is_adult === "true" ? "#d4edda" : "#f8d7da",
            color: user.is_adult === "true" ? "#155724" : "#721c24",
            fontWeight: "bold"
          }}
        >
          {user.is_adult === "true"
            ? "✅ Identidad Verificada, el usuario es mayor de edad"
            : "⚠️ Identidad Verificada, el usuario es menor de edad"}
        </div>
      )}

      {/* Panel de Administración */}
      {(user?.role === "admin" || profile?.role === "admin") && (
        <div
          style={{
            backgroundColor: "#f0f9e8",
            padding: "1rem",
            borderRadius: "4px",
            marginBottom: "1rem",
            border: "1px solid #c3e6cb"
          }}
        >
          <h3>🛠️ Panel de Administración</h3>
          <p>Esta sección solo es visible para administradores.</p>
          <div style={{ marginTop: "0.5rem" }}>
            <small>
              • Gestión de usuarios<br/>
              • Configuración del sistema<br/>
              • Reportes y estadísticas
            </small>
          </div>
        </div>
      )}

      {/* Botones de acción */}
      <div style={{ display: "flex", gap: "0.5rem", marginTop: "1rem", flexWrap: "wrap" }}>
        <button
          onClick={() => navigate("/perfil")}
          style={{
            backgroundColor: "#28a745",
            color: "white",
            border: "none",
            borderRadius: "4px",
            padding: "0.5rem 1rem",
            cursor: "pointer",
          }}
        >
          👤 Ver Perfil
        </button>

        <button
          onClick={handleViewToken}
          style={{
            backgroundColor: "#17a2b8",
            color: "white",
            border: "none",
            borderRadius: "4px",
            padding: "0.5rem 1rem",
            cursor: "pointer",
          }}
        >
          🔑 Ver Token
        </button>

        <button
          onClick={handleLogout}
          style={{
            backgroundColor: "#dc3545",
            color: "white",
            border: "none",
            borderRadius: "4px",
            padding: "0.5rem 1rem",
            cursor: "pointer",
          }}
        >
          🚪 Cerrar Sesión
        </button>
      </div>
    </div>
  );
};

export default Dashboard;