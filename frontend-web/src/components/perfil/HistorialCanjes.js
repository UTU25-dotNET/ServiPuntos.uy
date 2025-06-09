import React, { useEffect, useState } from "react";
import apiService from "../../services/apiService";

const HistorialCanjes = ({ usuarioId, onClose }) => {
  const [canjes, setCanjes] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    const fetchData = async () => {
      try {
        const data = await apiService.getCanjesByUsuario(usuarioId);
        setCanjes(data);
      } catch (err) {
        setError(err.message || "Error al cargar historial");
      } finally {
        setLoading(false);
      }
    };
    fetchData();
  }, [usuarioId]);

  if (loading) {
    return <p>Cargando historial...</p>;
  }

  if (error) {
    return <p>{error}</p>;
  }

  if (!canjes.length) {
    return (
      <div>
        <h4>Historial de Canjes</h4>
        <p>No hay canjes registrados.</p>
        {onClose && (
          <button onClick={onClose} style={{ marginTop: "1rem" }}>
            Cerrar
          </button>
        )}
      </div>
    );
  }

  return (
    <div style={{ marginTop: "1rem" }}>
      <h4>Historial de Canjes</h4>
      <table style={{ width: "100%", borderCollapse: "collapse" }}>
        <thead>
          <tr>
            <th style={{ textAlign: "left", borderBottom: "1px solid #ccc" }}>Producto</th>
            <th style={{ textAlign: "left", borderBottom: "1px solid #ccc" }}>Ubicación</th>
            <th style={{ textAlign: "left", borderBottom: "1px solid #ccc" }}>Fecha</th>
            <th style={{ textAlign: "left", borderBottom: "1px solid #ccc" }}>Puntos</th>
          </tr>
        </thead>
        <tbody>
          {canjes.map((c) => (
            <tr key={c.id}>
              <td style={{ padding: "0.5rem 0" }}>{c.producto || "-"}</td>
              <td style={{ padding: "0.5rem 0" }}>{c.ubicacion || "-"}</td>
              <td style={{ padding: "0.5rem 0" }}>
                {new Date(c.fechaCanje || c.fechaGeneracion).toLocaleDateString()}
              </td>
              <td style={{ padding: "0.5rem 0" }}>{c.puntos}</td>
            </tr>
          ))}
        </tbody>
      </table>
      {onClose && (
        <button onClick={onClose} style={{ marginTop: "1rem" }}>
          Cerrar
        </button>
      )}
    </div>
  );
};

export default HistorialCanjes;