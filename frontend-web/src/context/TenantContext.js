import React, { createContext, useContext, useState, useEffect } from 'react'

const MOCK_TENANTS = [
    { id: '11111111-1111-1111-1111-111111111111', nombre: 'TENANT-MOCK-1' },
    { id: '22222222-2222-2222-2222-222222222222', nombre: 'TENANT-MOCK-2' },
]

const TenantContext = createContext({
    tenantId: null,
    tenants: [],
    setTenantId: () => { },
})

export function TenantProvider({ children }) {
    const [tenants, setTenants] = useState([])
    const [tenantId, setTenantId] = useState(null)

    useEffect(() => {
        setTenants(MOCK_TENANTS)
        setTenantId(MOCK_TENANTS[0].id)
    }, [])

    return (
        <TenantContext.Provider value={{ tenantId, tenants, setTenantId }}>
            {children}
        </TenantContext.Provider>
    )
}

export function useTenant() {
    return useContext(TenantContext)
}
