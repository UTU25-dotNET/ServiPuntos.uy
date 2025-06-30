import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import CatalogoProductos from "../productos/CatalogoProductos";
import ComprarCombustibleModal from "../estaciones/ComprarCombustibleModal";
import ComprarServicioModal from "../estaciones/ComprarServicioModal";
import apiService from "../../services/apiService";

const UbicacionModal = ({ isOpen, onClose, ubicacion }) => {
  const [modalProductos, setModalProductos] = useState(false);
  const [modalCombustible, setModalCombustible] = useState({ abierto: false, tipo: "", precio: 0 });
  const [modalServicio, setModalServicio] = useState({ abierto: false, servicio: "", precio: 0 });
  const [userProfile, setUserProfile] = useState(null);
  const navigate = useNavigate();

  useEffect(() => {
    if (isOpen) {
      apiService.getUserProfile().then(setUserProfile).catch(() => {});
    }
  }, [isOpen]);

  if (!isOpen || !ubicacion) return null;

  const formatPrice = (price) => {
    if (!price || price === 0) return "No disponible";
    return `${price.toFixed(2)}`;
  };

  const openCatalogo = () => {
    navigate("/estaciones", { state: { openCatalogo: true, ubicacionId: ubicacion.id } });
  };
  const closeCatalogo = () => setModalProductos(false);

  const openCombustible = (tipo, precio) => setModalCombustible({ abierto: true, tipo, precio });
  const closeCombustible = () => setModalCombustible({ abierto: false, tipo: "", precio: 0 });

  const openServicio = (servicio, precio) => setModalServicio({ abierto: true, servicio, precio });
  const closeServicio = () => setModalServicio({ abierto: false, servicio: "", precio: 0 });

  const refreshUserProfile = async () => {
    try {
      const profile = await apiService.getUserProfile();
      setUserProfile(profile);
    } catch {}
  };

  const {
    nombre,
    direccion,
    ciudad,
    departamento,
    telefono,
    precioNaftaSuper,
    precioNaftaPremium,
    precioDiesel,
    cambioDeAceite,
    cambioDeNeumaticos,
    lavadoDeAuto,
    precioCambioAceite,
    precioCambioNeumaticos,
    precioLavado,
    horaApertura,
    horaCierre,
  } = ubicacion;

  return (
    <div
      className="modal show d-block"
      tabIndex="-1"
      style={{ backgroundColor: "rgba(0,0,0,0.5)" }}
      onClick={onClose}
    >
      <div className="modal-dialog modal-lg" onClick={(e) => e.stopPropagation()}>
        <div className="modal-content">
          <div className="modal-header" style={{ backgroundColor: "var(--primary-color)", color: "white" }}>
            <h5 className="modal-title">{nombre}</h5>
            <button type="button" className="btn-close" onClick={onClose}></button>
          </div>
          <div className="modal-body">
            <p className="mb-1"><strong>Dirección:</strong> {direccion}</p>
            <p className="mb-1"><strong>Ciudad:</strong> {ciudad}, {departamento}</p>
            {telefono && <p className="mb-1"><strong>Teléfono:</strong> {telefono}</p>}
            {horaApertura && horaCierre && horaApertura !== "00:00:00" && horaCierre !== "00:00:00" && (
              <p className="mb-3"><strong>Horario:</strong> {horaApertura.slice(0,5)} - {horaCierre.slice(0,5)}</p>
            )}
            <div className="mb-3">
              <h6>Combustible</h6>
              <div className="row g-2">
                <div className="col-12 col-md-4">
                  <div className="border rounded p-2 text-center">
                    <div>Nafta Súper</div>
                    <div className="fw-bold">${formatPrice(precioNaftaSuper)}</div>
                    <button className="btn btn-sm btn-primary mt-2" onClick={() => openCombustible("Nafta Súper", precioNaftaSuper)}>Comprar</button>
                  </div>
                </div>
                <div className="col-12 col-md-4">
                  <div className="border rounded p-2 text-center">
                    <div>Nafta Premium</div>
                    <div className="fw-bold">${formatPrice(precioNaftaPremium)}</div>
                    <button className="btn btn-sm btn-primary mt-2" onClick={() => openCombustible("Nafta Premium", precioNaftaPremium)}>Comprar</button>
                  </div>
                </div>
                <div className="col-12 col-md-4">
                  <div className="border rounded p-2 text-center">
                    <div>Diesel</div>
                    <div className="fw-bold">${formatPrice(precioDiesel)}</div>
                    <button className="btn btn-sm btn-primary mt-2" onClick={() => openCombustible("Diesel", precioDiesel)}>Comprar</button>
                  </div>
                </div>
              </div>
            </div>
            {(cambioDeAceite || cambioDeNeumaticos || lavadoDeAuto) && (
              <div className="mb-3">
                <h6>Servicios</h6>
                <div className="d-flex flex-wrap gap-2">
                  {cambioDeAceite && (
                    <button className="btn btn-outline-secondary" onClick={() => openServicio("Cambio de Aceite", precioCambioAceite)}>
                      Cambio de Aceite
                    </button>
                  )}
                  {cambioDeNeumaticos && (
                    <button className="btn btn-outline-secondary" onClick={() => openServicio("Cambio de Neumáticos", precioCambioNeumaticos)}>
                      Cambio de Neumáticos
                    </button>
                  )}
                  {lavadoDeAuto && (
                    <button className="btn btn-outline-secondary" onClick={() => openServicio("Lavado de Auto", precioLavado)}>
                      Lavado de Auto
                    </button>
                  )}
                </div>
              </div>
            )}
            <div className="text-center">
              <button className="btn btn-primary" onClick={openCatalogo}>Ver Catálogo de Productos</button>
            </div>
          </div>
        </div>
      </div>
      <CatalogoProductos
        ubicacion={ubicacion}
        isOpen={modalProductos}
        onClose={closeCatalogo}
        userProfile={userProfile}
        onProfileUpdated={refreshUserProfile}
      />
      <ComprarCombustibleModal
        isOpen={modalCombustible.abierto}
        onClose={closeCombustible}
        ubicacion={ubicacion}
        tipo={modalCombustible.tipo}
        precio={modalCombustible.precio}
        userProfile={userProfile}
      />
      <ComprarServicioModal
        isOpen={modalServicio.abierto}
        onClose={closeServicio}
        ubicacion={ubicacion}
        servicio={modalServicio.servicio}
        precio={modalServicio.precio}
        userProfile={userProfile}
      />
    </div>
  );
};

export default UbicacionModal;
