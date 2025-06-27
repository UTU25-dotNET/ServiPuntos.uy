import React from "react";
import "./PreciosWidget.css";

const PriceRow = ({ icon, colorClass, label, data }) => {
  return (
    <div className={`price-row ${colorClass}`}> 
      <div className="price-info">
        <div className="price-label">
          <span className="price-icon">{icon}</span>
          <span className="price-name">{label}</span>
        </div>
        {data ? (
          <div className="price-location">
            📍 {data.ubicacion}
            <br />
            {data.ciudad}, {data.departamento}
          </div>
        ) : (
          <div className="price-location">No disponible</div>
        )}
      </div>
      <div className="price-value">
        {data ? `$${Number(data.precio).toFixed(2)}` : "N/A"}
      </div>
    </div>
  );
};

export default PriceRow;
