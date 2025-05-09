import React from "react";
import { Link } from "react-router-dom";
import authService from "../services/authService";

const NavBar = () => {
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
        marginBottom: "1rem",
      }}
    >
      <div>
        <Link
          to="/"
          style={{
            color: "white",
            textDecoration: "none",
            fontWeight: "bold",
            fontSize: "1.2rem",
          }}
        >
          Servipuntos.uy <span style={{ color: "grey" }}>*Demo*</span>
        </Link>
      </div>
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
                  fontSize: "0.8rem",
                }}
              >
                {user.username || user.email}
              </span>
            )}
            {/* <Link to="/dashboard" style={{ 
              color: 'white', 
              textDecoration: 'none',
              marginRight: '1rem'
            }}>
              Dashboard
            </Link> */}
            {/* <Link to="/token" style={{ 
              color: 'white', 
              textDecoration: 'none',
              marginRight: '1rem'
            }}>
              Ver Token
            </Link> */}
            <Link
              to="/"
              onClick={() => authService.logout()}
              style={{
                color: "white",
                textDecoration: "none",
              }}
            >
              Cerrar Sesión
            </Link>
          </>
        ) : (
          <>
            {/* <Link to="/" style={{ 
              color: 'white', 
              textDecoration: 'none',
              marginRight: '1rem'
            }}>
              Inicio
            </Link> */}
            {/* <Link to="/login" style={{ 
              color: 'white', 
              textDecoration: 'none'
            }}>
              Iniciar Sesión
            </Link> */}
          </>
        )}
      </div>
    </nav>
  );
};

export default NavBar;
