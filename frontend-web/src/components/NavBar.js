import React from "react";
import { Link } from "react-router-dom";
import authService from "../services/authService";
import TenantSelector from "./TenantSelector";

const NavBar = ({
  tenants,
  selectedTenant,
  onSelectTenant
}) => {
  const isAuthenticated = authService.isAuthenticated();
  const user = authService.getCurrentUser();

  return (
    <nav
      style={{
        display: "flex",
        justifyContent: "space-between",
        padding: "1rem",
        backgroundColor: "#343a40",
        color: "white",
        marginBottom: "1rem"
      }}
    >
      <div>
        <Link
          to="/"
          style={{
            color: "white",
            textDecoration: "none",
            fontWeight: "bold",
            fontSize: "1.2rem"
          }}
        >
          Servipuntos.uy <span style={{ color: "grey" }}>*Demo*</span>
        </Link>
      </div>

      {/* ——— Dropdown de Tenants ——— */}
      <div>
        <TenantSelector
          tenants={tenants}
          selectedTenant={selectedTenant}
          onSelect={onSelectTenant}
        />
      </div>
      {/* ———————————————————————— */}

      <div style={{ display: "flex", alignItems: "center" }}>
        {isAuthenticated ? (
          <>
            {user && (
              <span
                style={{
                  marginRight: "1rem",
                  backgroundColor:
                    user.role === "admin" ? "#6f42c1" : "#28a745",
                  color: "white",
                  padding: "0.25rem 0.5rem",
                  borderRadius: "4px",
                  fontSize: "0.8rem"
                }}
              >
                {user.username || user.email}
              </span>
            )}
            <Link
              to="/"
              onClick={() => authService.logout()}
              style={{
                color: "white",
                textDecoration: "none"
              }}
            >
              Cerrar Sesión
            </Link>
          </>
        ) : (
          <>
            {/* aquí podrías poner links públicos si quisieras */}
          </>
        )}
      </div>
    </nav>
  );
};

export default NavBar;
