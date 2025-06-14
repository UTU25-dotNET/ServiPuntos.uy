import React, { useEffect, useRef, useState } from "react";
import Breadcrumb from "../layout/Breadcrumb";
import apiService from "../../services/apiService";

const PromocionesList = () => {
  const [promos, setPromos] = useState([]);
  const [ubicacionesMap, setUbicacionesMap] = useState({});
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [filter, setFilter] = useState("todas");

  const loadedRef = useRef(false);

  useEffect(() => {
    const load = async () => {
      if (loadedRef.current) return;
      loadedRef.current = true;
      setLoading(true);
      setError("");
      try {
        // Obtener siempre las promociones
        const promosData = await apiService.getPromociones();
        console.log("Promociones cargadas", promosData); // Added log
        setPromos(promosData);

        // Intentar cargar las ubicaciones pero no fallar si ocurre un error
        try {
          const ubicaciones = await apiService.getUbicacionesByUserTenant();
          const map = {};
          ubicaciones.forEach((u) => {
            map[u.id] = u.nombre;
          });
          setUbicacionesMap(map);
        } catch (err) {
          console.error("Error al cargar ubicaciones", err);
          // No interrumpe la carga de promociones
        }
      } catch (err) {
        console.error("Error al cargar promociones", err); // Added log
        setError(err.message);
      } finally {
        setLoading(false);
      }
    };
    load();
  }, []);

  const filteredPromos = promos.filter((p) => {
    if (filter === "todas") return true;
    if (filter === "promociones") return p.tipo === "Promocion";
    if (filter === "ofertas") return p.tipo === "Oferta";
    return true;
  });

  const formatDate = (d) => new Date(d).toLocaleDateString();

  return (
    <div className="container my-4">
      <Breadcrumb current="Promociones" />
      <h2 className="mb-3">Promociones</h2>
      <div className="mb-3">
        <select
          className="form-select w-auto d-inline-block"
          value={filter}
          onChange={(e) => setFilter(e.target.value)}
        >
          <option value="todas">Todas</option>
          <option value="promociones">Promociones</option>
          <option value="ofertas">Ofertas</option>
        </select>
      </div>
      {loading && <p>Cargando...</p>}
      {error && <p className="text-danger">{error}</p>}
      <div className="row">
        {filteredPromos.map((p) => (
          <div className="col-md-4 mb-3" key={p.id}>
            <div className="card h-100">
              <div className="card-body">
                <h5 className="card-title">{p.titulo}</h5>
                <p className="card-text">{p.descripcion}</p>
                <p className="card-text">
                  <small className="text-muted">Inicio: {formatDate(p.fechaInicio)}</small>
                  <br />
                  <small className="text-muted">Fin: {formatDate(p.fechaFin)}</small>
                </p>
                {p.tipo === "Promocion" && p.precioEnPuntos && (
                  <p className="card-text">Costo: {p.precioEnPuntos} puntos</p>
                )}
                {p.tipo === "Oferta" && (
                  <>
                    {p.precioEnPesos !== null && (
                      <p className="card-text">Precio: ${p.precioEnPesos}</p>
                    )}
                    {p.descuentoEnPesos !== null && (
                      <p className="card-text">Descuento: ${p.descuentoEnPesos}</p>
                    )}
                  </>
                )}
                {p.ubicaciones && p.ubicaciones.length > 0 && (
                  <div>
                    <small className="text-muted">Ubicaciones:</small>
                    <ul className="mb-0">
                      {p.ubicaciones.map((uid) => (
                        <li key={uid}>{ubicacionesMap[uid] || uid}</li>
                      ))}
                    </ul>
                  </div>
                )}
              </div>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
};

export default PromocionesList;
