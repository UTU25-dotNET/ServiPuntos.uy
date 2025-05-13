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
        // si no esta logeado va a l login
        if (!authService.isAuthenticated()) {
          navigate("/login");
          return;
        }

        // Obtener datos del usuario desde el token
        const userData = authService.getCurrentUser();
        setUser(userData);

        // Obtener perfil del usuario (simulado)
        const userProfile = await apiService.getUserProfile();
        setProfile(userProfile);
      } catch (err) {
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
      <div style={{ textAlign: "center", padding: "2rem" }}>Cargando...</div>
    );
  }

  return (
    <div style={{ maxWidth: "800px", margin: "0 auto", padding: "1rem" }}>
      <h2>Dashboard</h2>

      {error && (
        <div
          style={{
            backgroundColor: "#f8d7da",
            color: "#721c24",
            padding: "0.75rem",
            borderRadius: "4px",
            marginBottom: "1rem",
          }}
        >
          {error}
        </div>
      )}

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
            <div>
              <strong>ID:</strong>
            </div>
            <div>{user.id}</div>

            <div>
              <strong>Usuario:</strong>
            </div>
            <div>{user.username}</div>

            <div>
              <strong>Email:</strong>
            </div>
            <div>{user.email}</div>

            <div>
              <strong>Rol:</strong>
            </div>
            <div>{user.role}</div>

            {user.exp && (
              <>
                <div>
                  <strong>Expiración:</strong>
                </div>
                <div>{new Date(user.exp * 1000).toLocaleString()}</div>
              </>
            )}
          </div>
        </div>
      )}

      {profile && (
        <div
          style={{
            backgroundColor: "#e2f0ff",
            padding: "1rem",
            borderRadius: "4px",
            marginBottom: "1rem",
          }}
        >
          <h3>Perfil de Usuario (API)</h3>
          <p>
            <small>Simulado para desarrollo</small>
          </p>
          <div
            style={{
              display: "grid",
              gridTemplateColumns: "1fr 3fr",
              rowGap: "0.5rem",
              columnGap: "1rem",
            }}
          >
            {Object.entries(profile).map(([key, value]) => (
              <React.Fragment key={key}>
                <div>
                  <strong>{key}:</strong>
                </div>
                <div>
                  {typeof value === "object" ? JSON.stringify(value) : value}
                </div>
              </React.Fragment>
            ))}
          </div>
        </div>
      )}

      {user?.role === "admin" && (
        <div
          style={{
            backgroundColor: "#f0f9e8",
            padding: "1rem",
            borderRadius: "4px",
            marginBottom: "1rem",
          }}
        >
          <h3>Panel de Administración</h3>
          <p>Esta sección solo es visible para administradores.</p>
        </div>
      )}

      <div style={{ display: "flex", gap: "0.5rem", marginTop: "1rem" }}>
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
          Ver Token
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
          Cerrar Sesión
        </button>
      </div>
    </div>
  );
};

export default Dashboard;
