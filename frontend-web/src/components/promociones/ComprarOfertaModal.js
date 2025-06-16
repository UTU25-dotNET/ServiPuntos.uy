import React, { useEffect, useState } from "react";
import apiService from "../../services/apiService";
import { parseDecimal } from "../../utils/numberUtils";

const ComprarOfertaModal = ({ isOpen, onClose, oferta }) => {
  const [producto, setProducto] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [compraLoading, setCompraLoading] = useState(false);
  const [compraError, setCompraError] = useState("");

  useEffect(() => {
    const loadProducto = async () => {
      if (!isOpen || !oferta) return;
      const prodId = oferta.productoIds?.[0];
      if (!prodId) return;
      setLoading(true);
      setError("");
      try {
        const prod = await apiService.getProductoCanjeable(prodId);
        console.log("Producto cargado", prod);
        setProducto(prod);
      } catch (err) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    };
    loadProducto();
  }, [isOpen, oferta]);

  if (!isOpen || !oferta) return null;

  console.log("Oferta recibida", oferta);

  const precioBase = parseDecimal(oferta.precioEnPesos);
  const descuento = parseDecimal(oferta.descuentoEnPesos);
  const precioFinal = Math.max(precioBase - descuento, 0);
  const porcentaje = precioBase > 0 ? Math.round((descuento / precioBase) * 100) : 0;

  console.log("Calculos", { precioBase, descuento, precioFinal, porcentaje });

  const handlePagar = async () => {
    if (!oferta) return;
    setCompraLoading(true);
    setCompraError("");
    try {
      const ubicacionId = oferta.ubicaciones?.[0];
      if (!ubicacionId) throw new Error("Ubicaci\u00f3n no disponible");
      const productoUbicacion = {
        id: oferta.id,
        productoCanjeable: {
          id: oferta.productoIds?.[0] || oferta.id,
          nombre: oferta.titulo,
        },
        categoria: "Promocion",
        precio: precioFinal,
      };
      const result = await apiService.procesarTransaccion(
        productoUbicacion,
        ubicacionId,
        0,
        0
      );
      if (result?.codigo === "PENDING_PAYMENT" && result.datos?.approvalUrl) {
        window.location.href = result.datos.approvalUrl;
      } else if (result?.codigo !== "OK") {
        setCompraError(result?.mensaje || "Error en la compra");
      }
    } catch (err) {
      setCompraError(err.message);
    } finally {
      setCompraLoading(false);
    }
  };

  const closeModal = () => {
    if (!compraLoading) onClose();
  };

  return (
    <div
      className="modal show d-block"
      tabIndex="-1"
      style={{ backgroundColor: "rgba(0,0,0,0.5)" }}
      onClick={closeModal}
    >
      <div className="modal-dialog" onClick={(e) => e.stopPropagation()}>
        <div className="modal-content">
          <div className="modal-header">
            <h5 className="modal-title">{oferta.titulo}</h5>
            <button type="button" className="btn-close" onClick={closeModal}></button>
          </div>
          <div className="modal-body">
            {loading ? (
              <p>Cargando...</p>
            ) : error ? (
              <p className="text-danger">{error}</p>
            ) : (
              <>
                {producto && (
                  <div className="text-center mb-3">
                    <img
                      src={producto.fotoUrl || "placeholder-product.png"}
                      alt={producto.nombre}
                      className="img-fluid"
                      style={{ maxHeight: "200px", objectFit: "cover" }}
                    />
                  </div>
                )}
                {producto && producto.descripcion && <p>{producto.descripcion}</p>}
                <p>Precio: ${precioBase.toFixed(2)}</p>
                <p>Descuento: {porcentaje}% (${descuento.toFixed(2)})</p>
                <p>
                  <strong>Precio final: ${precioFinal.toFixed(2)}</strong>
                </p>
              </>
            )}
            {compraError && <p className="text-danger">{compraError}</p>}
          </div>
          <div className="modal-footer">
            <button
              type="button"
              className="btn btn-outline-secondary"
              onClick={closeModal}
              disabled={compraLoading}
            >
              Cancelar
            </button>
            <button
              type="button"
              className="btn btn-success"
              onClick={handlePagar}
              disabled={compraLoading || loading}
            >
              {compraLoading ? "Procesando..." : "Pagar con PayPal"}
            </button>
          </div>
        </div>
      </div>
    </div>
  );
};

export default ComprarOfertaModal;
