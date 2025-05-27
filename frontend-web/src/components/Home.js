import React, { useState, useEffect } from 'react'
import { useTenant } from '../context/TenantContext'
import authService from '../services/authService'
import { v4 as uuid } from 'uuid'

const MOCK_PRODUCTS = [
  { id: 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', nombre: 'Mock Prod A', precioUnitario: 50, categoria: 'Mock' },
  { id: 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', nombre: 'Mock Prod B', precioUnitario: 80, categoria: 'Mock' },
]
const MOCK_LOCATIONS = [
  { id: 'dddddddd-dddd-dddd-dddd-dddddddddddd', nombre: 'Ubicación X', productosLocales: MOCK_PRODUCTS },
  { id: 'eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee', nombre: 'Ubicación Y', productosLocales: MOCK_PRODUCTS },
]

export default function Home() {
  const { tenantId, tenants } = useTenant()
  const isAuth = authService.isAuthenticated()

  const [locs, setLocs] = useState([])
  const [selLoc, setSelLoc] = useState(null)
  const [selProd, setSelProd] = useState(null)
  const [res, setRes] = useState(null)

  useEffect(() => {
    if (!tenantId) return
    // Cargamos mock de ubicaciones y productos
    setLocs(MOCK_LOCATIONS)
    setSelLoc(MOCK_LOCATIONS[0])
    setSelProd(MOCK_PRODUCTS[0])
  }, [tenantId])

  const handleCompra = () => {
    const usuarioId = authService.getCurrentUser().id
    const mensaje = {
      Version: '1.0',
      IdMensaje: uuid(),
      TipoMensaje: 'transaccion',
      TenantId: tenantId,
      UbicacionId: selLoc.id,
      TerminalId: uuid(),
      Datos: {
        transaccion: {
          IdTransaccion: uuid(),
          IdentificadorUsuario: usuarioId,
          TipoTransaccion: 'minimercado',
          Monto: selProd.precioUnitario,
          MetodoPago: 'efectivo',
          Productos: [
            {
              IdProducto: selProd.id,
              NombreProducto: selProd.nombre,
              Categoria: selProd.categoria,
              Cantidad: 1,
              PrecioUnitario: selProd.precioUnitario,
              SubTotal: selProd.precioUnitario,
            },
          ],
        },
      },
    }

    // Fingimos la respuesta en el front
    setRes({
      idMensajeReferencia: uuid(),
      codigo: 'OK',
      mensaje: 'Transacción mock exitosa',
      timestamp: new Date().toISOString(),
      datos: mensaje.Datos,
    })
  }

  return (
    <div style={{ maxWidth: 800, margin: 'auto', padding: '1rem' }}>
      <h1>ServiPuntos</h1>
      {!isAuth && <p>Por favor inicia sesión.</p>}

      {isAuth && (
        <div style={{ background: '#f5f5f5', padding: 20, borderRadius: 8 }}>
          <h2>Prueba de Compra NAFTA</h2>
          <p>
            Tenant activo:{' '}
            <strong>{tenants.find(t => t.id === tenantId)?.nombre}</strong>
          </p>

          <label>
            Ubicación:{' '}
            <select
              value={selLoc?.id || ''}
              onChange={e => {
                const u = locs.find(l => l.id === e.target.value)
                setSelLoc(u)
                setSelProd(u.productosLocales[0])
              }}
            >
              {locs.map(l => (
                <option key={l.id} value={l.id}>
                  {l.nombre}
                </option>
              ))}
            </select>
          </label>
          <br />

          <label>
            Producto:{' '}
            <select
              value={selProd?.id || ''}
              onChange={e => {
                const p = selLoc.productosLocales.find(p => p.id === e.target.value)
                setSelProd(p)
              }}
            >
              {selLoc?.productosLocales.map(p => (
                <option key={p.id} value={p.id}>
                  {p.nombre} (${p.precioUnitario})
                </option>
              ))}
            </select>
          </label>
          <br />

          <button onClick={handleCompra} style={{ marginTop: 10 }}>
            Simular Compra NAFTA
          </button>

          {res && (
            <pre style={{ marginTop: 20, textAlign: 'left' }}>
              {JSON.stringify(res, null, 2)}
            </pre>
          )}
        </div>
      )}
    </div>
  )
}
