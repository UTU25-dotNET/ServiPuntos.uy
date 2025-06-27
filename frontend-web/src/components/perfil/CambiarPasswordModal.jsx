import React, { useState } from "react";
import authService from "../../services/authService";
import apiService from "../../services/apiService";


const CambiarPasswordModal = ({ isOpen, onClose, onSuccess }) => {
  const [currentPassword, setCurrentPassword] = useState("");
  const [newPassword, setNewPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  if (!isOpen) return null;

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!currentPassword) {
      setError("Ingresa tu contraseña actual");
      return;
    }
    if (newPassword.length < 6) {
      setError("La nueva contraseña debe tener al menos 6 caracteres");
      return;
    }
    if (newPassword !== confirmPassword) {
      setError("Las contraseñas nuevas no coinciden");
      return;
    }
    try {
      setLoading(true);
      setError("");
      const user = authService.getCurrentUser();
      await authService.verifyPassword(user.email, currentPassword);
      await apiService.updateUserProfile({ password: newPassword });
      setCurrentPassword("");
      setNewPassword("");
      setConfirmPassword("");
      if (onSuccess) onSuccess();
      onClose();
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="modal-overlay" onClick={onClose}>
      <div className="modal-content" onClick={(e) => e.stopPropagation()}>
        <h3 style={{ marginTop: 0 }}>Cambiar contraseña</h3>
        {error && (
          <div style={{ color: "#dc3545", marginBottom: "0.75rem" }}>{error}</div>
        )}
        <form onSubmit={handleSubmit}>
          <div className="input-group">
            <span className="icon">🔒</span>
            <input
              type="password"
              placeholder="Contraseña actual"
              value={currentPassword}
              onChange={(e) => setCurrentPassword(e.target.value)}
            />
          </div>
          <div className="input-group">
            <span className="icon">🔑</span>
            <input
              type="password"
              placeholder="Nueva contraseña"
              value={newPassword}
              onChange={(e) => setNewPassword(e.target.value)}
            />
          </div>
          <div className="input-group">
            <span className="icon">🔑</span>
            <input
              type="password"
              placeholder="Confirmar contraseña"
              value={confirmPassword}
              onChange={(e) => setConfirmPassword(e.target.value)}
            />
          </div>
          <div className="modal-buttons">
            <button type="button" className="danger-btn" onClick={onClose} disabled={loading}>
              Cancelar
            </button>
            <button type="submit" className="login-btn" disabled={loading}>
              Confirmar
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default CambiarPasswordModal;
