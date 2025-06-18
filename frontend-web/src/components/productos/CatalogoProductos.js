import React, { useState, useEffect } from "react";
import { QRCodeSVG } from "qrcode.react";
import apiService from "../../services/apiService";
import SeleccionarPuntosModal from "./SeleccionarPuntosModal";

const CatalogoProductos = ({ ubicacion, onClose, isOpen, userProfile, onProfileUpdated }) => {
  const [productos, setProductos] = useState([]);
  const [categorias, setCategorias] = useState([]);
  const [categoriaSeleccionada, setCategoriaSeleccionada] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [canjeQR, setCanjeQR] = useState(null);
  const [canjeLoading, setCanjeLoading] = useState(false);
  const [canjeError, setCanjeError] = useState("");
  const [carrito, setCarrito] = useState([]);
  const [qrCarrito, setQrCarrito] = useState([]);
  const [carritoLoading, setCarritoLoading] = useState(false);
  const [carritoError, setCarritoError] = useState("");
  const [compraLoading, setCompraLoading] = useState(false);
  const [compraError, setCompraError] = useState("");
  const [tenantInfo, setTenantInfo] = useState(null);
  const [mostrarPuntosModal, setMostrarPuntosModal] = useState(false);
  const [productoMixto, setProductoMixto] = useState(null);
  const [maxPuntosMixto, setMaxPuntosMixto] = useState(0);
  const [mostrarPuntosCarrito, setMostrarPuntosCarrito] = useState(false);
  const [maxPuntosCarrito, setMaxPuntosCarrito] = useState(0);
  const [esMayorDeEdad, setEsMayorDeEdad] = useState(true);

  const categorias18 = [
    'cigarros',
    'cigarrillos',
    'bebidas alcoholicas',
    'bebidas alcohólicas'
  ];

  const esCategoriaRestringida = (cat) => {
    if (!cat) return false;
    const c = cat.toLowerCase();
    return categorias18.some((v) => c.includes(v));
  };

  const ensureAgeVerified = async (categoria) => {
    if (!esCategoriaRestringida(categoria)) return true;
    if (userProfile?.ci && userProfile?.verificadoVEAI) return true;

    let ci = userProfile?.ci;

    try {
      const result = await apiService.verifyAge(ci);
      if (!result.isAllowed) {
        alert('Debes ser mayor de edad para acceder a este producto.');
        return false;
      }
      await apiService.updateUserProfile({ ci, verificadoVEAI: true });
      if (onProfileUpdated) await onProfileUpdated();
      return true;
    } catch (err) {
      alert(err.message || 'Error al verificar la edad');
      return false;
    }
  };

  useEffect(() => {
    const loadProductos = async () => {
      setLoading(true);
      setError("");

      try {
        let productosData = await apiService.getProductosByUbicacion(ubicacion.id);

        if (!esMayorDeEdad) {
          productosData = productosData.filter(p => !esCategoriaRestringida(p.categoria));
        }
        setProductos(productosData);

        const cats = [...new Set(productosData.map(p => p.categoria))];
        setCategorias(cats);
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
  }, [isOpen, ubicacion, esMayorDeEdad]);

  // Cargar información del tenant cuando se abre el catálogo
  useEffect(() => {
    const loadTenantInfo = async () => {
      try {
        const tenant = await apiService.getTenantInfo();
        setTenantInfo(tenant);
      } catch (err) {
        // Silenciar error para no interrumpir la experiencia del usuario
      }
    };

    if (isOpen) {
      loadTenantInfo();
    }
  }, [isOpen]);

  useEffect(() => {
    const verificarEdad = async () => {
      if (!userProfile?.ci) {
        setEsMayorDeEdad(true);
        return;
      }
      try {
        const result = await apiService.verifyAge(userProfile.ci);
        setEsMayorDeEdad(result.isAllowed);
      } catch {
        setEsMayorDeEdad(true);
      }
    };

    if (isOpen) {
      verificarEdad();
    }
  }, [isOpen, userProfile]);

  useEffect(() => {
    const params = new URLSearchParams(window.location.search);
    const paymentId = params.get('paymentId');
    const payerId = params.get('PayerID');
    if (paymentId && payerId) {
      apiService.confirmarPagoPaypal(paymentId, payerId)
        .then(() => {
          window.history.replaceState({}, document.title, window.location.pathname);
        })
        .catch(err => {
          setCompraError(err.message);
        });
    }
  }, []);

  const handleCanjear = async (productoUbicacion) => {
    if (!(await ensureAgeVerified(productoUbicacion.categoria))) return;
    setCanjeLoading(true);
    setCanjeError("");
    try {
      const result = await apiService.generarCanje(
        productoUbicacion.productoCanjeable.id,
        ubicacion.id
      );
      setCanjeQR(result?.datos?.codigoQR || null);
    } catch (err) {
      setCanjeError(err.message);
    } finally {
      setCanjeLoading(false);
    }
  };

  const handleComprar = async (productoUbicacion, puntos = 0) => {
    if (!(await ensureAgeVerified(productoUbicacion.categoria))) return;
    setCompraLoading(true);
    setCompraError("");
    try {
      const result = await apiService.procesarTransaccion(
        productoUbicacion,
        ubicacion.id,
        puntos,
        tenantInfo?.valorPunto || 0
      );
      if (result?.codigo === "PENDING_PAYMENT" && result.datos?.approvalUrl) {
        window.location.href = result.datos.approvalUrl;
      } else if (result?.codigo !== "OK") {
        setCompraError(result?.mensaje || "Error en la compra");
      }
    } catch (err) {
      setCompraError(err.message);
    } finally {
      setCompraLoading(false);
    }
  };

  const handleComprarMixto = async (productoUbicacion) => {
    if (!tenantInfo || !userProfile) return;
    if (!(await ensureAgeVerified(productoUbicacion.categoria))) return;
    const valorPunto = tenantInfo.valorPunto || 0;
    const puntosUsuario = userProfile.puntos || 0;
    const maxPuntos = Math.min(
      puntosUsuario,
      Math.floor(productoUbicacion.precio / valorPunto)
    );
    if (maxPuntos <= 0) {
      return handleComprar(productoUbicacion);
    }
    setProductoMixto(productoUbicacion);
    setMaxPuntosMixto(maxPuntos);
    setMostrarPuntosModal(true);
  };

  const confirmarCompraMixta = async (puntos) => {
    setMostrarPuntosModal(false);
    if (!productoMixto) return;
    await handleComprar(productoMixto, puntos);
    setProductoMixto(null);
  };

  const agregarAlCarrito = (producto) => {
    setCarrito((prev) => [...prev, producto]);
  };

  const quitarDelCarrito = (productoId) => {
    setCarrito((prev) => {
      const index = prev.findIndex((p) => p.id === productoId);
      if (index === -1) return prev;
      const updated = [...prev];
      updated.splice(index, 1);
      return updated;
    });
  };

  const handleCanjearCarrito = async () => {
    if (carrito.some((p) => esCategoriaRestringida(p.categoria))) {
      const first = carrito.find((p) => esCategoriaRestringida(p.categoria));
      if (!(await ensureAgeVerified(first.categoria))) return;
    }
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

  const handleComprarCarrito = async (puntos = 0) => {
    if (carrito.some((p) => esCategoriaRestringida(p.categoria))) {
      const first = carrito.find((p) => esCategoriaRestringida(p.categoria));
      if (!(await ensureAgeVerified(first.categoria))) return;
    }
    setCompraLoading(true);
    setCompraError("");
    try {
      let result;
      if (puntos > 0) {
        result = await apiService.procesarTransaccionMultipleMixto(
          carrito,
          ubicacion.id,
          puntos,
          tenantInfo?.valorPunto || 0
        );
      } else {
        result = await apiService.procesarTransaccionMultiple(carrito, ubicacion.id);
      }
      if (result?.codigo === "PENDING_PAYMENT" && result.datos?.approvalUrl) {
        window.location.href = result.datos.approvalUrl;
      } else if (result?.codigo !== "OK") {
        setCompraError(result?.mensaje || "Error en la compra");
      } else {
        setCarrito([]);
      }
    } catch (err) {
      setCompraError(err.message);
    } finally {
      setCompraLoading(false);
    }
  };

  const handleComprarCarritoMixto = async () => {
    if (!tenantInfo || !userProfile || carrito.length === 0) return;
    if (carrito.some((p) => esCategoriaRestringida(p.categoria))) {
      const first = carrito.find((p) => esCategoriaRestringida(p.categoria));
      if (!(await ensureAgeVerified(first.categoria))) return;
    }
    const valorPunto = tenantInfo.valorPunto || 0;
    const puntosUsuario = userProfile.puntos || 0;
    const total = carrito.reduce((sum, p) => sum + (p.precio || 0), 0);
    const max = Math.min(puntosUsuario, Math.floor(total / valorPunto));
    if (max <= 0) return handleComprarCarrito();
    setMaxPuntosCarrito(max);
    setMostrarPuntosCarrito(true);
  };

  const confirmarCompraCarritoMixto = async (puntos) => {
    setMostrarPuntosCarrito(false);
    await handleComprarCarrito(puntos);
  };

  const totalCarrito = carrito.reduce((sum, p) => sum + (p.precio || 0), 0);


  // Función para formatear el costo en puntos
  const formatCosto = (costo) => {
    if (!costo || costo === 0) return "Gratis";
    return `${costo.toLocaleString()} pts`;
  };

  // Formatea el precio en pesos uruguayos
  const formatPrecio = (precio) => {
    if (precio === null || precio === undefined) return "-";
    return `$ ${precio.toFixed(2)}`;
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
    <><div
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
          maxWidth: "1200px",
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
              } }
              onMouseLeave={(e) => {
                e.target.style.backgroundColor = "transparent";
                e.target.style.color = "#6c757d";
              } }
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
                  borderTop: "4px solid var(--primary-color)",
                  borderRadius: "50%",
                  width: "50px",
                  height: "50px",
                  animation: "spin 1s linear infinite",
                  margin: "0 auto 1rem"
                }} />
              <p style={{ color: "var(--primary-color)", fontSize: "1.1rem" }}>Cargando catálogo...</p>

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
            

              {categorias.length > 0 && (
                <div className="mb-3 text-center">
                  <label htmlFor="categoriaFiltro" className="form-label me-2">
                    Filtrar por categoría:
                  </label>
                  <select
                    id="categoriaFiltro"
                    className="form-select d-inline-block w-auto"
                    value={categoriaSeleccionada}
                    onChange={(e) => setCategoriaSeleccionada(e.target.value)}
                  >
                    <option value="">Todas</option>
                    {categorias.map((c) => (
                      <option key={c} value={c}>{c}</option>
                    ))}
                  </select>
                </div>
              )}

              {carrito.length > 0 && (
                <div className="card mb-3">
                  <div className="card-body">
                    <h5 className="card-title">Carrito ({carrito.length})</h5>
                    <ul className="list-group list-group-flush">
                      {Object.values(
                        carrito.reduce((acc, item) => {
                          const key = item.id;
                          if (!acc[key]) {
                            acc[key] = { ...item, cantidad: 0 };
                          }
                          acc[key].cantidad += 1;
                          return acc;
                        }, {})
                      ).map((p) => (
                        <li
                          key={p.id}
                          className="list-group-item d-flex justify-content-between align-items-center"
                        >
                          <div>
                            {p.productoCanjeable.nombre}
                            <span className="badge bg-secondary ms-2">{p.cantidad}</span>
                          </div>
                          <div className="d-flex align-items-center gap-2">
                            <small className="text-muted">{formatPrecio(p.precio * p.cantidad)}</small>
                            <button
                              className="btn btn-sm btn-outline-danger"
                              onClick={() => quitarDelCarrito(p.id)}
                            >
                              ✕
                            </button>
                          </div>
                        </li>
                      ))}
                    </ul>
                    <div className="d-flex justify-content-between align-items-center mt-3">
                      <strong>Total: {formatPrecio(totalCarrito)}</strong>
                      <div className="d-flex gap-2">
                        <button
                          className="btn btn-warning"
                          onClick={handleComprarCarrito}
                          disabled={compraLoading}
                        >
                          Comprar carrito
                        </button>
                        {tenantInfo && (
                          <button
                            className="btn btn-info"
                            onClick={handleComprarCarritoMixto}
                            disabled={compraLoading}
                          >
                            {`Dinero + ${tenantInfo.nombrePuntos || "Puntos"}`}
                          </button>
                        )}
                        <button
                          className="btn btn-success"
                          onClick={handleCanjearCarrito}
                          disabled={carritoLoading}
                        >
                          Canjear carrito
                        </button>
                      </div>
                    </div>
                    {carritoError && <p className="text-danger mt-2">{carritoError}</p>}
                    {compraError && <p className="text-danger mt-2">{compraError}</p>}
                  </div>
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
                {productos
                  .filter((p) => !categoriaSeleccionada || p.categoria === categoriaSeleccionada)
                  .map((productoUbicacion) => {
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
                            e.currentTarget.style.borderColor = "var(--primary-color)";
                          }
                        } }
                        onMouseLeave={(e) => {
                          e.currentTarget.style.transform = "translateY(0)";
                          e.currentTarget.style.boxShadow = "0 2px 8px rgba(0,0,0,0.1)";
                          e.currentTarget.style.borderColor = "#e9ecef";
                        } }
                      >
                        <div style={{ textAlign: "center", marginBottom: "1rem" }}>
                          <img
                            src={producto.fotoUrl || "placeholder-product.png"}
                            alt={producto.nombre}
                            style={{ width: "100%", height: "150px", objectFit: "cover", borderRadius: "8px" }} />
                        </div>
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
                              gap: "0.5rem",
                              marginBottom: "0.25rem"
                            }}
                          >
                            💵 {formatPrecio(productoUbicacion.precio)}
                          </div>
                          <div
                            style={{
                              fontSize: "1.2rem",
                              fontWeight: "bold",
                              color: "#856404",
                              display: "flex",
                              alignItems: "center",
                              justifyContent: "center",
                              gap: "0.5rem"
                            }}
                          >
                            🏅 {formatCosto(producto.costoEnPuntos)}
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
                              onClick={() => handleComprar(productoUbicacion)}
                              disabled={compraLoading}
                              style={{
                                width: "100%",
                                padding: "0.5rem",
                                backgroundColor: "#ffc107",
                                color: "#212529",
                                border: "none",
                                borderRadius: "6px",
                                cursor: "pointer"
                              }}
                            >
                              Comprar
                            </button>
                            <button
                              onClick={() => handleComprarMixto(productoUbicacion)}
                              disabled={compraLoading || !tenantInfo}
                              style={{
                                width: "100%",
                                padding: "0.5rem",
                                backgroundColor: "#17a2b8",
                                color: "white",
                                border: "none",
                                borderRadius: "6px",
                                cursor: "pointer"
                              }}
                            >
                              {`Comprar con Dinero + ${tenantInfo?.nombrePuntos || "Puntos"}`}
                            </button>
                            <button
                              onClick={() => handleCanjear(productoUbicacion)}
                              disabled={!userProfile || (userProfile.puntos || 0) < producto.costoEnPuntos || canjeLoading}
                              style={{
                                width: "100%",
                                padding: "0.5rem",
                                backgroundColor: "var(--primary-color)",
                                color: "white",
                                border: "none",
                                borderRadius: "6px",
                                cursor: "pointer"
                              }}
                            >
                              {`Canjear con ${tenantInfo?.nombrePuntos || "Puntos"}`}
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
                              🛒 Agregar al carrito
                            </button>
                          </div>
                        )}

                        {/* Footer con información adicional */}
                        <div style={{ height: "1rem" }} />
                      </div>
                    );
                  })}
              </div>
              {compraError && (
                <p style={{ color: "#dc3545", marginTop: "1rem" }}>{compraError}</p>
              )}
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
    <SeleccionarPuntosModal
        isOpen={mostrarPuntosModal}
        onClose={() => setMostrarPuntosModal(false)}
        maxPuntos={maxPuntosMixto}
        valorPunto={tenantInfo?.valorPunto || 0}
        precio={productoMixto?.precio || 0}
        nombrePuntos={tenantInfo?.nombrePuntos}
        onConfirm={confirmarCompraMixta} />
    <SeleccionarPuntosModal
        isOpen={mostrarPuntosCarrito}
        onClose={() => setMostrarPuntosCarrito(false)}
        maxPuntos={maxPuntosCarrito}
        valorPunto={tenantInfo?.valorPunto || 0}
        precio={totalCarrito}
        nombrePuntos={tenantInfo?.nombrePuntos}
        onConfirm={confirmarCompraCarritoMixto} />
    </>
  );
};

export default CatalogoProductos;