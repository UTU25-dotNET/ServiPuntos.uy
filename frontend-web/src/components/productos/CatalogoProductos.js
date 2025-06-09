import React, { useState, useEffect } from "react";
import { QRCodeSVG } from "qrcode.react";
import apiService from "../../services/apiService";

const CatalogoProductos = ({ ubicacion, onClose, isOpen, userProfile }) => {
  const [productos, setProductos] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [canjeQR, setCanjeQR] = useState(null);
  const [canjeLoading, setCanjeLoading] = useState(false);
  const [canjeError, setCanjeError] = useState("");
  const [carrito, setCarrito] = useState([]);
  const [qrCarrito, setQrCarrito] = useState([]);
  const [carritoLoading, setCarritoLoading] = useState(false);
  const [carritoError, setCarritoError] = useState("");

  useEffect(() => {
    const loadProductos = async () => {
      setLoading(true);
      setError("");

      try {
        const productosData = await apiService.getProductosByUbicacion(ubicacion.id);
        setProductos(productosData);
      } catch (err) {
        setError(err.message);
        setProductos([]);
      } finally {
        setLoading(false);
      }
    };

    if (isOpen && ubicacion) {
      loadProductos();
    }
  }, [isOpen, ubicacion]);

  const handleCanjear = async (productoId) => {
    setCanjeLoading(true);
    setCanjeError("");
    try {
      const result = await apiService.generarCanje(productoId, ubicacion.id);
      setCanjeQR(result?.datos?.codigoQR || null);
    } catch (err) {
      setCanjeError(err.message);
    } finally {
      setCanjeLoading(false);
    }
  };

  const agregarAlCarrito = (producto) => {
    setCarrito((prev) => [...prev, producto]);
  };

  const quitarDelCarrito = (productoId) => {
    setCarrito((prev) => prev.filter((p) => p.id !== productoId));
  };

  const handleCanjearCarrito = async () => {
    setCarritoLoading(true);
    setCarritoError("");
    try {
      const ids = carrito.map((p) => p.productoCanjeable.id);
      const result = await apiService.generarCanjes(ids, ubicacion.id);
      setQrCarrito(result?.datos?.resultados || []);
      setCarrito([]);
    } catch (err) {
      setCarritoError(err.message);
    } finally {
      setCarritoLoading(false);
    }
  };

  // Función para formatear el costo en puntos
  const formatCosto = (costo) => {
    if (!costo || costo === 0) return "Gratis";
    return `${costo.toLocaleString()} pts`;
  };

  // Función para determinar el color del stock
  const getStockColor = (stock) => {
    if (stock === 0) return "#dc3545"; // Rojo
    if (stock <= 5) return "#fd7e14";  // Naranja
    if (stock <= 10) return "#ffc107"; // Amarillo
    return "#28a745"; // Verde
  };

  // Función para obtener el texto del stock
  const getStockText = (stock) => {
    if (stock === 0) return "Agotado";
    if (stock <= 5) return "Pocas unidades";
    if (stock <= 10) return "Stock limitado";
    return "Disponible";
  };

  if (!isOpen) return null;

  return (
    <div
      style={{
        position: "fixed",
        top: 0,
        left: 0,
        right: 0,
        bottom: 0,
        backgroundColor: "rgba(0, 0, 0, 0.7)",
        display: "flex",
        justifyContent: "center",
        alignItems: "center",
        zIndex: 1000,
        padding: "1rem"
      }}
      onClick={onClose}
    >
      <div
        style={{
          backgroundColor: "white",
          borderRadius: "16px",
          maxWidth: "900px",
          width: "100%",
          maxHeight: "80vh",
          overflow: "hidden",
          boxShadow: "0 20px 60px rgba(0, 0, 0, 0.3)",
          display: "flex",
          flexDirection: "column"
        }}
        onClick={(e) => e.stopPropagation()}
      >
        {/* Header del modal */}
        <div
          style={{
            padding: "1.5rem 2rem",
            borderBottom: "2px solid #f8f9fa",
            backgroundColor: "#f8f9fa",
            borderRadius: "16px 16px 0 0"
          }}
        >
          <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center" }}>
            <div>
              <h2
                style={{
                  margin: "0 0 0.5rem 0",
                  color: "#212529",
                  fontSize: "1.5rem",
                  fontWeight: "600",
                  display: "flex",
                  alignItems: "center",
                  gap: "0.5rem"
                }}
              >
                🛒 Catálogo de Productos
              </h2>
              <p
                style={{
                  margin: "0",
                  color: "#6c757d",
                  fontSize: "1rem",
                  display: "flex",
                  alignItems: "center",
                  gap: "0.25rem"
                }}
              >
                🏪 {ubicacion.nombre} • 📍 {ubicacion.ciudad}
              </p>
            </div>
            <button
              onClick={onClose}
              style={{
                background: "none",
                border: "none",
                fontSize: "2rem",
                cursor: "pointer",
                color: "#6c757d",
                padding: "0.25rem",
                borderRadius: "50%",
                width: "3rem",
                height: "3rem",
                display: "flex",
                alignItems: "center",
                justifyContent: "center",
                transition: "all 0.2s ease"
              }}
              onMouseEnter={(e) => {
                e.target.style.backgroundColor = "#f8f9fa";
                e.target.style.color = "#212529";
              }}
              onMouseLeave={(e) => {
                e.target.style.backgroundColor = "transparent";
                e.target.style.color = "#6c757d";
              }}
            >
              ×
            </button>
          </div>
        </div>

        {canjeQR && (
          <div
            style={{
              position: "absolute",
              top: 0,
              left: 0,
              right: 0,
              bottom: 0,
              backgroundColor: "rgba(0,0,0,0.6)",
              display: "flex",
              justifyContent: "center",
              alignItems: "center",
              zIndex: 2000,
            }}
          >
            <div
              style={{
                background: "white",
                padding: "2rem",
                borderRadius: "12px",
                textAlign: "center",
              }}
            >
              <h3 style={{ marginTop: 0 }}>Código de Canje</h3>
              {canjeLoading ? (
                <p>Generando código...</p>
              ) : (
                <>
                  <QRCodeSVG value={canjeQR} size={180} />
                  <p style={{ wordBreak: "break-all" }}>{canjeQR}</p>
                </>
              )}
              {canjeError && (
                <p style={{ color: "#dc3545" }}>{canjeError}</p>
              )}
              <button
                onClick={() => setCanjeQR(null)}
                style={{ marginTop: "1rem", padding: "0.5rem 1rem" }}
              >
                Cerrar
              </button>
            </div>
          </div>
        )}

        {qrCarrito.length > 0 && (
          <div
            style={{
              position: "absolute",
              top: 0,
              left: 0,
              right: 0,
              bottom: 0,
              backgroundColor: "rgba(0,0,0,0.6)",
              display: "flex",
              justifyContent: "center",
              alignItems: "center",
              zIndex: 2000,
              overflow: "auto"
            }}
          >
            <div
              style={{
                background: "white",
                padding: "2rem",
                borderRadius: "12px",
                textAlign: "center",
                maxWidth: "90%"
              }}
            >
              <h3 style={{ marginTop: 0 }}>Códigos de Canje</h3>
              {carritoLoading ? (
                <p>Generando códigos...</p>
              ) : (
                <div style={{ display: "grid", gridTemplateColumns: "repeat(auto-fit, minmax(180px, 1fr))", gap: "1rem" }}>
                  {qrCarrito.map((q) => (
                    <div key={q.productoId} style={{ padding: "1rem", border: "1px solid #e9ecef", borderRadius: "8px" }}>
                      {q.codigoQR ? (
                        <>
                          <QRCodeSVG value={q.codigoQR} size={160} />
                          <p style={{ wordBreak: "break-all" }}>{q.codigoQR}</p>
                        </>
                      ) : (
                        <p style={{ color: "#dc3545" }}>{q.error}</p>
                      )}
                    </div>
                  ))}
                </div>
              )}
              <button
                onClick={() => setQrCarrito([])}
                style={{ marginTop: "1rem", padding: "0.5rem 1rem" }}
              >
                Cerrar
              </button>
            </div>
          </div>
        )}

        {/* Contenido del modal */}
        <div
          style={{
            padding: "1.5rem 2rem",
            overflow: "auto",
            flex: 1
          }}
        >
          {loading && (
            <div style={{ textAlign: "center", padding: "3rem" }}>
              <div
                style={{
                  border: "4px solid #f3f3f3",
                  borderTop: "4px solid #007bff",
                  borderRadius: "50%",
                  width: "50px",
                  height: "50px",
                  animation: "spin 1s linear infinite",
                  margin: "0 auto 1rem"
                }}
              />
              <p style={{ color: "#007bff", fontSize: "1.1rem" }}>Cargando catálogo...</p>

              <style>{`
                @keyframes spin {
                  0% { transform: rotate(0deg); }
                  100% { transform: rotate(360deg); }
                }
              `}</style>
            </div>
          )}

          {error && (
            <div
              style={{
                backgroundColor: "#f8d7da",
                color: "#721c24",
                padding: "1.5rem",
                borderRadius: "8px",
                marginBottom: "1rem",
                border: "1px solid #f5c6cb",
                textAlign: "center"
              }}
            >
              <strong>⚠️ Error:</strong> {error}
            </div>
          )}

          {!loading && !error && productos.length === 0 && (
            <div
              style={{
                backgroundColor: "#fff3cd",
                color: "#856404",
                padding: "3rem",
                borderRadius: "12px",
                textAlign: "center",
                border: "1px solid #ffeaa7"
              }}
            >
              <div style={{ fontSize: "4rem", marginBottom: "1rem" }}>📦</div>
              <h4 style={{ margin: "0 0 1rem 0", fontSize: "1.25rem" }}>
                Sin productos disponibles
              </h4>
              <p style={{ margin: "0", fontSize: "1rem" }}>
                Esta estación no tiene productos disponibles para canje en este momento.
              </p>
            </div>
          )}

          {!loading && productos.length > 0 && (
            <>
              {/* Información resumida */}
              <div
                style={{
                  marginBottom: "1.5rem",
                  padding: "1rem",
                  backgroundColor: "#e3f2fd",
                  borderRadius: "8px",
                  border: "1px solid #bbdefb",
                  textAlign: "center"
                }}
              >
                <p
                  style={{
                    margin: "0",
                    color: "#1976d2",
                    fontWeight: "600",
                    fontSize: "1.1rem"
                  }}
                >
                  💰 {productos.length} producto{productos.length !== 1 ? "s" : ""} disponible{productos.length !== 1 ? "s" : ""} para canje
                </p>
              </div>

              {carrito.length > 0 && (
                <div
                  style={{
                    marginBottom: "1rem",
                    padding: "1rem",
                    backgroundColor: "#e8f5e9",
                    borderRadius: "8px",
                    border: "1px solid #c8e6c9"
                  }}
                >
                  <h5 style={{ marginTop: 0 }}>Carrito ({carrito.length})</h5>
                  <ul style={{ listStyle: "none", padding: 0 }}>
                    {carrito.map((p) => (
                      <li key={p.id} style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: "0.25rem" }}>
                        <span>{p.productoCanjeable.nombre}</span>
                        <button onClick={() => quitarDelCarrito(p.id)} style={{ border: "none", background: "transparent", color: "#dc3545", cursor: "pointer" }}>✕</button>
                      </li>
                    ))}
                  </ul>
                  <button onClick={handleCanjearCarrito} disabled={carritoLoading}
                    style={{
                      marginTop: "0.5rem",
                      padding: "0.5rem 1rem",
                      backgroundColor: "#28a745",
                      color: "white",
                      border: "none",
                      borderRadius: "6px",
                      cursor: "pointer"
                    }}>
                    Canjear carrito
                  </button>
                  {carritoError && <p style={{ color: "#dc3545" }}>{carritoError}</p>}
                </div>
              )}

              {/* Grid de productos */}
              <div
                style={{
                  display: "grid",
                  gridTemplateColumns: "repeat(auto-fill, minmax(280px, 1fr))",
                  gap: "1.5rem"
                }}
              >
                {productos.map((productoUbicacion) => {
                  const producto = productoUbicacion.productoCanjeable;
                  if (!producto) return null;

                  return (
                    <div
                      key={productoUbicacion.id}
                      style={{
                        backgroundColor: "white",
                        border: "2px solid #e9ecef",
                        borderRadius: "12px",
                        padding: "1.5rem",
                        boxShadow: "0 2px 8px rgba(0,0,0,0.1)",
                        transition: "all 0.3s ease",
                        opacity: productoUbicacion.activo ? 1 : 0.6
                      }}
                      onMouseEnter={(e) => {
                        if (productoUbicacion.activo) {
                          e.currentTarget.style.transform = "translateY(-2px)";
                          e.currentTarget.style.boxShadow = "0 4px 20px rgba(0,0,0,0.15)";
                          e.currentTarget.style.borderColor = "#007bff";
                        }
                      }}
                      onMouseLeave={(e) => {
                        e.currentTarget.style.transform = "translateY(0)";
                        e.currentTarget.style.boxShadow = "0 2px 8px rgba(0,0,0,0.1)";
                        e.currentTarget.style.borderColor = "#e9ecef";
                      }}
                    >
                      {/* Header del producto */}
                      <div style={{ marginBottom: "1rem" }}>
                        <h4
                          style={{
                            margin: "0 0 0.5rem 0",
                            color: "#212529",
                            fontSize: "1.2rem",
                            fontWeight: "600",
                            lineHeight: "1.3"
                          }}
                        >
                          {producto.nombre}
                        </h4>

                        {producto.descripcion && (
                          <p
                            style={{
                              margin: "0",
                              color: "#6c757d",
                              fontSize: "0.9rem",
                              lineHeight: "1.4"
                            }}
                          >
                            {producto.descripcion}
                          </p>
                        )}
                      </div>

                      {/* Costo en puntos */}
                      <div
                        style={{
                          marginBottom: "1rem",
                          padding: "0.75rem",
                          backgroundColor: "#fff3cd",
                          borderRadius: "8px",
                          border: "1px solid #ffeaa7",
                          textAlign: "center"
                        }}
                      >
                        <div
                          style={{
                            fontSize: "1.5rem",
                            fontWeight: "bold",
                            color: "#856404",
                            display: "flex",
                            alignItems: "center",
                            justifyContent: "center",
                            gap: "0.5rem"
                          }}
                        >
                          💰 {formatCosto(producto.costoEnPuntos)}
                        </div>
                      </div>

                      {/* Estado del stock */}
                      <div
                        style={{
                          display: "flex",
                          justifyContent: "space-between",
                          alignItems: "center",
                          padding: "0.75rem",
                          backgroundColor: "#f8f9fa",
                          borderRadius: "8px",
                          marginBottom: "1rem"
                        }}
                      >
                        <div style={{ display: "flex", flexDirection: "column" }}>
                          <span
                            style={{
                              fontSize: "0.8rem",
                              color: "#6c757d",
                              fontWeight: "500"
                            }}
                          >
                            Stock disponible:
                          </span>
                          <span
                            style={{
                              fontSize: "1.1rem",
                              fontWeight: "bold",
                              color: getStockColor(productoUbicacion.stockDisponible)
                            }}
                          >
                            {productoUbicacion.stockDisponible} unidades
                          </span>
                        </div>
                        <div
                          style={{
                            padding: "0.25rem 0.75rem",
                            borderRadius: "12px",
                            fontSize: "0.8rem",
                            fontWeight: "600",
                            color: "white",
                            backgroundColor: getStockColor(productoUbicacion.stockDisponible)
                          }}
                        >
                          {getStockText(productoUbicacion.stockDisponible)}
                        </div>
                      </div>

                      {/* Estado activo/inactivo */}
                      {!productoUbicacion.activo && (
                        <div
                          style={{
                            padding: "0.5rem",
                            backgroundColor: "#f8d7da",
                            borderRadius: "6px",
                            border: "1px solid #f5c6cb",
                            textAlign: "center",
                            fontSize: "0.9rem",
                            color: "#721c24",
                            fontWeight: "500"
                          }}
                        >
                          ⚠️ Producto temporalmente no disponible
                        </div>
                      )}

                      {productoUbicacion.activo && productoUbicacion.stockDisponible > 0 && (
                        <div style={{ display: "flex", flexDirection: "column", gap: "0.5rem" }}>
                          <button
                            onClick={() => handleCanjear(producto.id)}
                            disabled={!userProfile || (userProfile.puntos || 0) < producto.costoEnPuntos || canjeLoading}
                            style={{
                              width: "100%",
                              padding: "0.5rem",
                              backgroundColor: "#007bff",
                              color: "white",
                              border: "none",
                              borderRadius: "6px",
                              cursor: "pointer"
                            }}
                          >
                            Canjear
                          </button>
                          <button
                            onClick={() => agregarAlCarrito(productoUbicacion)}
                            disabled={carritoLoading}
                            style={{
                              width: "100%",
                              padding: "0.5rem",
                              backgroundColor: "#28a745",
                              color: "white",
                              border: "none",
                              borderRadius: "6px",
                              cursor: "pointer"
                            }}
                          >
                            Agregar al carrito
                          </button>
                        </div>
                      )}

                      {/* Footer con información adicional */}
                      <div
                        style={{
                          marginTop: "1rem",
                          paddingTop: "1rem",
                          borderTop: "1px solid #f8f9fa",
                          fontSize: "0.8rem",
                          color: "#6c757d",
                          textAlign: "center"
                        }}
                      >
                        ID: {productoUbicacion.id.slice(0, 8)}...
                      </div>
                    </div>
                  );
                })}
              </div>
            </>
          )}
        </div>

        {/* Footer del modal */}
        <div
          style={{
            padding: "1rem 2rem",
            borderTop: "1px solid #f8f9fa",
            backgroundColor: "#f8f9fa",
            borderRadius: "0 0 16px 16px",
            textAlign: "center"
          }}
        >
          <button
            onClick={onClose}
            style={{
              backgroundColor: "#6c757d",
              color: "white",
              border: "none",
              borderRadius: "8px",
              padding: "0.75rem 2rem",
              fontSize: "1rem",
              fontWeight: "500",
              cursor: "pointer",
              transition: "background-color 0.2s ease"
            }}
            onMouseEnter={(e) => (e.target.style.backgroundColor = "#5a6268")}
            onMouseLeave={(e) => (e.target.style.backgroundColor = "#6c757d")}
          >
            Cerrar
          </button>
        </div>
      </div>
    </div>
  );
};

export default CatalogoProductos;