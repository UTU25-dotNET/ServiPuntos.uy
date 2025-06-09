import React, { useState, useEffect } from "react";
import apiService from "../../services/apiService";

const PreciosWidget = ({ tenantInfo }) => {
  const [preciosMinimos, setPreciosMinimos] = useState({
    super: null,
    premium: null,
    diesel: null
  });
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    loadPreciosMinimos();
  }, []);

  const loadPreciosMinimos = async () => {
    setLoading(true);
    setError("");
    
    try {
      
      // Obtener todas las ubicaciones del tenant
      const ubicaciones = await apiService.getUbicacionesByUserTenant();
      
      if (!ubicaciones || ubicaciones.length === 0) {
        setError("No se encontraron ubicaciones");
        return;
      }

      // Encontrar los precios mínimos para cada tipo de combustible
      const preciosCalculados = {
        super: null,
        premium: null,
        diesel: null
      };

      ubicaciones.forEach(ubicacion => {
        // Verificar que los precios existan y sean válidos (mayor a 0)
        const precioSuper = ubicacion.precioNaftaSuper;
        const precioPremium = ubicacion.precioNaftaPremium;
        const precioDiesel = ubicacion.precioDiesel;

        // Nafta Súper
        if (precioSuper && precioSuper > 0) {
          if (!preciosCalculados.super || precioSuper < preciosCalculados.super.precio) {
            preciosCalculados.super = {
              precio: precioSuper,
              ubicacion: ubicacion.nombre,
              ubicacionId: ubicacion.id,
              direccion: ubicacion.direccion,
              ciudad: ubicacion.ciudad,
              departamento: ubicacion.departamento
            };
          }
        }

        // Nafta Premium
        if (precioPremium && precioPremium > 0) {
          if (!preciosCalculados.premium || precioPremium < preciosCalculados.premium.precio) {
            preciosCalculados.premium = {
              precio: precioPremium,
              ubicacion: ubicacion.nombre,
              ubicacionId: ubicacion.id,
              direccion: ubicacion.direccion,
              ciudad: ubicacion.ciudad,
              departamento: ubicacion.departamento
            };
          }
        }

        // Diesel
        if (precioDiesel && precioDiesel > 0) {
          if (!preciosCalculados.diesel || precioDiesel < preciosCalculados.diesel.precio) {
            preciosCalculados.diesel = {
              precio: precioDiesel,
              ubicacion: ubicacion.nombre,
              ubicacionId: ubicacion.id,
              direccion: ubicacion.direccion,
              ciudad: ubicacion.ciudad,
              departamento: ubicacion.departamento
            };
          }
        }
      });

      setPreciosMinimos(preciosCalculados);
      
    } catch (err) {
      setError("Error al cargar los precios de combustible");
    } finally {
      setLoading(false);
    }
  };

  const formatPrice = (price) => {
    if (!price && price !== 0) return "N/A";
    return Number(price).toFixed(2);
  };

  if (loading) {
    return (
      <div style={{
        backgroundColor: "white",
        borderRadius: "12px",
        padding: "1.5rem",
        boxShadow: "0 4px 12px rgba(0,0,0,0.1)",
        border: "1px solid #e9ecef"
      }}>
        <div style={{
          display: "flex",
          alignItems: "center",
          gap: "0.75rem",
          marginBottom: "1rem"
        }}>
          <span style={{ fontSize: "1.5rem" }}>⛽</span>
          <h3 style={{
            margin: "0",
            color: "#7B3F00",
            fontSize: "1.2rem",
            fontWeight: "600"
          }}>
            Mejores Precios de Combustible
          </h3>
        </div>
        
        <div style={{
          display: "flex",
          justifyContent: "center",
          alignItems: "center",
          padding: "2rem"
        }}>
          <div style={{
            border: "3px solid #f3f3f3",
            borderTop: "3px solid #7B3F00",
            borderRadius: "50%",
            width: "30px",
            height: "30px",
            animation: "spin 1s linear infinite"
          }} />
        </div>
        
        <style>{`
          @keyframes spin {
            0% { transform: rotate(0deg); }
            100% { transform: rotate(360deg); }
          }
        `}</style>
      </div>
    );
  }

  if (error) {
    return (
      <div style={{
        backgroundColor: "white",
        borderRadius: "12px",
        padding: "1.5rem",
        boxShadow: "0 4px 12px rgba(0,0,0,0.1)",
        border: "1px solid #e9ecef"
      }}>
        <div style={{
          display: "flex",
          alignItems: "center",
          gap: "0.75rem",
          marginBottom: "1rem"
        }}>
          <span style={{ fontSize: "1.5rem" }}>⛽</span>
          <h3 style={{
            margin: "0",
            color: "#7B3F00",
            fontSize: "1.2rem",
            fontWeight: "600"
          }}>
            Mejores Precios de Combustible
          </h3>
        </div>
        
        <div style={{
          backgroundColor: "#fff2cd",
          border: "1px solid #ffeaa7",
          borderRadius: "8px",
          padding: "1rem",
          textAlign: "center"
        }}>
          <p style={{ 
            margin: "0", 
            color: "#856404",
            fontSize: "0.9rem" 
          }}>
            ⚠️ {error}
          </p>
        </div>
      </div>
    );
  }

  return (
    <div style={{
      backgroundColor: "white",
      borderRadius: "12px",
      padding: "1.5rem",
      boxShadow: "0 4px 12px rgba(0,0,0,0.1)",
      border: "1px solid #e9ecef"
    }}>
      {/* Header */}
      <div style={{
        display: "flex",
        alignItems: "center",
        gap: "0.75rem",
        marginBottom: "1.5rem"
      }}>
        <span style={{ fontSize: "1.5rem" }}>⛽</span>
        <h3 style={{
          margin: "0",
          color: "#7B3F00",
          fontSize: "1.2rem",
          fontWeight: "600"
        }}>
          Mejores Precios de Combustible
        </h3>
      </div>

      {/* Descripción */}
      <p style={{
        margin: "0 0 1.5rem 0",
        color: "#6c757d",
        fontSize: "0.9rem",
        lineHeight: "1.4"
      }}>
        Los precios más bajos disponibles en toda nuestra red de estaciones
      </p>

      {/* Lista de precios */}
      <div style={{ display: "grid", gap: "1rem" }}>
        
        {/* Nafta Súper */}
        <div style={{
          display: "flex",
          justifyContent: "space-between",
          alignItems: "flex-start",
          padding: "1rem",
          backgroundColor: "#e8f5e8",
          borderRadius: "8px",
          border: "1px solid #c3e6c3"
        }}>
          <div style={{ flex: 1 }}>
            <div style={{
              display: "flex",
              alignItems: "center",
              gap: "0.5rem",
              marginBottom: "0.25rem"
            }}>
              <span style={{ fontSize: "1.1rem" }}>🚗</span>
              <span style={{
                fontWeight: "600",
                color: "#2d5a2d",
                fontSize: "1rem"
              }}>
                Nafta Súper
              </span>
            </div>
            {preciosMinimos.super ? (
              <div style={{ fontSize: "0.8rem", color: "#6c757d" }}>
                📍 {preciosMinimos.super.ubicacion}
                <br />
                {preciosMinimos.super.ciudad}, {preciosMinimos.super.departamento}
              </div>
            ) : (
              <div style={{ fontSize: "0.8rem", color: "#6c757d" }}>
                No disponible
              </div>
            )}
          </div>
          <div style={{
            textAlign: "right",
            fontWeight: "bold",
            fontSize: "1.2rem",
            color: "#2d5a2d"
          }}>
            {preciosMinimos.super ? `$${formatPrice(preciosMinimos.super.precio)}` : "N/A"}
          </div>
        </div>

        {/* Nafta Premium */}
        <div style={{
          display: "flex",
          justifyContent: "space-between",
          alignItems: "flex-start",
          padding: "1rem",
          backgroundColor: "#fff9c4",
          borderRadius: "8px",
          border: "1px solid #f9c74f"
        }}>
          <div style={{ flex: 1 }}>
            <div style={{
              display: "flex",
              alignItems: "center",
              gap: "0.5rem",
              marginBottom: "0.25rem"
            }}>
              <span style={{ fontSize: "1.1rem" }}>⭐</span>
              <span style={{
                fontWeight: "600",
                color: "#b8860b",
                fontSize: "1rem"
              }}>
                Nafta Premium
              </span>
            </div>
            {preciosMinimos.premium ? (
              <div style={{ fontSize: "0.8rem", color: "#6c757d" }}>
                📍 {preciosMinimos.premium.ubicacion}
                <br />
                {preciosMinimos.premium.ciudad}, {preciosMinimos.premium.departamento}
              </div>
            ) : (
              <div style={{ fontSize: "0.8rem", color: "#6c757d" }}>
                No disponible
              </div>
            )}
          </div>
          <div style={{
            textAlign: "right",
            fontWeight: "bold",
            fontSize: "1.2rem",
            color: "#b8860b"
          }}>
            {preciosMinimos.premium ? `$${formatPrice(preciosMinimos.premium.precio)}` : "N/A"}
          </div>
        </div>

        {/* Diesel */}
        <div style={{
          display: "flex",
          justifyContent: "space-between",
          alignItems: "flex-start",
          padding: "1rem",
          backgroundColor: "#e3f2fd",
          borderRadius: "8px",
          border: "1px solid #90caf9"
        }}>
          <div style={{ flex: 1 }}>
            <div style={{
              display: "flex",
              alignItems: "center",
              gap: "0.5rem",
              marginBottom: "0.25rem"
            }}>
              <span style={{ fontSize: "1.1rem" }}>🚛</span>
              <span style={{
                fontWeight: "600",
                color: "#1565c0",
                fontSize: "1rem"
              }}>
                Diesel
              </span>
            </div>
            {preciosMinimos.diesel ? (
              <div style={{ fontSize: "0.8rem", color: "#6c757d" }}>
                📍 {preciosMinimos.diesel.ubicacion}
                <br />
                {preciosMinimos.diesel.ciudad}, {preciosMinimos.diesel.departamento}
              </div>
            ) : (
              <div style={{ fontSize: "0.8rem", color: "#6c757d" }}>
                No disponible
              </div>
            )}
          </div>
          <div style={{
            textAlign: "right",
            fontWeight: "bold",
            fontSize: "1.2rem",
            color: "#1565c0"
          }}>
            {preciosMinimos.diesel ? `$${formatPrice(preciosMinimos.diesel.precio)}` : "N/A"}
          </div>
        </div>
      </div>

      {/* Footer */}
      <div style={{
        marginTop: "1.5rem",
        padding: "0.75rem",
        backgroundColor: "#f8f9fa",
        borderRadius: "6px",
        textAlign: "center"
      }}>
        <p style={{
          margin: "0",
          fontSize: "0.8rem",
          color: "#6c757d"
        }}>
          💡 Precios actualizados en tiempo real
        </p>
      </div>
    </div>
  );
};

export default PreciosWidget;