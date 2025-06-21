import React, { useState, useEffect, useCallback } from "react";
import apiService from "../../services/apiService";
import PriceRow from "./PriceRow";
import "./PreciosWidget.css";

const PreciosWidget = () => {
  const [preciosMinimos, setPreciosMinimos] = useState({
    super: null,
    premium: null,
    diesel: null
  });
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  const loadPreciosMinimos = useCallback(async () => {
    setLoading(true);
    setError("");
    try {
      const ubicaciones = await apiService.getUbicacionesByUserTenant();
      if (!ubicaciones || ubicaciones.length === 0) {
        setError("No se encontraron ubicaciones");
        return;
      }

      const preciosCalculados = ubicaciones.reduce(
        (acc, u) => {
          const check = (tipo, valor) => {
            if (valor && valor > 0 && (!acc[tipo] || valor < acc[tipo].precio)) {
              acc[tipo] = {
                precio: valor,
                ubicacion: u.nombre,
                ubicacionId: u.id,
                direccion: u.direccion,
                ciudad: u.ciudad,
                departamento: u.departamento
              };
            }
          };
          check("super", u.precioNaftaSuper);
          check("premium", u.precioNaftaPremium);
          check("diesel", u.precioDiesel);
          return acc;
        },
        { super: null, premium: null, diesel: null }
      );

      setPreciosMinimos(preciosCalculados);
    } catch (err) {
      setError("Error al cargar los precios de combustible");
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    loadPreciosMinimos();
  }, [loadPreciosMinimos]);

  if (loading) {
    return (
      <div className="container">
        <div className="header">
          <span style={{ fontSize: "1.5rem" }}>⛽</span>
          <h3 className="header-title">Mejores Precios de Combustible</h3>
        </div>
        <div className="loader">
          <div className="spinner" />
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="container">
        <div className="header">
          <span style={{ fontSize: "1.5rem" }}>⛽</span>
          <h3 className="header-title">Mejores Precios de Combustible</h3>
        </div>
        <div className="error">
          <div className="error-box">
            <p className="error-text">⚠️ {error}</p>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="container">
      <div className="header">
        <span style={{ fontSize: "1.5rem" }}>⛽</span>
        <h3 className="header-title">Mejores Precios de Combustible</h3>
      </div>
      <p className="description">
        Los precios más bajos disponibles en toda nuestra red de estaciones
      </p>
      <div className="list">
        <PriceRow
          icon="🚗"
          colorClass="price-super"
          label="Nafta Súper"
          data={preciosMinimos.super}
        />
        <PriceRow
          icon="⭐"
          colorClass="price-premium"
          label="Nafta Premium"
          data={preciosMinimos.premium}
        />
        <PriceRow
          icon="🚛"
          colorClass="price-diesel"
          label="Diesel"
          data={preciosMinimos.diesel}
        />
      </div>
      <div className="footer">
        <p className="footer-text">💡 Precios actualizados en tiempo real</p>
      </div>
    </div>
  );
};

export default PreciosWidget;
