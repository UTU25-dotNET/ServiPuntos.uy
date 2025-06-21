import React, { useEffect, useRef, useState } from "react";
import { MapContainer, TileLayer, Marker, Popup } from "react-leaflet";
import L from "leaflet";
import "leaflet/dist/leaflet.css";
import apiService from "../../services/apiService";
import UbicacionDetails from "./UbicacionDetails";

import markerIcon2x from "leaflet/dist/images/marker-icon-2x.png";
import markerIcon from "leaflet/dist/images/marker-icon.png";
import markerShadow from "leaflet/dist/images/marker-shadow.png";
import "./MapaView.css";

// Fix default icon paths in Leaflet when using webpack
L.Icon.Default.mergeOptions({
  iconRetinaUrl: markerIcon2x,
  iconUrl: markerIcon,
  shadowUrl: markerShadow,
});

const openIcon = new L.DivIcon({
  html:
    '<div class="gas-marker"><img src="/markers/gas-station.svg" alt="open"/><span class="status-dot open"></span></div>',
  className: '',
  iconSize: [64, 64],
  iconAnchor: [32, 64],
});

const closedIcon = new L.DivIcon({
  html:
    '<div class="gas-marker"><img src="/markers/gas-station.svg" alt="closed"/><span class="status-dot closed"></span></div>',
  className: '',
  iconSize: [64, 64],
  iconAnchor: [32, 64],
});

const MapaView = () => {
  const [ubicaciones, setUbicaciones] = useState([]);
  const [selectedUbicacion, setSelectedUbicacion] = useState(null);
  const [tenantInfo, setTenantInfo] = useState(null);
  const mapRef = useRef(null);

  useEffect(() => {
    const fetchUbicaciones = async () => {
      try {
        const data = await apiService.getUbicacionesByUserTenant();
        setUbicaciones(data);
      } catch {
        try {
          const data = await apiService.getAllUbicaciones();
          setUbicaciones(data);
        } catch {
          setUbicaciones([]);
        }
      }
    };
    fetchUbicaciones();
  }, []);

  useEffect(() => {
    apiService.getTenantInfo().then(setTenantInfo).catch(() => {});
  }, []);

  const estaAbierta = (u) => {
    if (!u.horaApertura || !u.horaCierre) return true;
    if (u.horaApertura === "00:00:00" && u.horaCierre === "00:00:00") return true;
    const [hA, mA] = u.horaApertura.split(":").map(Number);
    const [hC, mC] = u.horaCierre.split(":").map(Number);
    const ahora = new Date();
    const apertura = new Date();
    apertura.setHours(hA, mA, 0, 0);
    const cierre = new Date();
    cierre.setHours(hC, mC, 0, 0);
    if (cierre <= apertura) {
      return ahora >= apertura || ahora <= cierre;
    }
    return ahora >= apertura && ahora <= cierre;
  };

  const handleItemClick = (ubicacion) => {
    if (selectedUbicacion?.id === ubicacion.id) {
      setSelectedUbicacion(null);
    } else {
      setSelectedUbicacion(ubicacion);
      if (mapRef.current) {
        const lat = parseFloat(ubicacion.latitud);
        const lng = parseFloat(ubicacion.longitud);
        if (!isNaN(lat) && !isNaN(lng)) {
          mapRef.current.flyTo([lat, lng], 15);
        }
      }
    }
  };

  const handleMarkerClick = (ubicacion) => {
    if (selectedUbicacion?.id === ubicacion.id) {
      setSelectedUbicacion(null);
    } else {
      setSelectedUbicacion(ubicacion);
      if (mapRef.current) {
        const lat = parseFloat(ubicacion.latitud);
        const lng = parseFloat(ubicacion.longitud);
        if (!isNaN(lat) && !isNaN(lng)) {
          mapRef.current.flyTo([lat, lng], 15);
        }
      }
    }
  };

  return (
    <div className="position-relative" style={{ height: "100vh", width: "100%" }}>
      <MapContainer
        center={[-34.9011, -56.1645]}
        zoom={12}
        style={{ height: "100%", width: "100%" }}
        whenCreated={(map) => (mapRef.current = map)}
      >
          <TileLayer url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png" />
            {ubicaciones.map((u) => {
              const lat = parseFloat(u.latitud);
              const lng = parseFloat(u.longitud);
              if (isNaN(lat) || isNaN(lng)) return null;
              return (
                <Marker
                  key={u.id}
                  position={[lat, lng]}
                  icon={estaAbierta(u) ? openIcon : closedIcon}
                  eventHandlers={{ click: () => handleMarkerClick(u) }}
                >
                  {selectedUbicacion?.id === u.id && <Popup>{u.nombre}</Popup>}
                </Marker>
              );
            })}
      </MapContainer>
      <div
        className="overflow-auto border position-fixed bg-white"
        style={{
          width: "300px",
          maxHeight: "90vh",
          top: "50%",
          right: 0,
          transform: "translateY(-50%)",
          margin: "1rem",
          zIndex: 1000,
        }}
      >
        <div className="list-group-item active fw-bold text-center sticky-top">
          {tenantInfo ? `Ubicaciones de ${tenantInfo.nombre}` : "Ubicaciones"}
        </div>
        <div className="accordion" id="ubicacionesAccordion">
          {ubicaciones.map((u) => (
            <div className="accordion-item" key={u.id} style={{ borderColor: 'var(--primary-color)' }}>
              <h2 className="accordion-header">
                <button
                  className={`accordion-button ${selectedUbicacion?.id === u.id ? '' : 'collapsed'}`}
                  type="button"
                  onClick={() => handleItemClick(u)}
                  style={{
                    '--bs-accordion-active-bg': 'var(--primary-color)',
                    '--bs-accordion-active-color': '#fff',
                    color: selectedUbicacion?.id === u.id ? '#fff' : 'var(--primary-color)'
                  }}
                >
                  {u.nombre}
                </button>
              </h2>
              <div className={`accordion-collapse collapse ${selectedUbicacion?.id === u.id ? 'show' : ''}`}
              >
                <div className="accordion-body p-0">
                  <UbicacionDetails ubicacion={u} expanded={selectedUbicacion?.id === u.id} />
                </div>
              </div>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
};

export default MapaView;
