import React, { useEffect, useState } from "react";
import { useSearchParams, useNavigate } from "react-router-dom";
import Breadcrumb from "./layout/Breadcrumb";
import apiService from "../services/apiService";

const PayPalResult = () => {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const [transaccion, setTransaccion] = useState(null);
  const [productosInfo, setProductosInfo] = useState([]);

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

  useEffect(() => {
    const cargarProductos = async () => {
      if (!isSuccess || !transaccion || !transaccion.detalles) return;
      let arr = [];
      try {
        const obj = JSON.parse(transaccion.detalles);
        if (Array.isArray(obj)) arr = obj;
        else if (obj.productos) arr = obj.productos;
        else if (obj.Productos) arr = obj.Productos;
      } catch {
        arr = [];
      }
      const grouped = Object.values(
        arr.reduce((acc, item) => {
          const id = item.idProducto || item.IdProducto;
          const nombre = item.nombreProducto || item.NombreProducto;
          const cantidad = item.cantidad || item.Cantidad || 0;
          if (!acc[id]) acc[id] = { id, nombre, cantidad };
          else acc[id].cantidad += cantidad;
          return acc;
        }, {})
      );
      try {
        const detalles = await Promise.all(
          grouped.map(async (p) => {
            try {
              const info = await apiService.getProductoCanjeable(p.id);
              return { ...p, ...info };
            } catch {
              return p;
            }
          })
        );
        setProductosInfo(detalles);
      } catch {
        setProductosInfo(grouped);
      }
    };
    cargarProductos();
  }, [isSuccess, transaccion]);

  return (
    <div className="container py-5">
      <div className="row justify-content-center">
        <div className="col-lg-6">
          <Breadcrumb current="Pago" />
          <div className="bg-white p-4 rounded shadow text-center">
            <div className="display-4 mb-3">
              {isSuccess ? "✅" : "❌"}
            </div>
            <h2 className={isSuccess ? "text-success" : "text-danger"}>
              {isSuccess ? "Pago aprobado" : "Pago cancelado"}
            </h2>
            <p className="text-muted">
              {isSuccess ? "El pago se completó con éxito." : "El pago fue cancelado por el usuario."}
            </p>
            {paymentId && (
              <p className="small text-muted mb-0">
                <strong>paymentId:</strong> {paymentId}
              </p>
            )}
            {payerId && (
              <p className="small text-muted mb-0">
                <strong>payerId:</strong> {payerId}
              </p>
            )}
            {token && (
              <p className="small text-muted mb-0">
                <strong>token:</strong> {token}
              </p>
            )}
            {isSuccess && transaccion && (
              <div className="text-start mt-3">
                <p className="mb-1">
                  <strong>Monto pagado:</strong> ${" "}
                  {transaccion.montoPagado ?? transaccion.monto}
                </p>
                {transaccion.esTransaccionMixta && (
                  <p className="mb-1 text-muted">
                    <small>
                      Total: ${" "}
                      {transaccion.monto} - Puntos utilizados: {" "}
                      {transaccion.puntosUtilizados}
                    </small>
                  </p>
                )}
                <p className="mb-3">
                  <strong>Fecha:</strong> {new Date(transaccion.fecha).toLocaleString()}
                </p>
                {productosInfo.length > 0 && (
                  <div className="mt-3">
                    <h5 className="mb-3">Productos</h5>
                    <ul className="list-group list-group-flush">
                      {productosInfo.map((p, idx) => (
                        <li
                          key={idx}
                          className="list-group-item d-flex align-items-start"
                        >
                          {p.fotoUrl && (
                            <img
                              src={p.fotoUrl}
                              alt={p.nombre || p.nombreProducto}
                              className="rounded me-3"
                              style={{ width: "60px", height: "60px", objectFit: "cover" }}
                            />
                          )}
                          <div className="flex-grow-1">
                            <div className="fw-bold">
                              {p.nombre || p.nombreProducto}
                            </div>
                            {p.descripcion && (
                              <small className="text-muted">{p.descripcion}</small>
                            )}
                          </div>
                          <span className="badge bg-primary rounded-pill ms-3">
                            x{p.cantidad}
                          </span>
                        </li>
                      ))}
                    </ul>
                  </div>
                )}
              </div>
            )}
            <button className="btn btn-primary mt-4" onClick={() => navigate("/")}>Volver al inicio</button>
          </div>
        
        </div>
       
      </div>
    </div>
  );
};

export default PayPalResult;