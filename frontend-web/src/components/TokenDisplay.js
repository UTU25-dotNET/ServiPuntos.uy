import React, { useState, useEffect } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import Breadcrumb from "./layout/Breadcrumb";
import tokenUtils from "../utils/tokenUtils";

const TokenDisplay = () => {
  const [token, setToken] = useState("");
  const [decodedToken, setDecodedToken] = useState(null);
  const [error, setError] = useState("");
  const location = useLocation();
  const navigate = useNavigate();

  // Función para decodificar token - definida fuera de otros hooks
  function handleTokenDecode(tokenToUse) {
    try {
      setError("");

      if (!tokenToUse || !tokenToUse.trim()) {
        setError("Por favor ingresa un token");
        setDecodedToken(null);
        return;
      }

      const decoded = tokenUtils.decodeToken(tokenToUse);

      if (!decoded) {
        setError("Token inválido o mal formado");
        setDecodedToken(null);
        return;
      }

      setDecodedToken(decoded);
    } catch (err) {
      setError(`Error al decodificar: ${err.message}`);
      setDecodedToken(null);
    }
  }

  // Procesar cambios en el token de entrada
  useEffect(() => {
    if (token) {
      handleTokenDecode(token);
    }
  }, [token]);

  // Cargar token de URL al iniciar
  useEffect(() => {
    const params = new URLSearchParams(location.search);
    const tokenParam = params.get("token");

    if (tokenParam) {
      setToken(tokenParam);
    }
  }, [location]);

  // Volver al dashboard
  const handleBack = () => {
    navigate("/dashboard");
  };

  // Copiar al portapapeles
  const copyToClipboard = () => {
    navigator.clipboard
      .writeText(token)
      .then(() => {
        alert("Token copiado al portapapeles");
      })
      .catch((err) => {
      });
  };

  return (
    <div style={{ maxWidth: "800px", margin: "0 auto", padding: "1rem" }}>
      <Breadcrumb current="Token" />
      <h2>Visualizador de JWT</h2>

      <div style={{ marginBottom: "1rem" }}>
        <textarea
          value={token}
          onChange={(e) => setToken(e.target.value)}
          placeholder="Ingresa el token JWT aquí..."
          style={{
            width: "100%",
            minHeight: "100px",
            padding: "0.5rem",
            fontFamily: "monospace",
          }}
        />
      </div>

      <div style={{ marginBottom: "1rem" }}>
        <button
          onClick={() => handleTokenDecode(token)}
          style={{
            backgroundColor: "#007bff",
            color: "white",
            border: "none",
            borderRadius: "4px",
            padding: "0.5rem 1rem",
            marginRight: "0.5rem",
            cursor: "pointer",
          }}
        >
          Decodificar
        </button>

        <button
          onClick={copyToClipboard}
          style={{
            backgroundColor: "#6c757d",
            color: "white",
            border: "none",
            borderRadius: "4px",
            padding: "0.5rem 1rem",
            marginRight: "0.5rem",
            cursor: "pointer",
          }}
        >
          Copiar Token
        </button>

        <button
          onClick={handleBack}
          style={{
            backgroundColor: "#17a2b8",
            color: "white",
            border: "none",
            borderRadius: "4px",
            padding: "0.5rem 1rem",
            cursor: "pointer",
          }}
        >
          Volver
        </button>
      </div>

      {/* El resto del componente (visualización del token) queda igual */}
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

      {decodedToken && (
        <div>
          <h3>Token Decodificado</h3>

          <div style={{ display: "flex", gap: "1rem", flexWrap: "wrap" }}>
            <div style={{ flex: 1, minWidth: "300px" }}>
              <h4>Header</h4>
              <pre
                style={{
                  backgroundColor: "#f8f9fa",
                  padding: "1rem",
                  borderRadius: "4px",
                  overflow: "auto",
                }}
              >
                {JSON.stringify(decodedToken.header, null, 2)}
              </pre>
            </div>

            <div style={{ flex: 1, minWidth: "300px" }}>
              <h4>Payload</h4>
              <pre
                style={{
                  backgroundColor: "#f8f9fa",
                  padding: "1rem",
                  borderRadius: "4px",
                  overflow: "auto",
                }}
              >
                {JSON.stringify(decodedToken.payload, null, 2)}
              </pre>
            </div>
          </div>

          <div style={{ marginTop: "1rem" }}>
            <h4>Firma (Base64 URL)</h4>
            <pre
              style={{
                backgroundColor: "#f8f9fa",
                padding: "1rem",
                borderRadius: "4px",
                overflow: "auto",
                wordBreak: "break-all",
              }}
            >
              {decodedToken.signature}
            </pre>
            <p>
              <small>
                Nota: La firma solo puede ser verificada en el backend con la
                clave secreta.
              </small>
            </p>
          </div>

          {decodedToken.payload.exp && (
            <div
              style={{
                marginTop: "1rem",
                backgroundColor:
                  decodedToken.payload.exp * 1000 < Date.now()
                    ? "#f8d7da"
                    : "#d4edda",
                color:
                  decodedToken.payload.exp * 1000 < Date.now()
                    ? "#721c24"
                    : "#155724",
                padding: "0.75rem",
                borderRadius: "4px",
              }}
            >
              <h4>Información de Expiración</h4>
              <p>
                <strong>Expira el:</strong>{" "}
                {new Date(decodedToken.payload.exp * 1000).toLocaleString()}
              </p>
              <p>
                <strong>Estado:</strong>{" "}
                {decodedToken.payload.exp * 1000 > Date.now()
                  ? "✅ Válido"
                  : "❌ Expirado"}
              </p>
              {decodedToken.payload.exp * 1000 > Date.now() && (
                <p>
                  <strong>Tiempo restante:</strong>{" "}
                  {Math.floor(
                    (decodedToken.payload.exp * 1000 - Date.now()) / 1000
                  )}{" "}
                  segundos
                </p>
              )}
            </div>
          )}
        </div>
      )}
    </div>
  );
};

export default TokenDisplay;
