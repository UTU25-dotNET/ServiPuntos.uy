import React from "react";

const ConfirmarCanjeModal = ({ isOpen, onClose, onConfirm, promo }) => {
  if (!isOpen || !promo) return null;

  return (
    <div className="modal-overlay" onClick={onClose}>
      <div className="modal-content" onClick={(e) => e.stopPropagation()}>
        <h3 style={{ marginTop: 0 }}>Confirmar canje</h3>
        <p>
          ¿Deseas canjear <strong>{promo.titulo}</strong>?
        </p>
        {promo.precioEnPuntos && (
          <p>Esta acción restará {promo.precioEnPuntos} puntos.</p>
        )}
        <div className="modal-buttons">
          <button type="button" className="danger-btn" onClick={onClose}>
            Cancelar
          </button>
          <button type="button" className="login-btn" onClick={onConfirm}>
            Confirmar
          </button>
        </div>
      </div>
    </div>
  );
};

export default ConfirmarCanjeModal;
