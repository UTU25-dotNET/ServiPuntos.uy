import React, { useEffect, useRef, useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import Breadcrumb from "../layout/Breadcrumb";
import { QRCodeSVG } from "qrcode.react";
import apiService from "../../services/apiService";
import ComprarOfertaModal from "./ComprarOfertaModal";

const PromocionesList = () => {
  const [promos, setPromos] = useState([]);
  const [ubicacionesMap, setUbicacionesMap] = useState({});
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [filter, setFilter] = useState("todas");
  const [canjeQR, setCanjeQR] = useState(null);
  const [canjeLoading, setCanjeLoading] = useState(false);
  const [canjeError, setCanjeError] = useState("");
  const [ofertaSeleccionada, setOfertaSeleccionada] = useState(null);
  const location = useLocation();
  const navigate = useNavigate();

  const loadedRef = useRef(false);

  useEffect(() => {
    if (!promos.length) return;
    const title = location.state?.selectedPromoTitle;
    if (!title) return;
    const promo = promos.find((p) => p.titulo === title);
    if (!promo) return;
    if (promo.tipo === "Oferta") {
      setOfertaSeleccionada(promo);
    } else {
      handleCanjear(promo);
    }
    navigate(location.pathname, { replace: true, state: {} });
  }, [promos, location.state]);

  useEffect(() => {
    const load = async () => {
      if (loadedRef.current) return;
      loadedRef.current = true;
      setLoading(true);
      setError("");
      try {
        // Obtener siempre las promociones del tenant del usuario
        const promosData = await apiService.getPromocionesByUserTenant();
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

  const handleComprar = (promo) => {
    setOfertaSeleccionada(promo);
  };

  const handleCanjear = async (promo) => {
    setCanjeLoading(true);
    setCanjeError("");
    try {
      const ubicacionId = promo.ubicaciones?.[0];
      const productoId = promo.productoIds?.[0];
      if (!ubicacionId || !productoId) {
        throw new Error("Datos incompletos para canjear");
      }

      const result = await apiService.generarCanje(productoId, ubicacionId);
      setCanjeQR(result?.datos?.codigoQR || null);
    } catch (err) {
      setCanjeError(err.message);
    } finally {
      setCanjeLoading(false);
    }
  };

  const getCardClasses = (tipo) =>
    tipo === "Oferta" ? "card h-100 border-success" : "card h-100 border-primary";

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
            <div className={getCardClasses(p.tipo)}>
              <div className="card-body d-flex flex-column">
                <div className="d-flex justify-content-between align-items-start mb-2">
                  <h5 className="card-title mb-0">{p.titulo}</h5>
                  <span className={
                    p.tipo === "Oferta" ? "badge bg-success" : "badge bg-primary"
                  }>
                    {p.tipo}
                  </span>
                </div>
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
                <div className="mt-auto">
                  {p.tipo === "Oferta" ? (
                    <button
                      className="btn btn-success w-100"
                      onClick={() => handleComprar(p)}
                    >
                      Comprar
                    </button>
                  ) : (
                    <button
                      className="btn btn-primary w-100"
                      onClick={() => handleCanjear(p)}
                    >
                      Canjear
                    </button>
                  )}
                </div>
              </div>
            </div>
          </div>
        ))}
      </div>
      {canjeError && <p className="text-danger mt-2">{canjeError}</p>}
      {canjeQR && (
        <div
          style={{
            position: "fixed",
            top: 0,
            left: 0,
            right: 0,
            bottom: 0,
            backgroundColor: "rgba(0,0,0,0.6)",
            display: "flex",
            justifyContent: "center",
            alignItems: "center",
            zIndex: 2000,
          }}
        >
          <div
            style={{
              background: "white",
              padding: "2rem",
              borderRadius: "12px",
              textAlign: "center",
            }}
          >
            <h3 style={{ marginTop: 0 }}>Código de Canje</h3>
            {canjeLoading ? (
              <p>Generando código...</p>
            ) : (
              <>
                <QRCodeSVG value={canjeQR} size={180} />
                <p style={{ wordBreak: "break-all" }}>{canjeQR}</p>
              </>
            )}
            <button
              onClick={() => setCanjeQR(null)}
              style={{ marginTop: "1rem", padding: "0.5rem 1rem" }}
            >
              Cerrar
            </button>
          </div>
        </div>
      )}
      {ofertaSeleccionada && (
        <ComprarOfertaModal
          isOpen={!!ofertaSeleccionada}
          onClose={() => setOfertaSeleccionada(null)}
          oferta={ofertaSeleccionada}
        />
      )}
    </div>
  );
};

export default PromocionesList;
