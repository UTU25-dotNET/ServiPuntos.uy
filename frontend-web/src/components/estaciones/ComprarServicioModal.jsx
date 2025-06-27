import React, { useState, useEffect } from "react";
import apiService from "../../services/apiService";
import SeleccionarPuntosModal from "../productos/SeleccionarPuntosModal";

const ComprarServicioModal = ({
  isOpen,
  onClose,
  ubicacion,
  servicio,
  precio,
  userProfile,
}) => {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [tenantInfo, setTenantInfo] = useState(null);
  const [mostrarPuntosModal, setMostrarPuntosModal] = useState(false);
  const [maxPuntos, setMaxPuntos] = useState(0);

  useEffect(() => {
    if (isOpen) {
      apiService.getTenantInfo().then(setTenantInfo).catch(() => {});
      setError("");
    }
  }, [isOpen]);

  const valorPunto = tenantInfo?.valorPunto || 0;

  const handleComprar = async (puntos = 0) => {
    if (!ubicacion) return;
    setLoading(true);
    setError("");
    try {
      const producto = {
        productoCanjeable: { id: crypto.randomUUID(), nombre: servicio },
        categoria: "servicio",
        precio: precio,
      };
      const result = await apiService.procesarTransaccion(
        producto,
        ubicacion.id,
        puntos,
        valorPunto,
        3
      );
      if (result?.codigo === "PENDING_PAYMENT" && result.datos?.approvalUrl) {
        window.location.href = result.datos.approvalUrl;
      } else if (result?.codigo !== "OK" && result?.mensaje) {
        setError(result.mensaje);
      } else {
        onClose();
      }
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  const handleComprarMixto = () => {
    if (!tenantInfo || !userProfile) return;
    const puntosUsuario = userProfile.puntos || 0;
    const max = Math.min(puntosUsuario, Math.floor(precio / valorPunto));
    if (max <= 0) return handleComprar();
    setMaxPuntos(max);
    setMostrarPuntosModal(true);
  };

  const confirmarCompraMixta = (puntos) => {
    setMostrarPuntosModal(false);
    handleComprar(puntos);
  };

  if (!isOpen) return null;

  const btnStyle = tenantInfo?.color
    ? { backgroundColor: tenantInfo.color, borderColor: tenantInfo.color }
    : {};

  return (
    <div
    className="modal show d-block"
    tabIndex="-1"
    style={{ backgroundColor: "rgba(0,0,0,0.5)" }}
    onClick={onClose}
  >
    <div className="modal-dialog" onClick={(e) => e.stopPropagation()}>
      <div className="modal-content">
        <div
          className="modal-header"
          style={{ backgroundColor: tenantInfo?.color || "#f8f9fa" }}
          >
            <h5 className="modal-title">{servicio}</h5>
            <button type="button" className="btn-close" onClick={onClose}></button>
          </div>
          <div className="modal-body">
            <p className="mb-3">
              Precio: <strong>${precio?.toFixed ? precio.toFixed(2) : precio}</strong>
            </p>
            {error && <div className="alert alert-danger py-2">{error}</div>}
          </div>
          <div className="modal-footer">
            <button
              type="button"
              className="btn btn-outline-secondary"
              onClick={onClose}
              disabled={loading}
            >
              Cancelar
            </button>
            <button
              type="button"
              className="btn btn-primary"
              style={btnStyle}
              onClick={() => handleComprar()}
              disabled={loading}
            >
              Pagar con Dinero
            </button>
            <button
              type="button"
              className="btn btn-primary"
              style={btnStyle}
              onClick={handleComprarMixto}
              disabled={loading || !tenantInfo}
            >
              Dinero + {tenantInfo?.nombrePuntos || "Puntos"}
            </button>
          </div>
        </div>
      </div>
      <SeleccionarPuntosModal
        isOpen={mostrarPuntosModal}
        onClose={() => setMostrarPuntosModal(false)}
        maxPuntos={maxPuntos}
        valorPunto={valorPunto}
        precio={precio}
        onConfirm={confirmarCompraMixta}
      />
    </div>
  );
};

export default ComprarServicioModal;