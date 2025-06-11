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

  return (
    <div className="modal-overlay" onClick={onClose}>
      <div
        className="modal-content"
        onClick={(e) => e.stopPropagation()}
        style={{ maxWidth: "400px" }}
      >
        <h3 style={{ marginTop: 0 }}>{servicio}</h3>
        <p style={{ marginTop: 0 }}>
          Precio: <strong>${precio?.toFixed ? precio.toFixed(2) : precio}</strong>
        </p>
        {error && <p style={{ color: "#dc3545" }}>{error}</p>}
        <div className="modal-buttons">
          <button type="button" className="danger-btn" onClick={onClose} disabled={loading}>
            Cancelar
          </button>
          <button type="button" className="login-btn" onClick={() => handleComprar()} disabled={loading}>
            Pagar con Dinero
          </button>
          <button
            type="button"
            className="login-btn"
            onClick={handleComprarMixto}
            disabled={loading || !tenantInfo}
          >
            Dinero + {tenantInfo?.nombrePuntos || "Puntos"}
          </button>
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