import React, { useEffect, useState } from "react";
import { useSearchParams, useNavigate } from "react-router-dom";
import Breadcrumb from "./layout/Breadcrumb";
import apiService from "../services/apiService";

const PayPalResult = () => {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const [transaccion, setTransaccion] = useState(null);

  const status =
    searchParams.get("status") ||
    (window.location.pathname.includes("paypal-cancel") ? "cancel" : "success");
  const paymentId = searchParams.get("paymentId");
  const payerId = searchParams.get("payerId") || searchParams.get("PayerID");
  const token = searchParams.get("token");
  const transaccionId = searchParams.get("transaccionId");

  const isSuccess = status !== "cancel";

  useEffect(() => {
    if (isSuccess && transaccionId) {
      apiService
        .getTransaccionById(transaccionId)
        .then((data) => setTransaccion(data))
        .catch(() => {});
    }
  }, [isSuccess, transaccionId]);

  let productos = [];
  if (transaccion && transaccion.detalles) {
    try {
      const obj = JSON.parse(transaccion.detalles);
      if (Array.isArray(obj)) {
        productos = obj;
      } else if (obj.productos) {
        productos = obj.productos;
      }
    } catch {
      productos = [];
    }
  }

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
        <Breadcrumb current="Pago" />
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
         {isSuccess && transaccion && (
          <div style={{ marginTop: "1rem", textAlign: "left" }}>
            <p style={{ margin: "0.25rem 0", fontSize: "0.9rem" }}>
              <strong>Monto:</strong> ${" "}{transaccion.monto}
            </p>
            <p style={{ margin: "0.25rem 0", fontSize: "0.9rem" }}>
              <strong>Fecha:</strong> {new Date(transaccion.fecha).toLocaleString()}
            </p>
            {productos.length > 0 && (
              <div>
                <strong>Productos:</strong>
                <ul style={{ paddingLeft: "1.2rem" }}>
                  {Object.values(
                    productos.reduce((acc, item) => {
                      const key = item.nombreProducto || item.NombreProducto;
                      if (!acc[key]) {
                        acc[key] = { ...item };
                      } else {
                        const cant = item.cantidad || item.Cantidad || 0;
                        acc[key].Cantidad = (acc[key].Cantidad || acc[key].cantidad || 0) + cant;
                      }
                      return acc;
                    }, {})
                  ).map((p, idx) => (
                    <li key={idx} style={{ fontSize: "0.9rem" }}>
                      {p.nombreProducto || p.NombreProducto} x {p.Cantidad || p.cantidad}
                    </li>
                  ))}
                </ul>
              </div>
            )}
          </div>
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