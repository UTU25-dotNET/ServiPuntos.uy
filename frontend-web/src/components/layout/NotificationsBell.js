import { useState } from 'react';
import apiService from '../../services/apiService';
import authService from '../../services/authService';

const NotificationsBell = ({ textColor, user }) => {
  const [open, setOpen] = useState(false);
  const [items, setItems] = useState([]);

  const loadNotifications = async () => {
    if (!authService.getToken()) return;

    try {
      await apiService.getUserProfile({ skipAuthError: true });
    } catch {
      return;
    }

    try {
      const data = await apiService.getMisNotificaciones({ skipAuthError: true });
      setItems(data);
    } catch {
      // Ignorar errores en carga inicial
    }
  };

  const toggle = async () => {
    const newOpen = !open;
    setOpen(newOpen);
    if (newOpen && items.length === 0) {
      await loadNotifications();
    }
  };

  const handleDelete = async (id) => {
    try {
      await apiService.borrarNotificacion(id);
      setItems(items.filter(n => n.id !== id));
    } catch {}
  };

  const unread = items.filter(n => !n.leida).length;

  return (
    <div className="dropdown">
      <button
        type="button"
        className="btn btn-link p-0 position-relative text-decoration-none"
        style={{ color: textColor }}
        onClick={toggle}
      >
        <span role="img" aria-label="notificaciones">🔔</span>
        {unread > 0 && (
          <span className="badge bg-danger ms-1 position-absolute top-0 start-100 translate-middle">
            {unread}
          </span>
        )}
      </button>
      <div
        className={`dropdown-menu dropdown-menu-end${open ? ' show' : ''}`}
        style={{ minWidth: '250px' }}
      >
        {items.length === 0 ? (
          <div className="px-3 py-2 text-muted">Sin notificaciones</div>
        ) : (
          items.map((n) => (
            <div key={n.id} className="dropdown-item d-flex justify-content-between align-items-start">
              <div className="me-2" style={{ maxWidth: '200px' }}>
                <div className="fw-bold">{n.titulo}</div>
                <small className="text-muted">{n.mensaje}</small>
              </div>
              <button type="button" className="btn-close" aria-label="Eliminar" onClick={() => handleDelete(n.id)}></button>
            </div>
          ))
        )}
      </div>
    </div>
  );
};

export default NotificationsBell;