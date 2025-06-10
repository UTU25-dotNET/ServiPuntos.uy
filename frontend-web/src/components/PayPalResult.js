import React from "react";
import { useSearchParams, useNavigate } from "react-router-dom";

const PayPalResult = () => {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();

  const status = searchParams.get("status") || (window.location.pathname.includes("paypal-cancel") ? "cancel" : "success");
  const paymentId = searchParams.get("paymentId");
  const payerId = searchParams.get("payerId");
  const token = searchParams.get("token");

  const isSuccess = status !== "cancel";

  return (
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
          width: "100%",
          maxWidth: "500px",
          backgroundColor: "white",
          padding: "2rem",
          borderRadius: "16px",
          boxShadow: "0 8px 32px rgba(0,0,0,0.1)",
          border: "1px solid #e9ecef",
          textAlign: "center"
        }}
      >
        <div style={{ fontSize: "3rem", marginBottom: "1rem" }}>
          {isSuccess ? "✅" : "❌"}
        </div>
        <h2
          style={{
            color: isSuccess ? "#28a745" : "#dc3545",
            marginBottom: "1rem"
          }}
        >
          {isSuccess ? "Pago aprobado" : "Pago cancelado"}
        </h2>
        {isSuccess ? (
          <p style={{ marginBottom: "1rem", color: "#6c757d" }}>
            El pago se completó con éxito.
          </p>
        ) : (
          <p style={{ marginBottom: "1rem", color: "#6c757d" }}>
            El pago fue cancelado por el usuario.
          </p>
        )}
        {paymentId && (
          <p style={{ margin: "0.25rem 0", fontSize: "0.9rem", color: "#6c757d" }}>
            <strong>paymentId:</strong> {paymentId}
          </p>
        )}
        {payerId && (
          <p style={{ margin: "0.25rem 0", fontSize: "0.9rem", color: "#6c757d" }}>
            <strong>payerId:</strong> {payerId}
          </p>
        )}
        {token && (
          <p style={{ margin: "0.25rem 0", fontSize: "0.9rem", color: "#6c757d" }}>
            <strong>token:</strong> {token}
          </p>
        )}
        <button
          onClick={() => navigate("/")}
          style={{
            marginTop: "2rem",
            padding: "0.75rem 1.5rem",
            backgroundColor: "#007bff",
            color: "white",
            border: "none",
            borderRadius: "6px",
            cursor: "pointer",
            fontSize: "1rem",
            fontWeight: "600"
          }}
          onMouseEnter={(e) => (e.target.style.backgroundColor = "#0056b3")}
          onMouseLeave={(e) => (e.target.style.backgroundColor = "#007bff")}
        >
          Volver al inicio
        </button>
      </div>
    </div>
  );
};

export default PayPalResult;