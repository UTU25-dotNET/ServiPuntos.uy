import React, { useState, useEffect } from "react";
import apiService from "../../services/apiService";
import SeleccionarPuntosModal from "../productos/SeleccionarPuntosModal";

const ComprarCombustibleModal = ({
  isOpen,
  onClose,
  ubicacion,
  tipo,
  precio,
  userProfile,
}) => {
  const [litros, setLitros] = useState(0);
  const [monto, setMonto] = useState(0);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [tenantInfo, setTenantInfo] = useState(null);
  const [mostrarPuntosModal, setMostrarPuntosModal] = useState(false);
  const [maxPuntos, setMaxPuntos] = useState(0);

  useEffect(() => {
    if (isOpen) {
      apiService.getTenantInfo().then(setTenantInfo).catch(() => {});
      setLitros(0);
      setMonto(0);
      setError("");
    }
  }, [isOpen]);

  const valorPunto = tenantInfo?.valorPunto || 0;

  const handleLitrosChange = (val) => {
    const l = parseFloat(val) || 0;
    setLitros(l);
    setMonto(parseFloat((l * precio).toFixed(2)));
  };

  const handleMontoChange = (val) => {
    const m = parseFloat(val) || 0;
    setMonto(m);
    setLitros(parseFloat((m / precio).toFixed(2)));
  };

  const handleComprar = async (puntos = 0) => {
    if (!ubicacion) return;
    setLoading(true);
    setError("");
    try {
      const producto = {
        productoCanjeable: { id: crypto.randomUUID(), nombre: tipo },
        categoria: "combustible",
        precio: monto,
      };
      const result = await apiService.procesarTransaccion(
        producto,
        ubicacion.id,
        puntos,
        valorPunto,
        1
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
    const max = Math.min(puntosUsuario, Math.floor(monto / valorPunto));
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
        <h3 style={{ marginTop: 0 }}>Comprar {tipo}</h3>
        <div style={{ display: "grid", gap: "0.75rem" }}>
          <label>
            Litros:
            <input
              type="number"
              value={litros}
              onChange={(e) => handleLitrosChange(e.target.value)}
              min="0"
              step="0.01"
            />
          </label>
          <label>
            Monto ($):
            <input
              type="number"
              value={monto}
              onChange={(e) => handleMontoChange(e.target.value)}
              min="0"
              step="0.01"
            />
          </label>
        </div>
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
        precio={monto}
        onConfirm={confirmarCompraMixta}
      />
    </div>
  );
};

export default ComprarCombustibleModal;