import React, { useState } from "react";

const SeleccionarPuntosModal = ({
  isOpen,
  onClose,
  maxPuntos,
  valorPunto,
  precio,
  nombrePuntos = "puntos",
  onConfirm
}) => {
  const [puntos, setPuntos] = useState(maxPuntos);

  if (!isOpen) return null;

  const handleConfirm = () => {
    onConfirm(puntos);
  };

  const precioFinal = Math.max(0, Math.round(precio - puntos * valorPunto));

  return (
    <div className="modal-overlay" onClick={onClose}>
      <div className="modal-content" onClick={(e) => e.stopPropagation()} style={{maxWidth: "350px"}}>
        <h3 style={{ marginTop: 0 }}>Selecciona cuántos {nombrePuntos} usar</h3>
        <p style={{ margin: "0.5rem 0" }}>Valor de cada punto: ${valorPunto}</p>
        <input
          type="range"
          min="0"
          max={maxPuntos}
          value={puntos}
          onChange={(e) => setPuntos(parseInt(e.target.value, 10))}
          className="range-slider"
        />
        <div style={{ display: "flex", justifyContent: "space-between", fontSize: "0.9rem" }}>
          <span>0</span>
          <span>{maxPuntos}</span>
        </div>
        <p style={{ marginTop: "1rem" }}>
          Pagarás <strong>${precioFinal}</strong> en pesos
        </p>
        <div className="modal-buttons">
          <button type="button" className="danger-btn" onClick={onClose}>
            Cancelar
          </button>
          <button type="button" className="login-btn" onClick={handleConfirm}>
            Confirmar
          </button>
        </div>
      </div>
    </div>
  );
};

export default SeleccionarPuntosModal;