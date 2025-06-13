import React, { useEffect, useState } from "react";
import apiService from "../../services/apiService";

const Historial = ({ usuarioId }) => {
  const [tipo, setTipo] = useState("canjes");
  const [canjePages, setCanjePages] = useState([]); // [{ items }]
  const [canjeCursor, setCanjeCursor] = useState(null);
  const [canjePageIndex, setCanjePageIndex] = useState(0);

  const [transPages, setTransPages] = useState([]); // [{ items }]
  const [transCursorState, setTransCursorState] = useState(null);
  const [transPageIndex, setTransPageIndex] = useState(0);
  const [expandedRows, setExpandedRows] = useState({});
  
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
        setCanjePages([{ items: c.items }]);
        setCanjeCursor(c.nextCursor);
        setCanjePageIndex(0);

        setTransPages([{ items: t.items }]);
        setTransCursorState(t.nextCursor);
        setTransPageIndex(0);
      } catch (err) {
        setError(err.message || "Error al cargar historial");
      } finally {
        setLoading(false);
      }
    };
    fetchData();
  }, [usuarioId]);

  const toggleRow = id => {
    setExpandedRows(prev => ({ ...prev, [id]: !prev[id] }));
  };

  useEffect(() => {
    setExpandedRows({});
  }, [canjePageIndex, transPageIndex, tipo]);

  const cargarMasCanjes = async () => {
    if (!canjeCursor) return;
    try {
      const res = await apiService.getCanjesByUsuario(usuarioId, canjeCursor);
      setCanjePages(prev => [...prev, { items: res.items }]);
      setCanjeCursor(res.nextCursor);
      setCanjePageIndex(prev => prev + 1);
    } catch (err) {
      setError(err.message || 'Error al cargar m\u00e1s canjes');
    }
  };

  const cargarMasTrans = async () => {
    if (!transCursorState) return;
    try {
      const res = await apiService.getTransaccionesByUsuario(transCursorState);
      setTransPages(prev => [...prev, { items: res.items }]);
      setTransCursorState(res.nextCursor);
      setTransPageIndex(prev => prev + 1);
    } catch (err) {
      setError(err.message || 'Error al cargar m\u00e1s transacciones');
    }
  };

  if (loading) return <p>Cargando historial...</p>;
  if (error) return <p>{error}</p>;

  const data = tipo === "canjes"
    ? (canjePages[canjePageIndex]?.items || [])
    : (transPages[transPageIndex]?.items || []);

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
            ? (canjePages[canjePageIndex]?.items || []).map(c => (
                <tr key={c.id}>
                  <td>{c.producto || "-"}</td>
                  <td>{c.ubicacion || "-"}</td>
                  <td>{new Date(c.fechaCanje || c.fechaGeneracion).toLocaleDateString('es-ES')}</td>
                  <td>{c.puntos}</td>
                  <td>{estadoLabel(c.estado)}</td>
                </tr>
              ))
              : (transPages[transPageIndex]?.items || []).map(t => {
                let productos = [];
                if (t.detalles) {
                  try {
                    const parsed = JSON.parse(t.detalles);
                    productos = parsed.Productos || [];
                  } catch {}
                }
                return (
                  <React.Fragment key={t.id}>
                    <tr>
                      <td>
                        <button
                          className="btn btn-sm btn-link p-0 me-2"
                          onClick={() => toggleRow(t.id)}
                        >
                          {expandedRows[t.id] ? '-' : '+'}
                        </button>
                        {new Date(t.fecha || t.fechaGeneracion).toLocaleDateString('es-ES')}
                      </td>
                      <td>{t.ubicacion || "-"}</td>
                      <td>${t.monto}</td>
                      <td>{t.tipo}</td>
                      <td>{t.puntosOtorgados}</td>
                      <td>{t.puntosUtilizados}</td>
                    </tr>
                    {expandedRows[t.id] && (
                      <tr>
                        <td colSpan="6">
                          <ul className="mb-0">
                            {productos.map((p, idx) => (
                              <li key={idx}>{p.NombreProducto} x{p.Cantidad} - ${p.SubTotal}</li>
                            ))}
                          </ul>
                        </td>
                      </tr>
                    )}
                  </React.Fragment>
                );
              })}
        </tbody>
      </table>
      {tipo === "canjes" && (
        <div className="d-flex gap-2 mt-2">
          {canjePageIndex > 0 && (
            <button className="btn btn-outline-secondary" onClick={() => setCanjePageIndex(prev => prev - 1)}>
              Anterior
            </button>
          )}
          {canjeCursor && (
            <button className="btn btn-outline-primary" onClick={cargarMasCanjes}>
              Siguiente
            </button>
          )}
        </div>
      )}
      {tipo === "transacciones" && (
        <div className="d-flex gap-2 mt-2">
          {transPageIndex > 0 && (
            <button className="btn btn-outline-secondary" onClick={() => setTransPageIndex(prev => prev - 1)}>
              Anterior
            </button>
          )}
          {transCursorState && (
            <button className="btn btn-outline-primary" onClick={cargarMasTrans}>
              Siguiente
            </button>
          )}
        </div>
      )}
    </div>
  );
};

export default Historial;