import React, { useEffect, useState } from "react";
import Breadcrumb from "../layout/Breadcrumb";
import apiService from "../../services/apiService";

const PromocionesList = () => {
  const [promos, setPromos] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  useEffect(() => {
    const load = async () => {
      setLoading(true);
      try {
        const data = await apiService.getPromociones();
        setPromos(data);
      } catch (err) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    };
    load();
  }, []);

  return (
    <div style={{ maxWidth: "800px", margin: "2rem auto" }}>
      <Breadcrumb items={[{ label: "Home", to: "/" }, { label: "Promociones" }]} />
      <h2>Promociones</h2>
      {loading && <p>Cargando...</p>}
      {error && <p style={{ color: "red" }}>{error}</p>}
      {promos.map((p) => (
        <div key={p.id} style={{ border: "1px solid #ccc", padding: "1rem", borderRadius: "8px", marginBottom: "1rem" }}>
          <h4>{p.titulo}</h4>
          <p>{p.descripcion}</p>
          {p.precioEnPuntos && <p>Por {p.precioEnPuntos} puntos</p>}
          {p.descuentoEnPuntos && <p>Descuento: {p.descuentoEnPuntos}%</p>}
        </div>
      ))}
    </div>
  );
};

export default PromocionesList;