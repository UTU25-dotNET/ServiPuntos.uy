import React, { useEffect, useRef, useState } from "react";
import { MapContainer, TileLayer, Marker, Popup } from "react-leaflet";
import L from "leaflet";
import "leaflet/dist/leaflet.css";
import apiService from "../../services/apiService";

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
  const [selectedId, setSelectedId] = useState(null);
  const mapRef = useRef(null);

  useEffect(() => {
    const fetchUbicaciones = async () => {
      try {
        const data = await apiService.getAllUbicaciones();
        setUbicaciones(data);
      } catch {
        setUbicaciones([]);
      }
    };
    fetchUbicaciones();
  }, []);

  const handleItemClick = (ubicacion) => {
    setSelectedId(ubicacion.id);
    if (mapRef.current) {
      mapRef.current.flyTo([ubicacion.latitud, ubicacion.longitud], 15);
    }
  };

  const handleMarkerClick = (ubicacion) => {
    setSelectedId(ubicacion.id);
    if (mapRef.current) {
      mapRef.current.flyTo([ubicacion.latitud, ubicacion.longitud], 15);
    }
  };

  return (
    <div style={{ display: "flex", height: "80vh" }}>
      <div style={{ flex: 1 }}>
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
              {selectedId === u.id && <Popup>{u.nombre}</Popup>}
            </Marker>
          ))}
        </MapContainer>
      </div>
      <div
        style={{
          width: "300px",
          overflowY: "auto",
          padding: "1rem",
          background: "#f8f9fa",
        }}
      >
        {ubicaciones.map((u) => (
          <div
            key={u.id}
            onClick={() => handleItemClick(u)}
            style={{
              cursor: "pointer",
              padding: "0.5rem",
              marginBottom: "0.5rem",
              borderRadius: "4px",
              border: "1px solid #ccc",
              background: selectedId === u.id ? "#e2e6ea" : "#fff",
            }}
          >
            {u.nombre}
          </div>
        ))}
      </div>
    </div>
  );
};

export default MapaView;
