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
    <div style={{ maxWidth: "600px", margin: "2rem auto", padding: "2rem", textAlign: "center", backgroundColor: "#f8f9fa", borderRadius: "8px", border: "1px solid #dee2e6" }}>
      <h2 style={{ color: isSuccess ? "#28a745" : "#dc3545", marginBottom: "1rem" }}>
        {isSuccess ? "Pago aprobado" : "Pago cancelado"}
      </h2>
      {isSuccess ? (
        <p>El pago se completó con éxito.</p>
      ) : (
        <p>El pago fue cancelado por el usuario.</p>
      )}
      {paymentId && (
        <p><strong>paymentId:</strong> {paymentId}</p>
      )}
      {payerId && (
        <p><strong>payerId:</strong> {payerId}</p>
      )}
      {token && (
        <p><strong>token:</strong> {token}</p>
      )}
      <button onClick={() => navigate("/")} style={{ marginTop: "1rem" }}>
        Volver al inicio
      </button>
    </div>
  );
};

export default PayPalResult;