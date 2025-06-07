import React, { useState, useEffect } from "react";
import { Link } from "react-router-dom";
import apiService from "../../services/apiService";

const PuntosWidget = ({ userProfile, tenantInfo }) => {
  const [productosCanjeables, setProductosCanjeables] = useState([]);
  const [loadingProductos, setLoadingProductos] = useState(true);
  const [estadisticas, setEstadisticas] = useState({
    productosDisponibles: 0,
    productoMasBarato: null,
    productoMasCaro: null
  });

  useEffect(() => {
    if (userProfile) {
      loadProductosInfo();
    }
  }, [userProfile]);

  const loadProductosInfo = async () => {
    setLoadingProductos(true);
    try {
      // Obtener todos los productos disponibles para calcular estadísticas
      const productos = await apiService.getAllProductosUbicacion();
      
      console.log("Productos obtenidos:", productos?.length); // Debug log
      
      if (productos && productos.length > 0) {
        // Debug: examinar estructura de los primeros productos
        console.log("Muestra del primer producto:", productos[0]);
        console.log("Estructura del productoCanjeable:", productos[0]?.productoCanjeable);
        
        // Debug: verificar cada condición del filtro individualmente
        const activosCount = productos.filter(pu => pu.activo).length;
        const conStockCount = productos.filter(pu => pu.stockDisponible > 0).length;
        const conCanjeableCount = productos.filter(pu => pu.productoCanjeable).length;
        
        console.log("Productos activos:", activosCount);
        console.log("Productos con stock > 0:", conStockCount);
        console.log("Productos con productoCanjeable:", conCanjeableCount);
        
        // Filtrar productos únicos y activos
        const productosUnicos = productos
          .filter(pu => {
            const cumpleActivo = pu.activo;
            const cumpleStock = pu.stockDisponible > 0;
            const cumpleCanjeable = pu.productoCanjeable;
            
            if (!cumpleActivo || !cumpleStock || !cumpleCanjeable) {
              console.log("Producto filtrado:", {
                id: pu.id,
                activo: pu.activo,
                stock: pu.stockDisponible,
                tieneCanjeable: !!pu.productoCanjeable,
                canjeable: pu.productoCanjeable
              });
            }
            
            return cumpleActivo && cumpleStock && cumpleCanjeable;
          })
          .reduce((acc, current) => {
            const existing = acc.find(p => p.productoCanjeable.id === current.productoCanjeable.id);
            if (!existing) {
              acc.push(current);
            }
            return acc;
          }, []);
  
        console.log("Productos únicos filtrados:", productosUnicos?.length); // Debug log
        setProductosCanjeables(productosUnicos);
  
        // Calcular estadísticas
        if (productosUnicos.length > 0) {
          const productosConPuntos = productosUnicos
            .map(p => p.productoCanjeable)
            .filter(p => p && typeof p.costoEnPuntos === 'number' && p.costoEnPuntos > 0); // ← FIX: Filtrar productos con costoEnPuntos válido
  
          console.log("Productos con puntos válidos:", productosConPuntos?.length); // Debug log
          console.log("Puntos del usuario:", userProfile.puntos); // Debug log
  
          const puntosUsuario = userProfile.puntos || 0;
          
          const productosCanjeablesConPuntos = productosConPuntos.filter(p => 
            p.costoEnPuntos <= puntosUsuario
          );
  
          console.log("Productos canjeables con puntos:", productosCanjeablesConPuntos?.length); // Debug log
  
          // ← FIX: Verificar que hay productos válidos antes de calcular min/max
          const productoMasBarato = productosConPuntos.length > 0 
            ? productosConPuntos.reduce((min, producto) => 
                producto.costoEnPuntos < min.costoEnPuntos ? producto : min
              )
            : null;
  
          const productoMasCaro = productosCanjeablesConPuntos.length > 0 
            ? productosCanjeablesConPuntos.reduce((max, producto) => 
                producto.costoEnPuntos > max.costoEnPuntos ? producto : max
              )
            : null;
  
          console.log("Producto más barato:", productoMasBarato); // Debug log
          console.log("Producto más caro canjeable:", productoMasCaro); // Debug log
  
          setEstadisticas({
            productosDisponibles: productosCanjeablesConPuntos.length,
            productoMasBarato,
            productoMasCaro
          });
        } else {
          // ← FIX: Resetear estadísticas si no hay productos
          setEstadisticas({
            productosDisponibles: 0,
            productoMasBarato: null,
            productoMasCaro: null
          });
        }
      } else {
        console.log("No se obtuvieron productos"); // Debug log
        // ← FIX: Resetear estadísticas si no hay productos
        setEstadisticas({
          productosDisponibles: 0,
          productoMasBarato: null,
          productoMasCaro: null
        });
      }
    } catch (error) {
      console.error("Error al cargar información de productos:", error);
      // ← FIX: Resetear estadísticas en caso de error
      setEstadisticas({
        productosDisponibles: 0,
        productoMasBarato: null,
        productoMasCaro: null
      });
    } finally {
      setLoadingProductos(false);
    }
  };

  const formatPuntos = (puntos) => {
    if (!puntos) return "0";
    return puntos.toLocaleString();
  };

  const getPuntosColor = (puntos) => {
    if (!puntos || puntos === 0) return "#6c757d";
    if (puntos < 100) return "#fd7e14";    // Naranja - pocos puntos
    if (puntos < 500) return "#ffc107";    // Amarillo - puntos moderados
    if (puntos < 1000) return "#28a745";   // Verde - buenos puntos
    return "#007bff";                      // Azul - muchos puntos
  };

  const getPuntosMessage = (puntos) => {
    if (!puntos || puntos === 0) return "¡Comienza a acumular puntos!";
    if (puntos < 100) return "¡Sigue acumulando para tu primer canje!";
    if (puntos < 500) return "¡Ya puedes canjear algunos productos!";
    if (puntos < 1000) return "¡Tienes buenos puntos acumulados!";
    return "¡Excelente! Tienes muchos puntos disponibles";
  };

  const puntosActuales = userProfile.puntos || 0;
  const colorPuntos = getPuntosColor(puntosActuales);

  return (
    <div
      style={{
        padding: "1.5rem",
        backgroundColor: "white",
        borderRadius: "12px",
        boxShadow: "0 2px 8px rgba(0,0,0,0.1)",
        border: "1px solid #e9ecef",
        transition: "transform 0.2s ease, box-shadow 0.2s ease"
      }}
      onMouseEnter={(e) => {
        e.currentTarget.style.transform = "translateY(-2px)";
        e.currentTarget.style.boxShadow = "0 8px 25px rgba(0,0,0,0.15)";
      }}
      onMouseLeave={(e) => {
        e.currentTarget.style.transform = "translateY(0)";
        e.currentTarget.style.boxShadow = "0 2px 8px rgba(0,0,0,0.1)";
      }}
    >
      {/* Header del Widget */}
      <div
        style={{
          display: "flex",
          alignItems: "center",
          justifyContent: "space-between",
          marginBottom: "1.5rem"
        }}
      >
        <h3
          style={{
            margin: "0",
            color: "#212529",
            fontSize: "1.3rem",
            fontWeight: "600",
            display: "flex",
            alignItems: "center",
            gap: "0.5rem"
          }}
        >
          💰 Mis Puntos
        </h3>
        <Link
          to="/perfil"
          style={{
            fontSize: "0.9rem",
            color: "#007bff",
            textDecoration: "none",
            fontWeight: "500"
          }}
        >
          Ver historial →
        </Link>
      </div>

      {/* Puntos Principales */}
      <div
        style={{
          textAlign: "center",
          marginBottom: "1.5rem",
          padding: "1.5rem",
          backgroundColor: "#f8f9fa",
          borderRadius: "8px",
          border: `2px solid ${colorPuntos}20`
        }}
      >
        <div
          style={{
            fontSize: "3rem",
            fontWeight: "bold",
            color: colorPuntos,
            marginBottom: "0.5rem",
            lineHeight: "1"
          }}
        >
          {formatPuntos(puntosActuales)}
        </div>
        <div
          style={{
            fontSize: "0.9rem",
            color: "#6c757d",
            fontWeight: "500",
            textTransform: "uppercase",
            letterSpacing: "0.5px",
            marginBottom: "0.5rem"
          }}
        >
          {tenantInfo.nombrePuntos || "Puntos"} Disponibles
        </div>
        <div
          style={{
            fontSize: "0.9rem",
            color: colorPuntos,
            fontWeight: "500",
            fontStyle: "italic"
          }}
        >
          {getPuntosMessage(puntosActuales)}
        </div>
      </div>

      {/* Estadísticas de Productos */}
      {loadingProductos ? (
        <div style={{ textAlign: "center", padding: "1rem" }}>
          <div
            style={{
              border: "2px solid #f3f3f3",
              borderTop: "2px solid #007bff",
              borderRadius: "50%",
              width: "24px",
              height: "24px",
              animation: "spin 1s linear infinite",
              margin: "0 auto 0.5rem"
            }}
          />
          <span style={{ color: "#6c757d", fontSize: "0.9rem" }}>Calculando productos disponibles...</span>
        </div>
      ) : (
        <div style={{ marginBottom: "1.5rem" }}>
          <div
            style={{
              display: "grid",
              gridTemplateColumns: "1fr 1fr",
              gap: "1rem",
              marginBottom: "1rem"
            }}
          >
            <div
              style={{
                textAlign: "center",
                padding: "0.75rem",
                backgroundColor: "#e3f2fd",
                borderRadius: "6px",
                border: "1px solid #bbdefb"
              }}
            >
              <div
                style={{
                  fontSize: "1.5rem",
                  fontWeight: "bold",
                  color: "#1976d2",
                  marginBottom: "0.25rem"
                }}
              >
                {estadisticas.productosDisponibles}
              </div>
              <div style={{ fontSize: "0.8rem", color: "#1565c0" }}>
                Productos canjeables
              </div>
            </div>

            <div
              style={{
                textAlign: "center",
                padding: "0.75rem",
                backgroundColor: "#e8f5e8",
                borderRadius: "6px",
                border: "1px solid #a5d6a7"
              }}
            >
              <div
                style={{
                  fontSize: "1.5rem",
                  
                  color: "#388e3c",
                  marginBottom: "0.25rem"
                }}
              >
                {estadisticas.productoMasBarato ? estadisticas.productoMasBarato.nombre : "N/A"}
              </div>
              <div style={{ fontSize: "0.8rem", color: "#2e7d32" }}>
                Producto más barato
              </div>
            </div>
          </div>

          {/* Producto destacado */}
          {estadisticas.productoMasCaro && (
            <div
              style={{
                padding: "1rem",
                backgroundColor: "#fff3cd",
                borderRadius: "8px",
                border: "1px solid #ffeaa7"
              }}
            >
              <div
                style={{
                  fontSize: "0.85rem",
                  color: "#856404",
                  fontWeight: "600",
                  marginBottom: "0.5rem",
                  display: "flex",
                  alignItems: "center",
                  gap: "0.5rem"
                }}
              >
                🎁 Producto destacado que puedes canjear:
              </div>
              <div
                style={{
                  fontSize: "0.9rem",
                  color: "#7B3F00",
                  fontWeight: "500"
                }}
              >
                {estadisticas.productoMasCaro.nombre} - {estadisticas.productoMasCaro.costoEnPuntos} pts
              </div>
            </div>
          )}
        </div>
      )}

      {/* Acciones */}


      {/* Información adicional */}
      <div
        style={{
          marginTop: "1rem",
          padding: "0.75rem",
          backgroundColor: "#f8f9fa",
          borderRadius: "6px",
          fontSize: "0.8rem",
          color: "#6c757d",
          textAlign: "center"
        }}
      >
        💡 Los puntos no tienen fecha de vencimiento
      </div>
    </div>
  );
};

export default PuntosWidget;