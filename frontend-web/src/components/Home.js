import React, { useState, useEffect } from "react";
import { Link } from "react-router-dom";
import authService from "../services/authService";
import apiService from "../services/apiService";
import { v4 as uuid } from "uuid";

const Home = ({ selectedTenant }) => {
  const isAuthenticated = authService.isAuthenticated();
  const [tenantDetail, setTenantDetail] = useState(null);
  const [selectedUbicacion, setSelectedUbicacion] = useState(null);
  const [selectedProducto, setSelectedProducto] = useState(null);
  const [respuesta, setRespuesta] = useState(null);

  // 1) Cuando cambia el tenant, cargo su detalle (incluye ubicaciones y productos)
  useEffect(() => {
    if (!selectedTenant) {
      setTenantDetail(null);
      setSelectedUbicacion(null);
      setSelectedProducto(null);
      return;
    }

    apiService
      .get(`tenant/${selectedTenant.id}`)    // <-- devuelve directamente el JSON
      .then((data) => {
        setTenantDetail(data);

        // selecciono por defecto la primera ubicación
        if (data.ubicaciones?.length) {
          const u = data.ubicaciones[0];
          setSelectedUbicacion(u);

          // y dentro de ella el primer producto local
          if (u.productosLocales?.length) {
            setSelectedProducto(u.productosLocales[0]);
          }
        }
      })
      .catch((err) => console.error("Error cargando tenant:", err));
  }, [selectedTenant]);

  // 2) Simular la compra usando GUIDs reales
  const handleCompra = async () => {
    if (!tenantDetail || !selectedUbicacion || !selectedProducto) return;

    const mensaje = {
      Version: "1.0",
      IdMensaje: uuid(),
      TipoMensaje: "transaccion",
      TenantId: tenantDetail.id,
      UbicacionId: selectedUbicacion.id,
      TerminalId: uuid(),
      Datos: {
        transaccion: {
          IdTransaccion: uuid(),
          IdentificadorUsuario: authService.getCurrentUser().id,
          TipoTransaccion: "minimercado",
          Monto: selectedProducto.precioUnitario * (selectedProducto.cantidad || 1),
          MonedaTransaccion: "UYU",
          MetodoPago: "efectivo",
          Productos: [
            {
              IdProducto: selectedProducto.id,         // <-- GUID real
              NombreProducto: selectedProducto.nombre,
              Categoria: selectedProducto.categoria,
              Cantidad: selectedProducto.cantidad || 1,
              PrecioUnitario: selectedProducto.precioUnitario,
              SubTotal:
                selectedProducto.precioUnitario *
                (selectedProducto.cantidad || 1),
            },
          ],
        },
      },
    };

    console.log("Payload NAFTA ->", mensaje);

    try {
      const res = await apiService.simulateTransaccion(mensaje);
      setRespuesta(res);
    } catch (err) {
      console.error("Error al llamar NAFTA:", err);
      setRespuesta(
        err.response?.data || { Codigo: "ERROR", Mensaje: err.message }
      );
    }
  };

  return (
    <div style={{ maxWidth: 800, margin: "0 auto", padding: "1rem" }}>
      <h1 style={{ color: "#7B3F00", textAlign: "center" }}>Servipuntos</h1>
      {isAuthenticated ? (
        <div
          style={{
            backgroundColor: "#f8f9fa",
            padding: "2rem",
            borderRadius: 8,
          }}
        >
          <h2>¡Bienvenido!</h2>
          <Link
            to="/dashboard"
            style={{
              display: "inline-block",
              backgroundColor: "#007bff",
              color: "white",
              padding: "0.5rem 1rem",
              borderRadius: 4,
              textDecoration: "none",
            }}
          >
            Ir al Dashboard
          </Link>

          <section style={{ marginTop: "2rem" }}>
            <h3>Prueba de Compra NAFTA</h3>

            {tenantDetail ? (
              <>
                <p>
                  Tenant activo: <strong>{tenantDetail.nombre}</strong>
                </p>

                <label>
                  Ubicación:{" "}
                  <select
                    value={selectedUbicacion?.id || ""}
                    onChange={(e) => {
                      const u = tenantDetail.ubicaciones.find(
                        (x) => x.id === e.target.value
                      );
                      setSelectedUbicacion(u);
                      setSelectedProducto(u.productosLocales?.[0] || null);
                    }}
                  >
                    {tenantDetail.ubicaciones.map((u) => (
                      <option key={u.id} value={u.id}>
                        {u.nombre || u.direccion}
                      </option>
                    ))}
                  </select>
                </label>
                <br />
                <label>
                  Producto:{" "}
                  <select
                    value={selectedProducto?.id || ""}
                    onChange={(e) => {
                      const p = selectedUbicacion.productosLocales.find(
                        (x) => x.id === e.target.value
                      );
                      setSelectedProducto(p);
                    }}
                  >
                    {selectedUbicacion.productosLocales.map((p) => (
                      <option key={p.id} value={p.id}>
                        {p.nombre} (${p.precioUnitario})
                      </option>
                    ))}
                  </select>
                </label>
                <br />

                <button
                  onClick={handleCompra}
                  style={{ marginTop: "1rem", padding: "0.5rem 1rem" }}
                >
                  Simular Compra NAFTA
                </button>

                {respuesta && (
                  <pre style={{ marginTop: "1rem", textAlign: "left" }}>
                    {JSON.stringify(respuesta, null, 2)}
                  </pre>
                )}
              </>
            ) : (
              <p>Selecciona un tenant en la barra superior para probar.</p>
            )}
          </section>
        </div>
      ) : (
        <p style={{ textAlign: "center" }}>
          Por favor <Link to="/login">inicia sesión</Link>.
        </p>
      )}
    </div>
  );
};

export default Home;
