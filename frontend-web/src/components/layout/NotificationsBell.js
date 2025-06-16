import { useState } from 'react';
import apiService from '../../services/apiService';
import authService from '../../services/authService';

const NotificationsBell = ({ textColor, user }) => {
  const [open, setOpen] = useState(false);
  const [items, setItems] = useState([]);

  const loadNotifications = async () => {
    if (!authService.getToken()) return;

    try {
      await apiService.getUserProfile();
    } catch {
      return;
    }

    try {
      const data = await apiService.getMisNotificaciones();
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
    <div style={{ position: 'relative' }}>
      <span onClick={toggle} style={{ cursor: 'pointer', color: textColor }}>
        🔔{unread > 0 && <span style={{ marginLeft: '4px', color: 'red' }}>{unread}</span>}
      </span>
      {open && (
        <div style={{ position: 'absolute', right: 0, top: '120%', background: 'white', color: '#000', border: '1px solid #ddd', borderRadius: '6px', padding: '0.5rem', zIndex: 999, minWidth: '250px' }}>
          {items.length === 0 ? (
            <div style={{ padding: '0.5rem' }}>Sin notificaciones</div>
          ) : (
            items.map(n => (
              <div key={n.id} style={{ borderBottom: '1px solid #eee', padding: '0.25rem 0', display: 'flex', justifyContent: 'space-between', alignItems: 'start' }}>
                <div style={{ maxWidth: '200px' }}>
                  <strong>{n.titulo}</strong>
                  <div style={{ fontSize: '0.85rem' }}>{n.mensaje}</div>
                </div>
                <button onClick={() => handleDelete(n.id)} style={{ background: 'transparent', border: 'none', color: '#dc3545' }}>x</button>
              </div>
            ))
          )}
        </div>
      )}
    </div>
  );
};

export default NotificationsBell;