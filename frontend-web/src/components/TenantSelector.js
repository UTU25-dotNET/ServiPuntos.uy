import React from 'react'
import { useTenant } from '../context/TenantContext'

export default function TenantSelector() {
    const { tenants, tenantId, setTenantId } = useTenant()

    return (
        <select
            value={tenantId || ''}
            onChange={e => setTenantId(e.target.value)}
        >
            <option value='' disabled>
                — Seleccione un Tenant —
            </option>
            {tenants.map(t => (
                <option key={t.id} value={t.id}>
                    {t.nombre}
                </option>
            ))}
        </select>
    )
}
