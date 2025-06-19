import React, { useEffect, useRef, useState } from "react";
import { MapContainer, TileLayer, Marker, Popup } from "react-leaflet";
import L from "leaflet";
import "leaflet/dist/leaflet.css";
import apiService from "../../services/apiService";
import UbicacionModal from "./UbicacionModal";

import markerIcon2x from "leaflet/dist/images/marker-icon-2x.png";
import markerIcon from "leaflet/dist/images/marker-icon.png";
import markerShadow from "leaflet/dist/images/marker-shadow.png";

// Fix default icon paths in Leaflet when using webpack
L.Icon.Default.mergeOptions({
  iconRetinaUrl: markerIcon2x,
  iconUrl: markerIcon,
  shadowUrl: markerShadow,
});

const MapaView = () => {
  const [ubicaciones, setUbicaciones] = useState([]);
  const [selectedUbicacion, setSelectedUbicacion] = useState(null);
  const [showModal, setShowModal] = useState(false);
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

  const handleItemClick = (ubicacion) => {
    setSelectedUbicacion(ubicacion);
    setShowModal(true);
    if (mapRef.current) {
      mapRef.current.flyTo([ubicacion.latitud, ubicacion.longitud], 15);
    }
  };

  const handleMarkerClick = (ubicacion) => {
    setSelectedUbicacion(ubicacion);
    setShowModal(true);
    if (mapRef.current) {
      mapRef.current.flyTo([ubicacion.latitud, ubicacion.longitud], 15);
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
            {ubicaciones.map((u) => (
              <Marker
                key={u.id}
                position={[u.latitud, u.longitud]}
                eventHandlers={{ click: () => handleMarkerClick(u) }}
              >
                {selectedUbicacion?.id === u.id && <Popup>{u.nombre}</Popup>}
              </Marker>
            ))}
      </MapContainer>
      <div
        className="list-group overflow-auto border position-absolute top-0 end-0 bg-white"
        style={{ width: "300px", maxHeight: "100vh", margin: "1rem" }}
      >
          {ubicaciones.map((u) => (
            <button
              key={u.id}
              type="button"
              onClick={() => handleItemClick(u)}
              className={`list-group-item list-group-item-action ${selectedUbicacion?.id === u.id ? 'active' : ''}`}
            >
              {u.nombre}
            </button>
          ))}
        </div>
        <UbicacionModal
          isOpen={showModal}
          onClose={() => setShowModal(false)}
          ubicacion={selectedUbicacion}
        />
    </div>
  );
};

export default MapaView;
