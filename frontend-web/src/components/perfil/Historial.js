import React, { useEffect, useState } from "react";
import apiService from "../../services/apiService";

const Historial = ({ usuarioId }) => {
  const [tipo, setTipo] = useState("canjes");
  const [canjes, setCanjes] = useState([]);
  const [transacciones, setTransacciones] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  const estadoLabel = (estado) => {
    switch (estado) {
      case "Generado":
        return "Pendiente";
      case "Canjeado":
        return "Confirmado";
      default:
        return estado;
    }
  };

  useEffect(() => {
    const fetchData = async () => {
      try {
        const [c, t] = await Promise.all([
          apiService.getCanjesByUsuario(usuarioId),
          apiService.getTransaccionesByUsuario()
        ]);
        setCanjes(c);
        setTransacciones(t);
      } catch (err) {
        setError(err.message || "Error al cargar historial");
      } finally {
        setLoading(false);
      }
    };
    fetchData();
  }, [usuarioId]);

  if (loading) return <p>Cargando historial...</p>;
  if (error) return <p>{error}</p>;

  const data = tipo === "canjes" ? canjes : transacciones;

  if (!data.length) {
    return (
      <div className="mt-3">
        <div className="mb-2">
          <select className="form-select" value={tipo} onChange={e => setTipo(e.target.value)}>
            <option value="canjes">Historial de Canjes</option>
            <option value="transacciones">Historial de Transacciones</option>
          </select>
        </div>
        <p>No hay registros.</p>
      </div>
    );
  }

  return (
    <div className="mt-3">
      <div className="mb-2">
        <select className="form-select" value={tipo} onChange={e => setTipo(e.target.value)}>
          <option value="canjes">Historial de Canjes</option>
          <option value="transacciones">Historial de Transacciones</option>
        </select>
      </div>
      <table className="table table-striped">
        <thead>
          {tipo === "canjes" ? (
            <tr>
              <th>Producto</th>
              <th>Ubicación</th>
              <th>Fecha</th>
              <th>Puntos</th>
              <th>Estado</th>
            </tr>
          ) : (
            <tr>
              <th>Fecha</th>
              <th>Ubicación</th>
              <th>Monto</th>
              <th>Tipo</th>
              <th>P. Otorgados</th>
              <th>P. Utilizados</th>
            </tr>
          )}
        </thead>
        <tbody>
          {tipo === "canjes"
            ? canjes.map(c => (
                <tr key={c.id}>
                  <td>{c.producto || "-"}</td>
                  <td>{c.ubicacion || "-"}</td>
                  <td>{new Date(c.fechaCanje || c.fechaGeneracion).toLocaleDateString('es-ES')}</td>
                  <td>{c.puntos}</td>
                  <td>{estadoLabel(c.estado)}</td>
                </tr>
              ))
            : transacciones.map(t => (
                <tr key={t.id}>
                  <td>{new Date(t.fecha || t.fechaGeneracion).toLocaleDateString('es-ES')}</td>
                  <td>{t.ubicacion || "-"}</td>
                  <td>${t.monto}</td>
                  <td>{t.tipo}</td>
                  <td>{t.puntosOtorgados}</td>
                  <td>{t.puntosUtilizados}</td>
                </tr>
              ))}
        </tbody>
      </table>
    </div>
  );
};

export default Historial;