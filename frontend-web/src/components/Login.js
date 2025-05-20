import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import authService from "../services/authService";
import { GoogleLogin } from "@react-oauth/google";

const Login = () => {
  const [credentials, setCredentials] = useState({
    email: "",
    password: "",
  });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const navigate = useNavigate();

  // si ya esta logeado va directo al dashboard
  useEffect(() => {
    if (authService.isAuthenticated()) {
      navigate("/dashboard");
    }
  }, [navigate]);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setCredentials((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  // Manejar envío del formulario
  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError("");

    try {
      await authService.login(credentials.email, credentials.password);
      window.location.href = "/dashboard";
    } catch (err) {
      setError(err.message || "Error en el inicio de sesión");
    } finally {
      setLoading(false);
    }
  };
  // Manejar el inicio de sesión con Google
  const handleGoogleLogin = async (credentialResponse) => {
    try {
      setLoading(true);
      setError("");

      const response = await fetch("api/auth/signin", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          googleToken: credentialResponse.credential,
          // provider: 'google',
          // token: credentialResponse.credential
        }),
      });
      if (!response.ok) {
        const errorData = await response.json();
        throw new Error(
          errorData.message || "Error en el inicio de sesión con Google"
        );
      }

      const data = await response.json();

      // Guardo el JWT que devuelve el backend (no el de Google)
      localStorage.setItem("token", data.token);
      window.location.href = "/dashboard";
    } catch (err) {
      setError(err.message || "Error en el inicio de sesión con Google");
    } finally {
      setLoading(false);
    }
  };

  // Usar tokens predefinidos para desarrollo
  const applyTestAccount = (type) => {
    switch (type) {
      case "user":
        setCredentials({
          email: "user@gmail.com",
          password: "user",
        });
        break;
      case "admin":
        setCredentials({
          email: "admin@gmail.com",
          password: "admin",
        });
        break;
      default:
        break;
    }
  };

  // Usar token hardcodeado directamente
  const applyHardcodedToken = (tokenType) => {
    const token = authService.setHardcodedToken(tokenType);
    if (token) {
      navigate("/dashboard");
    } else {
      setError("Error al aplicar token de prueba");
    }
  };

  return (
    <div style={{ maxWidth: "400px", margin: "0 auto", padding: "1rem" }}>
      <h2 style={{ color: "#7B3F00" }}>Servipuntos.uy</h2>
      <h3 style={{ marginBottom: "1rem" }}>Iniciar Sesión</h3>
      <p style={{ marginBottom: "1rem", color: "#6c757d" }}>
        Ingresá para acceder a todas las funcionalidades.
      </p>

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

      <form onSubmit={handleSubmit}>
        <div style={{ marginBottom: "1rem" }}>
          <label
            htmlFor="email"
            style={{ display: "block", marginBottom: "0.5rem" }}
          >
            Email:
          </label>
          <input
            type="email"
            id="email"
            name="email"
            value={credentials.email}
            onChange={handleChange}
            required
            style={{
              width: "100%",
              padding: "0.5rem",
              borderRadius: "4px",
              border: "1px solid #ced4da",
            }}
          />
        </div>

        <div style={{ marginBottom: "1rem" }}>
          <label
            htmlFor="password"
            style={{ display: "block", marginBottom: "0.5rem" }}
          >
            Contraseña:
          </label>
          <input
            type="password"
            id="password"
            name="password"
            value={credentials.password}
            onChange={handleChange}
            required
            style={{
              width: "100%",
              padding: "0.5rem",
              borderRadius: "4px",
              border: "1px solid #ced4da",
            }}
          />
        </div>

        <div>
          <button
            type="submit"
            disabled={loading}
            style={{
              backgroundColor: "#007bff",
              color: "white",
              border: "none",
              borderRadius: "4px",
              padding: "0.5rem 1rem",
              cursor: loading ? "not-allowed" : "pointer",
              opacity: loading ? 0.7 : 1,
            }}
          >
            {loading ? "Iniciando sesión..." : "Iniciar Sesión"}
          </button>
        </div>
        <div style={{ marginTop: "2rem", textAlign: "center" }}>
          <div style={{ marginBottom: "1rem" }}>
            <GoogleLogin
              onSuccess={handleGoogleLogin}
              onError={() => setError("Error al iniciar sesión con Google")}
              text="signin_with"
              shape="rectangular"
              theme="filled_blue"
              useOneTap={false}
            />
          </div>
        </div>
      </form>

      <div style={{ marginTop: "2rem" }}>
        <h4>Cuentas de prueba</h4>
        <div style={{ display: "flex", gap: "0.5rem", marginBottom: "1rem" }}>
          <button
            onClick={() => applyTestAccount("user")}
            style={{
              backgroundColor: "#28a745",
              color: "white",
              border: "none",
              borderRadius: "4px",
              padding: "0.5rem 1rem",
              fontSize: "0.8rem",
              cursor: "pointer",
            }}
          >
            Usuario Externo
          </button>

          <button
            onClick={() => applyTestAccount("admin")}
            style={{
              backgroundColor: "#6f42c1",
              color: "white",
              border: "none",
              borderRadius: "4px",
              padding: "0.5rem 1rem",
              fontSize: "0.8rem",
              cursor: "pointer",
            }}
          >
            Administrador
          </button>
        </div>

        {/* <h4>Tokens hardcodeados (desarrollo)</h4>
        <div style={{ display: 'flex', gap: '0.5rem', flexWrap: 'wrap' }}>
          <button 
            onClick={() => applyHardcodedToken('user')}
            style={{
              backgroundColor: '#17a2b8',
              color: 'white',
              border: 'none',
              borderRadius: '4px',
              padding: '0.5rem 1rem',
              fontSize: '0.8rem',
              cursor: 'pointer'
            }}
          >
            Token Usuario
          </button>
          
          <button 
            onClick={() => applyHardcodedToken('admin')}
            style={{
              backgroundColor: '#20c997',
              color: 'white',
              border: 'none',
              borderRadius: '4px',
              padding: '0.5rem 1rem',
              fontSize: '0.8rem',
              cursor: 'pointer'
            }}
          >
            Token Admin
          </button>
          
          <button 
            onClick={() => applyHardcodedToken('expired')}
            style={{
              backgroundColor: '#dc3545',
              color: 'white',
              border: 'none',
              borderRadius: '4px',
              padding: '0.5rem 1rem',
              fontSize: '0.8rem',
              cursor: 'pointer'
            }}
          >
            Token Expirado
          </button>
        </div> */}
      </div>
    </div>
  );
};

export default Login;
