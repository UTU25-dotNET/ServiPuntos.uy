import React, { useEffect, useState } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import authService from "../../services/authService";
import Breadcrumb from "../layout/Breadcrumb";

const AuthCallback = () => {
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();

  useEffect(() => {
    const handleAuthCallback = async () => {
      try {
        // Obtener parámetros de la URL
        const token = searchParams.get("token");
        const errorParam = searchParams.get("error");

        // Si hay un error en los parámetros
        if (errorParam) {
          setError(decodeURIComponent(errorParam));
          setLoading(false);
          return;
        }

        // Si no hay token, redirigir al login
        if (!token) {
          setError("No se pudo completar la autenticación. Falta el token.");
          setTimeout(() => navigate("/login"), 3000);
          return;
        }

        // Guardar el token y configurar cierre automático
        authService.loginWithToken(token);

        // Verificar que el token sea válido
        if (!authService.isAuthenticated()) {
          throw new Error("El token recibido no es válido");
        }


        // Redirigir al dashboard
        window.location.href = "/";

      } catch (err) {
        setError(err.message || "Error al procesar la autenticación");
        setLoading(false);
      }
    };

    handleAuthCallback();
  }, [navigate, searchParams]);

  if (loading) {
    return (
      <div style={{
        display: "flex",
        flexDirection: "column",
        alignItems: "center",
        justifyContent: "center",
        minHeight: "50vh",
        textAlign: "center"
      }}>
        <div style={{
          border: "4px solid #f3f3f3",
          borderTop: "4px solid #3498db",
          borderRadius: "50%",
          width: "40px",
          height: "40px",
          animation: "spin 2s linear infinite",
          marginBottom: "1rem"
        }} />
        <h3>Completando autenticación...</h3>
        <p>Por favor espera mientras procesamos tu inicio de sesión.</p>
        
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
    <div style={{
      maxWidth: "500px",
      margin: "2rem auto",
      padding: "2rem",
      textAlign: "center",
      backgroundColor: "#f8f9fa",
      borderRadius: "8px",
      border: "1px solid #dee2e6"
    }}>
      <Breadcrumb current="Callback" />
      <h2 style={{ color: "#dc3545", marginBottom: "1rem" }}>
        Error de Autenticación
      </h2>
      
      <div style={{
        backgroundColor: "#f8d7da",
        color: "#721c24",
        padding: "1rem",
        borderRadius: "4px",
        marginBottom: "1.5rem",
        border: "1px solid #f5c6cb"
      }}>
        {error}
      </div>

      <div style={{ display: "flex", gap: "1rem", justifyContent: "center" }}>
        <button
          onClick={() => navigate("/login")}
          style={{
            backgroundColor: "#007bff",
            color: "white",
            border: "none",
            borderRadius: "4px",
            padding: "0.75rem 1.5rem",
            cursor: "pointer",
            fontSize: "1rem"
          }}
        >
          Volver al Login
        </button>
        
        <button
          onClick={() => navigate("/")}
          style={{
            backgroundColor: "#6c757d",
            color: "white",
            border: "none",
            borderRadius: "4px",
            padding: "0.75rem 1.5rem",
            cursor: "pointer",
            fontSize: "1rem"
          }}
        >
          Ir al Inicio
        </button>
      </div>
    </div>
  );
};

export default AuthCallback;